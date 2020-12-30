using System;
using System.Collections.Generic;
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

            public bool IsEmpty() => Topic == Topics.Undefined && Art == null &&
                                     AdditionalArt == null;

            public void Clear()
            {
                Topic = Topics.Undefined;
                Art = null;
                AdditionalArt = null;
            }
        }

        private readonly MorphAnalyzer _morph;
        
        private readonly string[] _delimiters =
            {" ", ",", ".", ":", ";", "?", "!", " - "};

        private readonly string[] _dateKeywords =
            {"когда", "год", "век", "впервые", "появиться", "упоминание"};
        private readonly string[] _museumKeywords = {"музей", "место"};

        private readonly string[] _masterClassesKeywords =
            {"мастер-класс", "проводить", "научиться", "учиться"};

        private readonly string[] _popularityKeywords =
            {"популярность", "популярный", "сейчас", "пора", "распространить"};

        private readonly string[] _genresKeywords = {"жанр"};

        private QuestionInfo _context;

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

        private string BuildDateAnswer(ArtLeaf art, bool showParents = false)
        {
            string name = ChangeArtNameCase(art.Name, "рд");
            if (showParents)
            {
                name += " (" + string.Join(" / ", art.Parents) + ")";
            }

            return "Появление " + name + " относят к " +
                   art.Date.ConvertDateToString();
        }

        private string BuildMuseumNumberAnswer(ArtLeaf art, bool showParents = false)
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

        private string BuildMasterClassesAnswer(ArtLeaf art, bool showParents = false)
        {
            string name = ChangeArtNameCase(art.Name, "дт");
            if (showParents)
            {
                name += " (" + string.Join(" / ", art.Parents) + ")";
            }
            return art.AreMasterClassesHeld
                ? "Да, по " + name + " проводятся мастер-классы"
                : "Нет, по " + name + " не проводятся мастер-классы";
        }

        private string BuildPopularityAnswer(ArtLeaf art, bool showParents = false)
        {
            string name = art.Name;
            if (showParents)
            {
                name += " (" + string.Join(" / ", art.Parents) + ")";
            }

            switch (art.Popularity)
            {
                case PopularityEnum.None:
                {
                    var nameParts = _morph.Parse(name.Split(' ')).ToArray();
                    var verb = VerbToShortParticiple("распространить",
                        nameParts[0].BestTag["род"]);
                        return name + " уже не " + verb;
                }
                case PopularityEnum.Seldom:
                    return name + " - редкий вид искусства в наше время";
                case PopularityEnum.Common:
                    return name + " - достаточно распространённый вид искусства";
                case PopularityEnum.Widely:
                    return name + " - широко распространённый вид искусства";
                default:
                    return
                        "Не могу определить, насколько популярно искусство " +
                        name;
            }
        }

        public string BuildGenresAnswer(ArtLeaf art, bool showParents = false)
        {
            string name = ChangeArtNameCase(art.Name, "дт");
            if (showParents)
            {
                name += " (" + string.Join(" / ", art.Parents) + ")";
            }

            return "Работы по " + name +
                   " представлены в следующих жанрах: " +
                   string.Join(", ",
                       art.Genres.Select(genre =>
                           genre.GetAttrValueByEnumValue()));
        }
        
        private string ChangeArtNameCase(string artName, string wordCase)
        {
            var nameParts =
                _morph.Parse(artName.Split(' ')).ToArray();
            var resultName = new string[nameParts.Length];
            bool nounHandled = false;
            string gender = null;
            for (var i = 0; i < nameParts.Length; i++)
            {
                // Все слова будут того же рода, что и первое.
                // Таким образом, избежим ошибок при разборе существительных,
                // идущих после прилагательных ("монументальная графика",
                // "мелкая пластика": здесь сущ. как слова мужского рода
                // в винительном падеже). По прилагательному всегда
                // правильно определим род многозначных существительных.
                if (i == 0)
                {
                    gender = nameParts[i].BestTag["род"];
                }

                switch (nameParts[i].BestTag["чр"])
                {
                    // Если в наименовании более одного существительного
                    // ("резьба по дереву", "дизайн итерьера"), будем менять
                    // падеж только первого, второе уже согласовано.
                    case "сущ" when !nounHandled:
                        resultName[i] = ChangeWordCase(nameParts[i], wordCase, gender);
                        nounHandled = true;
                        break;
                    case "прил":
                        resultName[i] = ChangeWordCase(nameParts[i], wordCase, gender);
                        break;
                    default:
                        resultName[i] = nameParts[i].Text;
                        break;
                }
            }

            return string.Join(" ", resultName);
        }

        private string ChangeWordCase(MorphInfo wordInfo, string wordCase, string originGender = null)
        {
            string gender = originGender ?? wordInfo.BestTag["род"];

            var result = _morph.Inflect(new[]
            {
                new InflectTask(
                    wordInfo.Text,
                    _morph.TagHelper.CreateTag(wordInfo.BestTag["чр"],
                        gender, wordInfo.BestTag["число"], "им"),
                    _morph.TagHelper.CreateTag(wordInfo.BestTag["чр"],
                        gender, wordInfo.BestTag["число"], wordCase))
            }).ToArray();
            return result.Length == 0 ? wordInfo.Text : result[0];
        }

        private string VerbToShortParticiple(string verb, string gender)
        {
            var result = _morph.Inflect(new[]
            {
                new InflectTask(verb,
                    _morph.TagHelper.CreateTag("инф_гл"),
                    _morph.TagHelper.CreateTag("кр_прич",
                        gndr: gender, nmbr: "ед", tens: "прош", voic: "страд"))
            }).ToArray();
            return result.Length == 0 ? verb : result[0];
        }

        private string ChangeGenderToFemale(MorphInfo wordInfo)
        {
            var result = _morph.Inflect(new[]
            {
                new InflectTask(
                    wordInfo.BestTag.Lemma,
                    _morph.TagHelper.CreateTag(wordInfo.BestTag["чр"],
                        "муж", wordInfo.BestTag["число"],
                        wordInfo.BestTag["падеж"]),
                    _morph.TagHelper.CreateTag(wordInfo.BestTag["чр"], "жен",
                        wordInfo.BestTag["число"], "им"))
            }).ToArray();
            return result.Length == 0 ? wordInfo.Text : result[0];
        }

        private bool TryDefineTopic(string keyword, ref Topics topic)
        {
            if (_dateKeywords.Contains(keyword))
            {
                topic = Topics.Date;
            }

            if (_museumKeywords.Contains(keyword))
            {
                topic = Topics.MuseumNumber;
            }

            if (_masterClassesKeywords.Contains(keyword))
            {
                topic = Topics.MasterClasses;
            }

            if (_popularityKeywords.Contains(keyword))
            {
                topic = Topics.Popularity;
            }

            if (_genresKeywords.Contains(keyword))
            {
                topic = Topics.Genres;
            }

            return topic != Topics.Undefined;
        }

        private List<ArtLeaf> FindArtsByOneWord(List<ArtLeaf> arts,
            MorphInfo word)
        {
            if (word.BestTag.Has("сущ"))
            {
                return arts.FindAll(art =>
                    art.Name.ToLower().Contains(word.BestTag.Lemma));
            }

            return new List<ArtLeaf>();
        }

        private ArtLeaf FindArtByTwoWords(IEnumerable<ArtLeaf> arts,
            MorphInfo first, MorphInfo second)
        {
            // Наименования искусств, состоящие из двух слов, содержат
            // прилагательное и существительное: "декоративная живопись",
            // "монументальная графика" и др.
            ArtLeaf result = null;
            if (first.BestTag.Has("прил"))
            {
                var adj = ChangeGenderToFemale(first);
                var noun =
                    second.Tags.FirstOrDefault(tag => tag.Has("сущ", "жен"));
                if (noun != null)
                {
                    result = arts.FirstOrDefault(art =>
                        art.Name.ToLower() == adj + " " + noun.Lemma);
                }
            }

            return result;
        }

        private ArtLeaf FindArtByThreeWords(IEnumerable<ArtLeaf> arts,
            MorphInfo first, MorphInfo second, MorphInfo third)
        {
            // Наименования искусств, состоящие из трёх слов:
            // "резьба по дереву", "резьба по кости", "резьба по камню".
            var artName = first.BestTag.Lemma + " " + second.Text + " " +
                          third.Text;
            return arts.FirstOrDefault(art => art.Name.ToLower() == artName);
        }

        private QuestionInfo HandleQuestion(string question)
        {
            // Разбиваем вопрос на слова и проводим морфологический разбор слов.
            QuestionInfo questionInfo = new QuestionInfo();
            var words = question.Split(_delimiters,
                StringSplitOptions.RemoveEmptyEntries);
            var wordsInfo = _morph.Parse(words).ToArray();

            // По каждому слову пытаемся определить тематику вопроса
            // или наименование искусства, о котором был задан вопрос.
            for (var i = 0; i < wordsInfo.Length; i++)
            {
                bool wordHandled = false;
                string lemma = wordsInfo[i].BestTag.Lemma; // нормальная форма слова

                // Определяем тематику.
                if (questionInfo.Topic == Topics.Undefined)
                {
                    wordHandled = TryDefineTopic(lemma, ref questionInfo.Topic);
                }

                // Определяем вид искусства.
                if (!wordHandled && questionInfo.Art == null)
                {
                    // Находим все искусства, содержащие в имени текущее слово.
                    var arts =
                        FindArtsByOneWord(ApplicationContext.GetInstance().Arts,
                            wordsInfo[i]);
                    if (arts.Count == 1) // найден один вид искусства
                    {
                        questionInfo.Art = arts[0];
                    }
                    else if (arts.Count > 1) // найдено несколько видов искусства
                    {
                        // Проверяем, представляет ли текущее слово полное
                        // название вида искуства или только его часть.
                        if (arts.All(art => art.Name.ToLower() == lemma))
                        {
                            // Если текущее слово является полным названием
                            // всех найденных искусств (темпера как вид
                            // монументальной и вид станковой живописи, витраж
                            // как вид монументальной живописи и вид декоративно-
                            // прикладного искусства), то сохраняем оба искусства.
                            questionInfo.Art = arts[0];
                            questionInfo.AdditionalArt = arts[1];
                        }
                        else // текущее слово составляет часть названия
                             // ("живопись", "графика", "резьба")
                        {
                            if (i > 0) // если есть предудущее слово
                            {
                                // Пытаемся найти название искусства по двум словам:
                                // прилагательному и существительному
                                // ("миниатюрная живопись", "декоративная живопись").
                                questionInfo.Art = FindArtByTwoWords(arts,
                                    wordsInfo[i - 1], wordsInfo[i]);
                            }
                            
                            if (i < wordsInfo.Length - 2 && questionInfo.Art == null)
                                // если за текущим словом идут ещё два
                            {
                                // Пытаемся найти название искусства по трём словам
                                // ("резьба по дереву", "резьба по кости", "резьба по камню").
                                questionInfo.Art = FindArtByThreeWords(arts,
                                    wordsInfo[i], wordsInfo[i + 1],
                                    wordsInfo[i + 2]);
                            }
                        }
                    }
                }
            }

            return questionInfo;
        }

        private string GetAnswer(QuestionInfo questionInfo)
        {
            string answer = "";
            if (questionInfo.Topic == Topics.Undefined)
            {
                answer = "Хм, этого не могу подсказать. " +
                         "Попробуй перефразировать, пожалуйста";
            }
            else if (questionInfo.Art == null)
            {
                answer = "О каком виде искусства ты говоришь?";
            }
            else
            {
                bool showParents = false;
                var arts = new List<ArtLeaf> { questionInfo.Art };
                if (questionInfo.AdditionalArt != null)
                {
                    arts.Add(questionInfo.AdditionalArt);
                    showParents = true;
                }

                foreach (var art in arts)
                {
                    if (answer != "")
                    {
                        answer += "\n";
                    }

                    switch (questionInfo.Topic)
                    {
                        case Topics.Date:
                            answer += BuildDateAnswer(art, showParents);
                            break;
                        case Topics.MuseumNumber:
                            answer += BuildMuseumNumberAnswer(art, showParents);
                            break;
                        case Topics.MasterClasses:
                            answer += BuildMasterClassesAnswer(art, showParents);
                            break;
                        case Topics.Popularity:
                            answer += BuildPopularityAnswer(art, showParents);
                            break;
                        case Topics.Genres:
                            answer += BuildGenresAnswer(art, showParents);
                            break;
                    }
                }
            }

            return answer;
        }

        public string Answer(string question)
        {
            string answer;
            var questionInfo = HandleQuestion(question);
            if (questionInfo.IsEmpty())
            {
                _context.Clear();
                answer = "Хм, этого не могу подсказать. " +
                         "Попробуй перефразировать, пожалуйста";
            }
            else
            {
                if (questionInfo.Topic == Topics.Undefined)
                {
                    questionInfo.Topic = _context.Topic;
                }
                else if (questionInfo.Art == null)
                {
                    if (_context.Art != null)
                    {
                        questionInfo.Art = _context.Art;
                        questionInfo.AdditionalArt = _context.AdditionalArt;
                    }
                }

                _context = questionInfo;
                answer = GetAnswer(questionInfo);
            }

            return answer;
        }
    }
}
