using System.Text.Json.Serialization;

namespace DevelopmentCardsApi.Models
{
    public class DevelopmentCard
    {
        public DevelopmentCard()
        {
        }

        public DevelopmentCard(CardType cardType)
        {
            CardType = cardType;
        }

        public int Id { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardType CardType { get; set; }
    }

    public enum CardType
    {
        VictoryPoint,
        Knight,
        Monopoly,
        RoadBuilding,
        YearOfPlenty
    }
}
