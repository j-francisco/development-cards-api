using System.Collections.Generic;
using DevelopmentCardsApi.Models;

namespace DevelopmentCardsApi.DataTransferObjects
{
    public class GameResponse
    {
        public GameResponse()
        {
        }

        public int GameId { get; set; }

        public string GameCode { get; set; }

        public int AvailableCardCount { get; set; }

        public string CurrentPlayerName { get; set; }

        public ICollection<GameCard> CurrentPlayerCards { get; set; }

        public ICollection<PlayerGameHand> OtherPlayerHands { get; set; }
    }
}
