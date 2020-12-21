namespace ArtRecommenderSystem.Logic
{
    public class Period
    {
        public string Name { get; }
        public int Start { get; }
        public int End { get; }

        public Period(string name, int start, int end)
        {
            Name = name;
            Start = start;
            End = end;
        }

        public int GetDuration()
        {
            return End - Start;
        }

        public override string ToString()
        {
            return $"{Name} ({Start.ConvertDateToString()} - " +
                   $"{End.ConvertDateToString()})";
        }
    }
}
