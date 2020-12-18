using System;
using System.Linq;

namespace ArtRecommenderSystem.Logic
{
    public static class SimilarityMeasureCalculator
    {
        public static double CalcEuclideanDistance(ValuePair[] valuePairs)
        {
            double result = 0;
            foreach (var pair in valuePairs)
            {
                result += Math.Pow((pair.First - pair.Second) / pair.MaxDifference, 2);
            }

            result = Math.Sqrt(result / valuePairs.Length);
            return result;
        }

        public static double CalcManhattanDistance(ValuePair[] valuePairs)
        {
            double result = 0;
            foreach (var pair in valuePairs)
            {
                result += Math.Abs(pair.First - pair.Second) / pair.MaxDifference;
            }

            result = Math.Sqrt(result / valuePairs.Length);
            return result;
        }

        public static int CalcTreeProximity(string[] parentList1,
            string[] parentList2)
        {
            if (parentList1 == null)
            {
                throw new ArgumentNullException(nameof(parentList1));
            }

            if (parentList2 == null)
            {
                throw new ArgumentNullException(nameof(parentList2));
            }

            var result = parentList1.Length + parentList2.Length;
            var commonParents = 0;
            var commonDepth = Math.Min(parentList1.Length, parentList2.Length);
            while (commonParents < commonDepth
                   && parentList1[commonParents] == parentList2[commonParents])
            {
                commonParents++;
            }

            result -= commonParents * 2;
            result++;

            return result;
        }

        public static double CalcCorrelation(ValuePair[] valuePairs)
        {
            double firstMedium = valuePairs.Sum(pair => pair.First) / valuePairs.Length;
            double secondMedium = valuePairs.Sum(pair => pair.Second) / valuePairs.Length;

            double nominator = 0;
            double sumFirst = 0;
            double sumSecond = 0;

            foreach (var pair in valuePairs)
            {
                nominator += (pair.First - firstMedium) *
                             (pair.Second - secondMedium);

                sumFirst += Math.Pow(pair.First - firstMedium, 2);
                sumSecond += Math.Pow(pair.Second - secondMedium, 2);
            }

            double denominator = Math.Sqrt(sumFirst * sumSecond);
            return nominator / denominator;
        }
    }
}
