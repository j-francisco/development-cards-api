using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using DevelopmentCardsApi.Data;
using DevelopmentCardsApi.Models;
using DevelopmentCardsApi.Services;
using DevelopmentCardsApi.DataTransferObjects;
using DevelopmentCardsApi.Hubs;

namespace DevelopmentCardsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly GameContext _context;
        private readonly IGameService _gameService;
        private readonly IHubContext<GameHub> _gameHubContext;

        public GameController(GameContext context, IGameService gameService, IHubContext<GameHub> gameHubContext)
        {
            _context = context;
            _gameService = gameService;
            _gameHubContext = gameHubContext;
        }

        // GET: api/Game
        [HttpGet]
        public async Task<ActionResult<GameResponse>> GetGame()
        {
            var currentPlayer = await GetCurrentPlayer();

            if (currentPlayer?.GameId == null)
            {
                return NotFound();
            }

            var game = await _gameService.GetGame(currentPlayer.GameId.Value);

            if (game == null)
            {
                return NotFound();
            }

            return _gameService.GetGameResponse(game, currentPlayer);
        }

        // POST: api/Game
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<GameResponse>> PostGame(NewGameRequest gameRequest)
        {
            var game = await _gameService.CreateGame(gameRequest);

            var currentPlayer = game.Players.First();

            Response.Headers.Add("X-CurrentPlayerToken", currentPlayer.Token.ToString());

            var gameResponse = _gameService.GetGameResponse(game, currentPlayer);

            return CreatedAtAction("GetGame", new { id = game.Id }, gameResponse);
        }

        // POST: api/Game/{gameCode}/Join
        [HttpPost]
        [Route("{gameCode}/Join")]
        public async Task<ActionResult<GameResponse>> JoinGame(string gameCode, JoinGameRequest joinGameRequest)
        {
            if (string.IsNullOrEmpty(gameCode))
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(joinGameRequest.PlayerName))
            {
                return BadRequest();
            }

            var player = await _gameService.CreatePlayer(joinGameRequest.PlayerName);

            var gameResponse = await _gameService.JoinGame(gameCode, player);

            if (gameResponse == null)
            {
                return NotFound();
            }

            await SendGameUpdateToClients(gameResponse.GameId, player.Id);

            Response.Headers.Add("X-CurrentPlayerToken", player.Token.ToString());

            return gameResponse;
        }

        // POST: api/Game/{gameCode}/Leave
        [HttpPost]
        [Route("{gameCode}/Leave")]
        public async Task<IActionResult> LeaveGame(string gameCode)
        {
            if (string.IsNullOrEmpty(gameCode))
            {
                return BadRequest();
            }

            var currentPlayer = await GetCurrentPlayer();

            if (currentPlayer == null)
            {
                return BadRequest();
            }

            var game = await _gameService.GetGame(gameCode);

            if (game == null)
            {
                return NotFound();
            }

            if (currentPlayer.GameId != game.Id)
            {
                return NotFound();
            }

            await _gameService.LeaveGame(game, currentPlayer);

            await SendGameUpdateToClients(game.Id, currentPlayer.Id);

            return Ok();
        }

        // POST: api/Game/{gameCode}/End
        [HttpPost]
        [Route("{gameCode}/End")]
        public async Task<IActionResult> EndGame(string gameCode)
        {
            if (string.IsNullOrEmpty(gameCode))
            {
                return BadRequest();
            }

            var currentPlayer = await GetCurrentPlayer();

            if (currentPlayer == null)
            {
                return BadRequest();
            }

            var game = await _gameService.GetGame(gameCode);

            if (game == null)
            {
                return NotFound();
            }

            // Ensure only a player in the game can end the game
            if (!game.Players.Any(p => p.Id == currentPlayer.Id))
            {
                return NotFound();
            }

            await _gameService.EndGame(game);

            // TODO send game ended to clients

            return Ok();
        }

        // POST: api/Game/PullCard
        [HttpPost]
        [Route("PullCard")]
        public async Task<ActionResult<GameResponse>> PullCard()
        {
            var currentPlayer = await GetCurrentPlayer();

            if (currentPlayer == null)
            {
                return BadRequest();
            }

            var gameResponse = await _gameService.PullCard(currentPlayer);

            if (gameResponse == null)
            {
                return BadRequest();
            }

            await SendGameUpdateToClients(gameResponse.GameId, currentPlayer.Id);

            return gameResponse;
        }

        // POST: api/Game/PlayCard
        [HttpPost]
        [Route("PlayCard")]
        public async Task<ActionResult<GameResponse>> PlayCard(PlayCardRequest playCardRequest)
        {
            var currentPlayer = await GetCurrentPlayer();

            if (currentPlayer == null || playCardRequest == null)
            {
                return BadRequest();
            }

            var gameResponse = await _gameService.PlayCard(currentPlayer, playCardRequest.GameCardId);

            if (gameResponse == null)
            {
                return BadRequest();
            }

            await SendGameUpdateToClients(gameResponse.GameId, currentPlayer.Id);

            return gameResponse;
        }

        private async Task<Player> GetCurrentPlayer()
        {
            string currentPlayerToken = Request.Headers["X-CurrentPlayerToken"];

            if (!Guid.TryParse(currentPlayerToken, out Guid playerGuid))
            {
                return null;
            }

            // TODO handle if not found, etc

            return await _context.Players.FirstOrDefaultAsync(p => p.Token == playerGuid);
        }

        private async Task SendGameUpdateToClients(int gameId, int currentPlayerId)
        {
            await _gameService.SendGameUpdateToClients(gameId, currentPlayerId);
        }
    }
}
