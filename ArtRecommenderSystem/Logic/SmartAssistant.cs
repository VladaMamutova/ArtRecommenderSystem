using System;
using System.Linq;
using ArtRecommenderSystem.Database;
using ArtRecommenderSystem.Models;
using ArtRecommenderSystem.Utilities;
using DeepMorphy;
using DeepMorphy.Model;

namespace ArtRecommenderSystem.Logic
{
    class SmartAssistant
    {
        private enum Topics
        {
            Undefined,
            Date,
            MuseumNumber,
            MasterClasses,
            Popularity,
            Genres
        }

        private struct QuestionInfo
        {
            public Topics Topic;
            public ArtLeaf Art;
            public ArtLeaf AdditionalArt;
        }

        private readonly MorphAnalyzer _morph;
        
        private readonly string[] _delimiters =
            {" ", ",", ".", ":", ";", "?", "!", " - "};

        private string[] DateKeywords =
            {"когда", "год", "век", "впервые", "появиться"};
        private string[] MuseumKeywords = {"музей", "место"};

        private string[] MasterClassesKeywords =
            {"мастер-класс", "научиться", "учиться"};

        private string[] PopularityKeywords =
            {"популярность", "популярный", "сейчас", "пора", "распространить"};

        private string[] GenresKeywords =
            {"жанр"};

        private ArtLeaf _lastArt;
        private ArtLeaf _lastAdditionalArt;

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

        private bool TryDefineTopic(string keyword, ref Topics topic)
        {
            if (DateKeywords.Contains(keyword))
            {
                topic = Topics.Date;
            }

            if (MuseumKeywords.Contains(keyword))
            {
                topic = Topics.MuseumNumber;
            }

            if (MasterClassesKeywords.Contains(keyword))
            {
                topic = Topics.MasterClasses;
            }

            if (PopularityKeywords.Contains(keyword))
            {
                topic = Topics.Popularity;
            }

            if (GenresKeywords.Contains(keyword))
            {
                topic = Topics.Genres;
            }

            return topic != Topics.Undefined;
        }

        private string GetAnswer(QuestionInfo questionInfo, bool showArtParents = false)
        {
            string answer;
            bool showParents = showArtParents || questionInfo.AdditionalArt != null;
            var art = questionInfo.Art;
            switch (questionInfo.Topic)
            {
                case Topics.Date:
                    answer = BuildDateAnswer(art, showParents);
                    break;
                case Topics.MuseumNumber:
                    answer = BuildMuseumNumberAnswer(art, showParents);
                    break;
                case Topics.MasterClasses:
                    answer = BuildMasterClassesAnswer(art, showParents);
                    break;
                case Topics.Popularity:
                    answer = BuildPopularityAnswer(art, showParents);
                    break;
                case Topics.Genres:
                    answer = BuildGenresAnswer(art, showParents);
                    break;
                default:
                    answer = "Про какой вид искусства ты говоришь?";
                    break;
            }

            if (questionInfo.AdditionalArt != null)
            {
                answer += "\n" + GetAnswer(new QuestionInfo
                {
                    Topic = questionInfo.Topic,
                    Art = questionInfo.AdditionalArt
                }, true);
            }

            return answer;
        }

        public string BuildDateAnswer(ArtLeaf art, bool showParents = false)
        {
            string name = art.Name;
            if (showParents)
            {
                name += " (" + string.Join(" / ", art.Parents) + ")";
            }
            return "Первые работы по искусству " + name + " относят к " +
                art.Date.ConvertDateToString();
        }

        public string BuildMuseumNumberAnswer(ArtLeaf art, bool showParents = false)
        {
            string name = art.Name;
            if (showParents)
            {
                name += " (" + string.Join(" / ", art.Parents) + ")";
            }
            switch (art.MuseumNumber)
            {
                case 0: return name + " не выставляется в музеях";
                case 1: return name + " выставляется только в 1 музее";
                default:
                    return name +
                           $" выставляется в {art.MuseumNumber} музеях";
            }
        }

        public string BuildMasterClassesAnswer(ArtLeaf art, bool showParents = false)
        {
            string name = art.Name;
            if (showParents)
            {
                name += " (" + string.Join(" / ", art.Parents) + ")";
            }
            return art.AreMasterClassesHeld
                ? "Да, по искусству " + name + " проводятся мастер-классы"
                : "Нет, по искусству " + name + " не проводятся мастер-классы";
        }

        public string BuildPopularityAnswer(ArtLeaf art, bool showParents = false)
        {
            string name = art.Name;
            if (showParents)
            {
                name += " (" + string.Join(" / ", art.Parents) + ")";
            }
            switch (art.Popularity)
            {
                case PopularityEnum.None:
                    return "Искусство " + name + " уже не распространено";
                case PopularityEnum.Seldom:
                    return name + " - достаточно редкий вид искусства в наше время";
                case PopularityEnum.Common:
                    return "Искусство " + name + " в достаточной мере распространено";
                case PopularityEnum.Widely:
                    return name + " - один из самых популярных видов искусства";
                default:
                    return
                        "Не могу определить, насколько популярно искусство " +
                        name;
            }
        }

        public string BuildGenresAnswer(ArtLeaf art, bool showParents = false)
        {
            string name = art.Name;
            if (showParents)
            {
                name += " (" + string.Join(" / ", art.Parents) + ")";
            }

            return "Работы по искусству " + name +
                   " представлены в следующих жанрах: " +
                   string.Join(", ",
                       art.Genres.Select(genre =>
                           genre.GetAttrValueByEnumValue()));
        }


