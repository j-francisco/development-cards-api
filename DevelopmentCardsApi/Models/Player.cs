using System;
using System.Collections.Generic;

namespace DevelopmentCardsApi.Models
{
    public class Player
    {
        public Player()
        {
            GameCards = new List<GameCard>();
        }

        public int Id { get; set; }

        public Guid Token { get; set; }

        public string Name { get; set; }

        public int? GameId { get; set; }

        public int? OriginalGameId { get; set; }

        public ICollection<GameCard> GameCards { get; set; }
    }
}
