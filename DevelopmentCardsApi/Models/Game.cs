using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DevelopmentCardsApi.Models
{
    public class Game
    {
        public Game()
        {
            Players = new List<Player>();
            GameCards = new List<GameCard>();
        }

        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

        public ICollection<Player> Players { get; set; }

        public ICollection<GameCard> GameCards { get; set; }

        [NotMapped]
        public ICollection<GameCard> AvailableGameCards => GameCards.Where(gc => gc.Player == null).ToList();
    }
}
