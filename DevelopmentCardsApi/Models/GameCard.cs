using System;
using System.Text.Json.Serialization;

namespace DevelopmentCardsApi.Models
{
    public class GameCard
    {
        public GameCard()
        {
            CreatedAt = DateTime.Now;
        }

        public int Id { get; set; }

        public int GameId { get; set; }

        [JsonIgnore]
        public Game Game { get; set; }

        public int DevelopmentCardId { get; set; }

        public DevelopmentCard DevelopmentCard { get; set; }

        public int? PlayerId { get; set; }

        [JsonIgnore]
        public Player Player { get; set; }

        public bool Played { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime PulledAt { get; set; }

        public DateTime PlayedAt { get; set; }
    }
}
