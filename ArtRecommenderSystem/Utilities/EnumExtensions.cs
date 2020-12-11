using System;
using System.Linq;
using System.Runtime.Serialization;

namespace ArtRecommenderSystem.Utilities
{
    public static class EnumExtensions
    {
        public static string GetAttrValueByEnumValue<T>(this T enumValue)
            where T : Enum
        {
            var enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            var memberInfo = enumType.GetMember(enumValue.ToString());
            var attribute = memberInfo.FirstOrDefault()
                ?.GetCustomAttributes(false)
                .OfType<EnumMemberAttribute>().FirstOrDefault();
            return attribute?.Value;
        }

        public static T GetEnumMemberByAttrValue<T>(this string attributeValue)
            where T : Enum
        {
            var enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            var enumValues = enumType.GetEnumValues();

            foreach (var enumValue in enumValues)
            {
                var memInfo = enumType.GetMember(enumValue.ToString());
                var attribute = memInfo.FirstOrDefault()
                    ?.GetCustomAttributes(false)
                    .OfType<EnumMemberAttribute>().FirstOrDefault();

                if (string.Equals(attribute?.Value, attributeValue,
                    StringComparison.CurrentCultureIgnoreCase))
                    return (T) enumValue;
            }

            throw new ArgumentException(
                "None of T values has attribute value.");
        }
    }
}