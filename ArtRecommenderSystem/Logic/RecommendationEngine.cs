using System;
using System.Collections.Generic;
using System.Linq;
using ArtRecommenderSystem.Database;

namespace ArtRecommenderSystem.Logic
{
    public static class RecommendationEngine
    {
        public class CollaborativeFiltering
        {
            public struct Recommendations
            {
                public List<string> UserInterests { get; }
                public List<int> ArtIdList { get; }

                public Recommendations(List<string> userInterests,
                    List<int> artIdList)
                {
                    UserInterests = userInterests;
                    ArtIdList = artIdList;
                }
            }

            public Recommendations GetRecommendations()
            {
                // Получаем предпочтения текущего пользователя.
                var currentFavorites =
                    ApplicationContext.GetInstance().GetFavorites();

                var users = ApplicationContext.GetInstance()
                    .GetUsersExceptCurrent();

                // Получаем предпочтения других пользователей и
                // находим число совпадений в предпочтениях с текущим.
                var userScores = new Dictionary<int, int>();
                foreach (var user in users)
                {
                    var favorites = ApplicationContext.GetInstance()
                        .GetUserFavorites(user.Id);
                    var score = 0;
                    foreach (var currentFavorite in currentFavorites)
                    {
                        if (favorites.Any(favorite =>
                            favorite.ArtId == currentFavorite.ArtId))
                        {
                            score++;
                        }
                    }

                    if (score > 0) userScores.Add(user.Id, score);
                }

                // Выбираем пользователей с похожими вкусами, чьи предпочтения будем рекумендовать.
                // В данном случае, берём пользователей с максимальным числом совпадений.
                var maxUserScores =
                    userScores.Where(userScore =>
                        userScore.Value ==
                        userScores.Max(score => score.Value));

                // Получаем идентификаторы видов искусств в предпочтениях похожих пользователей.
                var recommendedArtIds = new List<int>();
                var interests = new List<string>();
                foreach (var userScore in maxUserScores)
                {
                    recommendedArtIds.AddRange(ApplicationContext.GetInstance()
                        .GetUserFavorites(userScore.Key)
                        .Select(favorite => favorite.ArtId));
                    interests.AddRange(users
                        .Find(user => user.Id == userScore.Key)
                        .Interests.Split(new[] {", "},
                            StringSplitOptions.RemoveEmptyEntries));
                }

                recommendedArtIds = recommendedArtIds.Distinct().ToList();
                interests = interests.Distinct().ToList();

                // Фильтруем список искусств, оставляя только те, которые
                // текущий пользователь ещё не занёс к себе в предпочтения.
                var recommendationIdList = new List<int>();
                foreach (var artId in recommendedArtIds)
                {
                    if (currentFavorites.All(
                        favorite => favorite.ArtId != artId))
                    {
                        recommendationIdList.Add(artId);
                    }
                }

                return new Recommendations(interests, recommendationIdList);
            }
        }
    }
}
