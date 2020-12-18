namespace ArtRecommenderSystem.Logic
{
    public struct ValuePair
    {
        public double First { get; }
        public double Second { get; }
        public double MaxDifference { get; }

        public ValuePair(double first, double second, double maxDifference)
        {
            First = first;
            Second = second;
            MaxDifference = maxDifference;
        }
    }
}
