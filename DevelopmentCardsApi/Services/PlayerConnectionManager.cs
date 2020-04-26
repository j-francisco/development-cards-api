using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentCardsApi.Services
{
    public class PlayerConnectionManager : IPlayerConnectionManager
    {
        // Key is ConnectionId, Value is PlayerToken
        // A PlayerToken may belong to multiple ConnectionIds
        private readonly ConcurrentDictionary<string, string> _connectedPlayers;

        public PlayerConnectionManager()
        {
            _connectedPlayers = new ConcurrentDictionary<string, string>();
        }

        public void StorePlayerConnection(string playerToken, string connectionId)
        {
            _connectedPlayers[connectionId] = playerToken;
        }

        public void RemovePlayerConnection(string connectionId)
        {
            _connectedPlayers.TryRemove(connectionId, out string _);
        }

        public IReadOnlyList<string> GetConnectionIds(string playerToken)
        {
            return _connectedPlayers.Where(pair => pair.Value == playerToken).Select(pair => pair.Key).ToList();
        }
    }
}
