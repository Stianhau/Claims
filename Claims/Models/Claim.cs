using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Claims
{
    public class Claim
    {
        [JsonProperty(PropertyName = "id")]
        public required string Id { get; set; }
        
        [JsonProperty(PropertyName = "coverId")]
        public required string CoverId { get; set; }

        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }

        [JsonProperty(PropertyName = "name")]
        public required string Name { get; set; }

        [JsonProperty(PropertyName = "claimType")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public ClaimType Type { get; set; }

        [JsonProperty(PropertyName = "damageCost")]
        public decimal DamageCost { get; set; }

    }


    public enum ClaimType
    {
        // [EnumMember(Value = "Collision")] 
        Collision = 0,
        Grounding = 1,
        BadWeather = 2,
        // [EnumMember(Value = "Fire")] 
        Fire = 3
    }
}
