
using System.Collections.Generic;

namespace ArtRecommenderSystem.Logic
{
    public interface IRecommendationEngine
    {
        List<int> Recommend();
    }
}
