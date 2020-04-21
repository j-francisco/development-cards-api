using System;
using System.Collections.Generic;
using DevelopmentCardsApi.Models;

namespace DevelopmentCardsApi.DataTransferObjects
{
    public class PlayerGameHand
    {
        public PlayerGameHand()
        {
        }

        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public ICollection<GameCard> CardsPlayed { get; set; }

        public int CardsUnplayedCount { get; set; }
    }
}
