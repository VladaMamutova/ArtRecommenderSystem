using System;
using System.Collections.Generic;
using ArtRecommenderSystem.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArtRecommenderSystem.Logic
{
    class ArtNodeConverter : JsonConverter
    {
        private static ArtItem CreateItem(JToken obj)
        {
            var type = (string)obj["type"];

            switch (type)
            {
                case "leaf": return new ArtLeaf();
                case "node": return new ArtNode();
                default: throw new NotSupportedException();
            }
        }
        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var array = JArray.Load(reader);
            var target = new List<ArtItem>();

            if (array.HasValues)
            {
                foreach (var child in array.Children())
                {
                    var item = CreateItem(child);
                    serializer.Populate(child.CreateReader(), item);
                    target.Add(item);
                }
            }

            return target;
        }

        // Определяет, для каких типов должен использоваться конвертер.
        // В данном случае он всегда возвращает false, так как мы будем
        // применять конвертер к конкретным полям json с помощью атрибутов.
        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
