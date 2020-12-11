using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ArtRecommenderSystem.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PopularityEnum
    {
        [EnumMember(Value = "не распространен")] None = 0,
        [EnumMember(Value = "мало распространен")] Seldom = 1,
        [EnumMember(Value = "распространен")] Common = 2,
        [EnumMember(Value = "широко распространен")] Widely = 3
    }
}