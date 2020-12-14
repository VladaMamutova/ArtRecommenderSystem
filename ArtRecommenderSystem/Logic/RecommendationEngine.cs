using System.Collections.Generic;
using System.Linq;
using ArtRecommenderSystem.Database;

namespace ArtRecommenderSystem.Logic
{
    public static class RecommendationEngine
    {
        public static List<int> CollaborativeFiltering()
        {
            // предпочтения текущего пользователя
            var currentFavorites =
                ApplicationContext.GetInstance().GetFavorites();

            var users = ApplicationContext.GetInstance().GetUsersExceptCurrent();
            var similarityScore = new Dictionary<int, int>();
            for (int i = 0; i < users.Count; i++)
            {
                // предпочтения другого пользователя
                var favorites = ApplicationContext.GetInstance()
                    .GetUserFavorites(users[i].Id);
                var score = 0;

                // Находим число совпадений в предпочтениях текущего и другого пользователей.
                foreach (var currentFavorite in currentFavorites)
                {
                    if (favorites.Any(favorite =>
                        favorite.ArtId == currentFavorite.ArtId))
                    {
                        score++;
                    }
                }

                if (score > 0) similarityScore.Add(i, score);
            }

            // Выбираем пользователей с похожими вкусами, чьи предпочтения будем рекумендовать.
            // В данном случае, берём пользователей с максимальным числом совпадений.
            var maxSimilarityScores =
                similarityScore.Where(score =>
                    score.Value == similarityScore.Max(s => s.Value));

            // Получаем идентификаторы видов искусств в предпочтениях похожих пользователей.
            var recommendedArtIds = new List<int>();
            foreach (var score in maxSimilarityScores)
            {
                recommendedArtIds.AddRange(ApplicationContext.GetInstance()
                    .GetUserFavorites(users[score.Key].Id)
                    .Select(favorite => favorite.ArtId));
            }
            recommendedArtIds = recommendedArtIds.Distinct().ToList();

            // Фильтруем список искусств, оставляя только те, которые
            // текущий пользователь ещё не занёс к себе в предпочтения.
            var recommendationIdList = new List<int>();
            foreach (var artId in recommendedArtIds)
            {
                if (currentFavorites.All(favorite => favorite.ArtId != artId))
                {
                    recommendationIdList.Add(artId);
                }
            }

            return recommendationIdList;
        }
    }
}
