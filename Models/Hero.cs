using HeroDex.Models.Enums;
using System.Text.Json.Serialization;

namespace HeroDex.Models
{
    public class Hero
    {

        [JsonPropertyName("id")]
        public string Id{ get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("attributes")]
        public Attributes Attributes { get; set; }

        [JsonPropertyName("abilities")]
        public List<Ability> Abilities { get; set; }

        [JsonPropertyName("type")]
        [JsonConverter(typeof(HeroTypeConverter))]
        public HeroType HeroType { get; set; }
    }
}
