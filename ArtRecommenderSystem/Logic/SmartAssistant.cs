using System;
using System.Linq;
using DeepMorphy;

namespace ArtRecommenderSystem.Logic
{
    class SmartAssistant
    {
        private readonly MorphAnalyzer _morph;
        private readonly string[] _delimiters = {" ", ",", ".", ":", ";", "?", "!", " - "};

        private SmartAssistant()
        {
            // При создании MorphAnalyzer требуется дополнительное время
            // на загрузку словарей и нейронной сети,
            // поэтому SmartAssistant реализован в виде Синглтона.

            // withLemmatization = true - для приведения слов к нормальной форме.
            _morph = new MorphAnalyzer(withLemmatization: true);
        }

        private static readonly Lazy<SmartAssistant> LazyInstance =
            new Lazy<SmartAssistant>(
                () => new SmartAssistant());

        public static SmartAssistant Instance => LazyInstance.Value;

        public string Answer(string question)
        {
            string answer = "";
            var words = question.Split(_delimiters,
                StringSplitOptions.RemoveEmptyEntries);

            var results = _morph.Parse(words).ToArray();
            answer = string.Join(" ", results.Select(result => result.BestTag.Lemma));

            return answer;
        }
    }
}
