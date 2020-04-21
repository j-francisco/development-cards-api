using System.Collections.Generic;

namespace DevelopmentCardsApi.Services
{
    public interface IPlayerConnectionManager
    {
        void StorePlayerConnection(string playerToken, string connectionId);
        void RemovePlayerConnection(string playerToken, string connectionId);
        IReadOnlyList<string> GetConnectionIds(string playerToken);
    }
}
