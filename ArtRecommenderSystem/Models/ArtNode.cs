using System.Collections.Generic;
using ArtRecommenderSystem.Logic;
using Newtonsoft.Json;

namespace ArtRecommenderSystem.Models
{
    internal class ArtNode: ArtItem
    {
        [JsonConverter(typeof(ArtNodeConverter))]
        public List<ArtItem> NestedObjects { get; set; }

        public IEnumerable<ArtLeaf> GetAllLeaves()
        {
            var leaves = new List<ArtLeaf>();
            foreach (var nestedObject in NestedObjects)
            {
                if (nestedObject is ArtNode artNode)
                {
                    leaves.AddRange(artNode.GetAllLeaves());
                }
                else
                {
                    leaves.Add(nestedObject as ArtLeaf);
                }
            }

            return leaves;
        }

        public void InitParents(string[] parents)
        {
            foreach (var nestedObject in NestedObjects)
            {
                nestedObject.Parents = parents;
                if (nestedObject is ArtNode artNode)
                {
                    var childParents = new List<string>();
                    childParents.AddRange(parents);
                    childParents.Add(nestedObject.Name);
                    artNode.InitParents(childParents.ToArray());
                }
            }
        }
    }
}
