using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ArtRecommenderSystem.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Genres
    {
        [EnumMember(Value = "бытовой")] Household = 0,
        [EnumMember(Value = "портрет")] Portrait = 1,
        [EnumMember(Value = "пейзаж")] Landscape = 2,
        [EnumMember(Value = "исторический")] Historical = 3,
        [EnumMember(Value = "мифологический")] Mythological = 4,
        [EnumMember(Value = "религиозный")] Religious = 5,
        [EnumMember(Value = "батальный")] Battle = 6,
        [EnumMember(Value = "натюрморт")] StillLife = 7,
        [EnumMember(Value = "марина")] Marina = 8,
        [EnumMember(Value = "анималистический")] Animalistic = 9,
        [EnumMember(Value = "интерьер")] Interior = 10,
        [EnumMember(Value = "архитектура")] Architecture = 11,
        [EnumMember(Value = "декоративный")] Decorative = 12
    }
}