        //private string ArtNameToSpecialCase(string artName, string wordCase)
        //{
        //    var artNameParts =
        //        _morph.Parse(artName.Split(' ')).ToArray();
        //    var resultArtName = "";
        //    bool nounHandled = false;
        //    foreach (var part in artNameParts)
        //    {
        //        if (part["чр"].BestGramKey != "предл")
        //        {
        //            switch (part["чр"].BestGramKey)
        //            {
        //                case "сущ" when !nounHandled:
        //                    resultArtName += " " + WordToSpecialCase(part, wordCase);
        //                    nounHandled = true;
        //                    break;
        //                case "прил":
        //                    resultArtName += " " + WordToSpecialCase(part, wordCase);
        //                    break;
        //            }
        //        }
        //    }

        //    return resultArtName.TrimStart(' ');
        //}

        //private string WordToSpecialCase(MorphInfo wordInfo, string wordCase)
        //{
        //    var result = _morph.Inflect(new[]
        //    {
        //        new InflectTask(
        //            wordInfo.BestTag.Lemma,
        //            _morph.TagHelper.CreateTag(wordInfo["чр"].BestGramKey,
        //                wordInfo["род"].BestGramKey,
        //                wordInfo["число"].BestGramKey,
        //                wordInfo["падеж"].BestGramKey),
        //            _morph.TagHelper.CreateTag(wordInfo["чр"].BestGramKey,
        //                wordInfo["род"].BestGramKey,
        //                wordInfo["число"].BestGramKey,
        //                wordCase))
        //    }).ToArray();
        //    return result.Length == 0 ? wordInfo.Text : result[0];
        //}

        private QuestionInfo HandleQuestion(string question)
        {
            QuestionInfo questionInfo = new QuestionInfo();
            var words = question.Split(_delimiters,
                StringSplitOptions.RemoveEmptyEntries);
            var wordsInfo = _morph.Parse(words).ToArray();
            for (var i = 0; i < wordsInfo.Length; i++)
            {
                var word = wordsInfo[i];
                bool wordHandled = false;
                string lemma = word.BestTag.Lemma;
                if (questionInfo.Topic == Topics.Undefined)
                {
                    wordHandled = TryDefineTopic(lemma, ref questionInfo.Topic);
                }

                if (!wordHandled && questionInfo.Art == null)
                {
                    var possibleArts = ApplicationContext.GetInstance().Arts
                        .FindAll(art =>
                            art.Name.ToLower().StartsWith(lemma) ||
                            art.Name.ToLower().EndsWith(lemma));
                    if (possibleArts.Count == 1)
                    {
                        questionInfo.Art = possibleArts[0];
                    }
                    else if (possibleArts.Count > 1)
                    {
                        bool singleWord = possibleArts.All(art =>
                            art.Name.ToLower() == lemma);
                        if (singleWord)
                        {
                            questionInfo.Art = possibleArts[0];
                            questionInfo.AdditionalArt = possibleArts[1];
                        }
                        else
                        {
                            if (i > 0 && wordsInfo[i - 1].BestTag.Has("прил"))
                            {
                                var adj =
                                    GetFeminineAdjective(wordsInfo[i - 1]);
                                var possibleArt = ApplicationContext
                                    .GetInstance().Arts
                                    .FirstOrDefault(art =>
                                        art.Name.ToLower()
                                            .Contains(adj + " " + lemma));
                                if (possibleArt != null)
                                {
                                    questionInfo.Art = possibleArt;
                                }

                            }
                            else if (i < wordsInfo.Length - 1)
                            {
                                var artName =
                                    wordsInfo[i].BestTag.Lemma + " " +
                                    wordsInfo[i + 1].Text + " " +
                                    wordsInfo[i + 2].Text;
                                var possibleArt =
                                    possibleArts.FirstOrDefault(art =>
                                        art.Name.ToLower() == artName);
                                if (possibleArt != null)
                                {
                                    questionInfo.Art = possibleArt;
                                }
                            }
                        }
                    }
                }
            }

            if (questionInfo.Art == null)
            {
                if (_lastArt != null)
                {
                    questionInfo.Art = _lastArt;
                    questionInfo.AdditionalArt = _lastAdditionalArt;
                }
            }

            _lastArt = questionInfo.Art;
            _lastAdditionalArt = questionInfo.AdditionalArt;
            return questionInfo;
        }

        private string GetFeminineAdjective(MorphInfo wordInfo)
        {
            var result = _morph.Inflect(new[]
            {
                new InflectTask(
                    wordInfo.BestTag.Lemma,
                    _morph.TagHelper.CreateTag("прил", "муж", "ед", "им"),
                    _morph.TagHelper.CreateTag("прил", "жен", "ед","им"))
            }).ToArray();
            return result.Length == 0 ? wordInfo.Text : result[0];
        }

        public string Answer(string question)
        {
            string answer;
            
            var questionInfo = HandleQuestion(question);
            if (questionInfo.Topic == Topics.Undefined)
            {
                answer = "Хм, этого не могу подсказать. Попробуй перефразировать, пожалуйста";
            }
            else if (questionInfo.Art == null)
            {
                answer = "Про какой вид искусства ты говоришь?";
            }
            else
            {
                answer = GetAnswer(questionInfo);
            }

            return answer;
        }
    }
}
