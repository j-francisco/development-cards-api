using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevelopmentCardsApi.Data;
using DevelopmentCardsApi.DataTransferObjects;
using DevelopmentCardsApi.Hubs;
using DevelopmentCardsApi.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DevelopmentCardsApi.Services
{
    public class GameService : IGameService
    {
        private GameContext _gameContext;
        private readonly IHubContext<GameHub> _gameHubContext;
        private IPlayerConnectionManager _playerConnectionManager;

        public GameService(
            GameContext gameContext,
            IHubContext<GameHub> gameHubContext,
            IPlayerConnectionManager playerConnectionManager)
        {
            _gameContext = gameContext;
            _gameHubContext = gameHubContext;
            _playerConnectionManager = playerConnectionManager;
        }

        public async Task<Player> CreatePlayer(string name)
        {
            var player = new Player
            {
                Name = name,
                Token = Guid.NewGuid()
            };

            _gameContext.Players.Add(player);

            await _gameContext.SaveChangesAsync();

            return player;
        }

        public async Task<Game> GetGame(string gameCode)
        {
            var gameCodeUpper = gameCode.ToUpperInvariant();
            return await _gameContext.Games
                .Include(g => g.Players)
                .Include(g => g.GameCards).ThenInclude(gameCard => gameCard.DevelopmentCard)
                .FirstOrDefaultAsync(g => g.Code == gameCodeUpper);
        }

        public async Task<Game> GetGame(int gameId)
        {
            return await _gameContext.Games
                .Include(g => g.Players)
                .Include(g => g.GameCards).ThenInclude(gameCard => gameCard.DevelopmentCard)
                .FirstOrDefaultAsync(g => g.Id == gameId);
        }

        public async Task<Game> CreateGame(NewGameRequest gameRequest)
        {
            var player = new Player
            {
                Name = gameRequest.PlayerName,
                Token = Guid.NewGuid()
            };

            // TODO check this hasn't been used.
            var code = GenerateGameCode().ToUpper();

            var startingCards = await GetStartingCards();

            var game = new Game
            {
                Code = code,
            };

            game.GameCards = startingCards;

            game.Players.Add(player);

            _gameContext.Games.Add(game);
            await _gameContext.SaveChangesAsync();

            player.OriginalGameId = game.Id;

            await _gameContext.SaveChangesAsync();

            return game;
        }

        public GameResponse GetGameResponse(Game game, Player currentPlayer)
        {
            var currentPlayerCards = game.GameCards
                .Where(gc => gc.Player?.Id == currentPlayer.Id)
                .OrderBy(gc => gc.PulledAt).ToList();

            var otherPlayerHands = new List<PlayerGameHand>();
            var otherPlayers = game.Players.Where(p => p.Id != currentPlayer.Id).OrderBy(p => p.Name);
            foreach (var otherPlayer in otherPlayers)
            {
                var otherPlayerCards = game.GameCards
                    .Where(gc => gc.Player?.Id == otherPlayer.Id)
                    .OrderBy(gc => gc.PlayedAt).ToList();

                var cardsPlayed = otherPlayerCards.Where(c => c.Played).ToList();

                var cardsUnplayedCount = otherPlayerCards.Count(c => !c.Played);

                var hand = new PlayerGameHand
                {
                    PlayerId = otherPlayer.Id,
                    PlayerName = otherPlayer.Name,
                    CardsPlayed = cardsPlayed,
                    CardsUnplayedCount = cardsUnplayedCount
                };

                otherPlayerHands.Add(hand);
            }

            var gameResponse = new GameResponse
            {
                GameId = game.Id,
                GameCode = game.Code,
                AvailableCardCount = game.AvailableGameCards.Count,
                CurrentPlayerCards = currentPlayerCards,
                OtherPlayerHands = otherPlayerHands,
                CurrentPlayerName = currentPlayer.Name
            };

            return gameResponse;
        }

        public async Task<GameResponse> JoinGame(string gameCode, Player player)
        {
            var game = await GetGame(gameCode);

            if (game == null)
            {
                return null;
            }

            game.Players.Add(player);

            player.OriginalGameId = game.Id;

            await _gameContext.SaveChangesAsync();

            return GetGameResponse(game, player);
        }

        public async Task LeaveGame(Game game, Player player)
        {
            game.Players.Remove(player);
            player.GameId = null;

            if (!game.Players.Any())
            {
                // Delete game if no more players
                await EndGame(game);
                return;
            }

            await _gameContext.SaveChangesAsync();
        }

        public async Task EndGame(Game game)
        {
            _gameContext.GameCards.RemoveRange(game.GameCards);

            _gameContext.Players.RemoveRange(_gameContext.Players.Where(p => p.OriginalGameId == game.Id));

            _gameContext.Games.Remove(game);

            await _gameContext.SaveChangesAsync();
        }

        public async Task<GameResponse> PullCard(Player player)
        {
            if (player.GameId == null)
            {
                return null;
            }

            var game = await GetGame(player.GameId.Value);

            if (!game.AvailableGameCards.Any())
            {
                // Maybe throw exception with a specific message?
                return GetGameResponse(game, player);
            }

            var cardDeck = game.AvailableGameCards.ToList();

            var random = new Random();
            var index = random.Next(cardDeck.Count);
            var card = cardDeck[index];

            card.Player = player;
            card.PulledAt = DateTime.Now;

            await _gameContext.SaveChangesAsync();

            return GetGameResponse(game, player);
        }

        public async Task<GameResponse> PlayCard(Player player, int gameCardId)
        {
            if (player.GameId == null)
            {
                return null;
            }

            var game = await GetGame(player.GameId.Value);

            var gameCard = game.GameCards.FirstOrDefault(gc => gc.Id == gameCardId);

            if (gameCard == null)
            {
                return null;
            }

            gameCard.Played = true;
            gameCard.PlayedAt = DateTime.Now;

            await _gameContext.SaveChangesAsync();

            return GetGameResponse(game, player);
        }

        public async Task SendGameUpdateToClients(int gameId, int currentPlayerId)
        {
            var game = await GetGame(gameId);

            var otherPlayers = game.Players.Where(p => p.Id != currentPlayerId);
            foreach (var otherPlayer in otherPlayers)
            {
                var gameResponse = GetGameResponse(game, otherPlayer);

                var connectionIds = _playerConnectionManager.GetConnectionIds(otherPlayer.Token.ToString());

                await _gameHubContext.Clients.Clients(connectionIds).SendAsync("ReceiveGame", gameResponse);
            }
        }

        private string GenerateGameCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var stringChars = new char[4];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new string(stringChars);

            return finalString;
        }

        private async Task<List<GameCard>> GetStartingCards()
        {
            var developmentCards = await _gameContext.DevelopmentCards.AsNoTracking().ToListAsync();

            return developmentCards.Select(dc => new GameCard { DevelopmentCardId = dc.Id }).ToList();
        }
    }
}
