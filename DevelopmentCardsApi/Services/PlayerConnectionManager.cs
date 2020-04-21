using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentCardsApi.Services
{
    public class PlayerConnectionManager : IPlayerConnectionManager
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _connectedPlayers;

        public PlayerConnectionManager()
        {
            _connectedPlayers = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
        }

        public void StorePlayerConnection(string playerToken, string connectionId)
        {
            var connectionIds = _connectedPlayers.GetOrAdd(playerToken, new ConcurrentDictionary<string, string>());

            if (!connectionIds.ContainsKey(connectionId))
            {
                connectionIds.TryAdd(connectionId, playerToken);
            }
        }

        public void RemovePlayerConnection(string playerToken, string connectionId)
        {
            if (!_connectedPlayers.TryGetValue(playerToken, out var connectionIds))
            {
                return;
            }

            connectionIds.TryRemove(connectionId, out string _);

            if (connectionIds.IsEmpty)
            {
                _connectedPlayers.TryRemove(playerToken, out var _);
            }
        }

        public IReadOnlyList<string> GetConnectionIds(string playerToken)
        {
            if (!_connectedPlayers.TryGetValue(playerToken, out var connectionIdLookup))
            {
                return new List<string>();
            }

            return connectionIdLookup.Keys.ToList();
        }
    }
}
