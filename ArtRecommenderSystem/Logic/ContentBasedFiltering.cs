using System.Collections.Generic;
using System.Linq;
using ArtRecommenderSystem.Database;
using ArtRecommenderSystem.Models;

namespace ArtRecommenderSystem.Logic
{
    public class ContentBasedFiltering: IRecommendationEngine
    {
        public List<int> Recommend()
        {
            // Получаем предпочтения текущего пользователя.
            var currentFavorites =
                ApplicationContext.GetInstance().GetFavorites();

            var arts = ApplicationContext.GetInstance().Arts;
            var favoriteArts = currentFavorites.Select(favorite =>
                arts.Find(art => art.Id == favorite.ArtId)).ToList();

            // Исключаем избранные виды искусства текущего пользователя.
            arts = arts.Where(art =>
                    currentFavorites.All(favorite => favorite.ArtId != art.Id))
                .ToList();
            
            // Создаём матрицу схожести видов искусства (строки - избранное
            // текущего пользователя, столбцы - все виды искусства, исключая избранное).
            var similarityMatrix =
                new double[favoriteArts.Count, arts.Count];
            for (var i = 0; i < similarityMatrix.GetLength(0); i++)
            {
                for (var j = 0; j < similarityMatrix.GetLength(1); j++)
                {
                    var pairs = BuildValuePairs(favoriteArts[i], arts[j]);

                    var euclideanDistance = SimilarityMeasureCalculator
                        .CalcEuclideanDistance(pairs);
                    double treeProximity = SimilarityMeasureCalculator.CalcTreeProximity(
                            favoriteArts[i].Parents, arts[j].Parents);

                    // Используем евклидово расстояние (составляет 70% в итоговом значении)
                    // и близость по дереву (30%) для получения итогового сходства видов искусства.
                    similarityMatrix[i, j] =
                        (1 - euclideanDistance) * 0.7 +
                        (1 - treeProximity / ArtHelper.MAX_TREE_PROXIMITY) * 0.3;
                }
            }
            
            // Для каждого вида искусства получаем общее сходство
            // со всеми избранными видами искусства
            // (key - id вида искусства, value - среднее арифметическое
            // всех значений схожести по столбцу).
            var artsSimilarity = new Dictionary<int, double>();
            for (var j = 0; j < similarityMatrix.GetLength(1); j++)
            {
                double similarity = 0;
                for (var i = 0; i < similarityMatrix.GetLength(0); i++)
                {
                    similarity += similarityMatrix[i, j];
                }

                similarity /= similarityMatrix.GetLength(0);

                artsSimilarity.Add(arts[j].Id, similarity);
            }

            // Отбираем те виды искусства, сходство которых больше 50%, и сортируем их по убыванию.
            var recommendedArtIds = artsSimilarity
                .Where(artSimilarity => artSimilarity.Value > 0.5)
                .OrderByDescending(artSimilarity =>
                    artSimilarity.Value)
                .Select(artSimilarity => artSimilarity.Key);

            return recommendedArtIds.ToList();
        }

        private ValuePair[] BuildValuePairs(ArtLeaf art1, ArtLeaf art2)
        {
            return new[]
            {
                    new ValuePair(
                        ArtHelper.GetPeriodNumber(art1.Date),
                        ArtHelper.GetPeriodNumber(art2.Date),
                        ArtHelper.MAX_PERIOD_DIFFERENCE),

                    new ValuePair(art1.MuseumNumber, art2.MuseumNumber,
                        ArtHelper.MAX_MUSEUM_NUMBER_DIFFERENCE),

                    new ValuePair(art1.AreMasterClassesHeld ? 1 : 0,
                        art2.AreMasterClassesHeld ? 1 : 0,
                        ArtHelper.MAX_MASTER_CLASSES_DIFFERENCE),

                    new ValuePair((double) art1.Popularity,
                        (double) art2.Popularity,
                        ArtHelper.MAX_POPULARITY_DIFFERENCE),

                    new ValuePair(1,
                        ArtHelper.GetGenresSimilarity(art1.Genres.ToArray(),
                            art2.Genres.ToArray()),
                        ArtHelper.MAX_GENRE_DIFFERENCE)
                };
        }
    }
}
