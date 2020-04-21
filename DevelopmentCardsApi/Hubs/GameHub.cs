using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using DevelopmentCardsApi.DataTransferObjects;
using DevelopmentCardsApi.Services;
using Microsoft.AspNetCore.SignalR;

namespace DevelopmentCardsApi.Hubs
{
    public class GameHub : Hub
    {
        private readonly IPlayerConnectionManager _playerConnectionManager;

        public GameHub(IPlayerConnectionManager playerConnectionManager)
        {
            _playerConnectionManager = playerConnectionManager;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            await Clients.Caller.SendAsync("Message", "Successfully Connected");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Caller.SendAsync("Message", "Successfully Disconnected");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SubscribeGame(GameSubscription gameSubscription)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Game-{gameSubscription.GameId}");

            _playerConnectionManager.StorePlayerConnection(gameSubscription.PlayerToken, Context.ConnectionId);

            await Clients.Caller.SendAsync("Message", "Successfully Subscribed");
        }

        public async Task UnsubscribeGame(GameSubscription gameSubscription)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Game-{gameSubscription.GameId}");

            _playerConnectionManager.RemovePlayerConnection(gameSubscription.PlayerToken, Context.ConnectionId);

            await Clients.Caller.SendAsync("Message", "Successfully Unsubscribed");
        }
    }
}
