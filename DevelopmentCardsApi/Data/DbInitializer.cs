using System.Collections.Generic;
using System.Linq;
using DevelopmentCardsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DevelopmentCardsApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(GameContext context)
        {
            context.Database.Migrate();

            if (context.DevelopmentCards.Any())
            {
                return;
            }

            var seedDevelopmentCards = new List<DevelopmentCard>();

            for (var i = 0; i < 14; i++)
            {
                seedDevelopmentCards.Add(new DevelopmentCard(CardType.Knight));
            }

            for (var i = 0; i < 5; i++)
            {
                seedDevelopmentCards.Add(new DevelopmentCard(CardType.VictoryPoint));
            }

            for (var i = 0; i < 2; i++)
            {
                seedDevelopmentCards.Add(new DevelopmentCard(CardType.Monopoly));
            }

            for (var i = 0; i < 2; i++)
            {
                seedDevelopmentCards.Add(new DevelopmentCard(CardType.RoadBuilding));
            }

            for (var i = 0; i < 2; i++)
            {
                seedDevelopmentCards.Add(new DevelopmentCard(CardType.YearOfPlenty));
            }

            context.DevelopmentCards.AddRange(seedDevelopmentCards);
            context.SaveChanges();
        }
    }
}
