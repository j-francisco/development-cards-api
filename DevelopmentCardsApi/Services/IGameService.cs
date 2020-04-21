using System.Threading.Tasks;
using DevelopmentCardsApi.DataTransferObjects;
using DevelopmentCardsApi.Models;

namespace DevelopmentCardsApi.Services
{
    public interface IGameService
    {
        Task<Player> CreatePlayer(string name);
        Task<Game> GetGame(string gameCode);
        Task<Game> GetGame(int gameId);
        Task<Game> CreateGame(NewGameRequest gameRequest);
        GameResponse GetGameResponse(Game game, Player player);
        Task<GameResponse> JoinGame(string gameCode, Player player);
        Task LeaveGame(Game game, Player player);
        Task EndGame(Game game);
        Task<GameResponse> PullCard(Player player);
        Task<GameResponse> PlayCard(Player player, int gameCardId);
        Task SendGameUpdateToClients(int gameId, int currentPlayerId);
    }
}
