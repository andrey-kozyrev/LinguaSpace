using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

using LinguaSpace.Common;
using LinguaSpace.Data;

namespace LinguaSpace.Practice
{
    [DebuggerDisplay("Text")]
	internal abstract class FlashCardItemBase : NotifyPropertyChangedImpl, IFlashCardItem, INotifyPropertyChanged
	{
		#region Fields

		protected IMeaning meaning;
		protected FlashCardItemStatus status = FlashCardItemStatus.Unknown;

		#endregion

		#region IFlashCardItem

		public FlashCardItemBase(IMeaning meaning)
		{
			Debug.Assert(meaning != null);
			this.meaning = meaning;
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public abstract FlashCardItemType Type
        {
            get;
        }

        [System.Reflection.Obfuscation(Exclude = true)]
		public abstract String Caption
		{
			get;
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public abstract String Text
		{
			get;
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public FlashCardItemStatus Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
				RaisePropertyChangedEvent("Status");
			}
		}

		#endregion

		#region Implementation

		protected String List2String<T>(IEnumerable<T> list)
		{
			StringBuilder sb = new StringBuilder();
			foreach (T item in list)
			{
				sb.Append(item.ToString());
				sb.Append(", ");
			}
			sb.Length -= 2;
			return sb.ToString();
		}

		#endregion

		#region Overrides

		public override bool Equals(Object obj)
		{
			bool equals = false;
			FlashCardItemBase other = obj as FlashCardItemBase;
			if (other != null)
			{
                equals = this.meaning.Equals(other.meaning);
			}
			else 
			{
                equals = base.Equals(obj);
			}
			return equals;
		}

		#endregion
	}

	internal class FlashCardItemWord : FlashCardItemBase
	{
		public FlashCardItemWord(IMeaning meaning)
			: base(meaning)
		{
			Debug.Assert(!StringUtils.IsEmpty(this.Text));
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public override FlashCardItemType Type
        {
            get
            {
                return FlashCardItemType.Word;
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public override String Caption
		{
			get 
			{
				return "Word";
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public override string Text
		{
			get 
			{
				String text = this.meaning.Usage;
				if (StringUtils.IsEmpty(text))
				{
					text = this.meaning.Word.Text;
				}
				return text;
			}
		}
	}

	internal class FlashCardItemDefinition : FlashCardItemBase
	{
		public FlashCardItemDefinition(IMeaning meaning)
			: base(meaning)
		{
			Debug.Assert(!StringUtils.IsEmpty(meaning.Definition));
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public override FlashCardItemType Type
        {
            get
            {
                return FlashCardItemType.Definition;
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
		public override string Caption
		{
			get
			{
				return "Definition";
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public override string Text
		{
			get
			{
				return this.meaning.Definition;
			}
		}
	}

	internal class FlashCardItemTranslations : FlashCardItemBase
	{
		public FlashCardItemTranslations(IMeaning meaning)
			: base(meaning)
		{
			Debug.Assert(meaning.Translations.Count > 0);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public override FlashCardItemType Type
        {
            get
            {
                return FlashCardItemType.Translations;
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
		public override string Caption
		{
			get
			{
				return "Translations";
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public override string Text
		{
			get
			{
				return List2String<String>(this.meaning.Translations);
			}
		}
	}

	internal class FlashCardItemSynonyms : FlashCardItemBase
	{
		public FlashCardItemSynonyms(IMeaning meaning)
			: base(meaning)
		{
			Debug.Assert(meaning.Synonyms.Count > 0);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public override FlashCardItemType Type
        {
            get
            {
                return FlashCardItemType.Synonyms;
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
		public override string Caption
		{
			get
			{
				return "Synonyms";
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public override string Text
		{
			get
			{
				return List2String<IWord>(this.meaning.Synonyms);
			}
		}
	}

	internal class FlashCardItemAntonyms : FlashCardItemBase
	{
		public FlashCardItemAntonyms(IMeaning meaning)
			: base(meaning)
		{
			Debug.Assert(meaning.Antonyms.Count > 0);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public override FlashCardItemType Type
        {
            get
            {
                return FlashCardItemType.Antonyms;
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
		public override string Caption
		{
			get
			{
				return "Antonyms";
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public override string Text
		{
			get
			{
				return List2String<IWord>(this.meaning.Antonyms);
			}
		}
	}

	internal class FlashCard : IFlashCard, INotifyPropertyChanged
	{
		#region Fields

		private FlashCardItemBase question;
		private ICollection<IFlashCardItem> answers;
		private FlashCardStatus status = FlashCardStatus.Question;
		private String example;
		private IStatistics statistics;

		#endregion

		#region IFlashCard

		public FlashCard(IFlashCardItem question, ICollection<IFlashCardItem> answers, String example, IStatistics statistics)
		{
			Debug.Assert(question != null);
			Debug.Assert(answers != null);
			Debug.Assert(answers.Count > 2);
			Debug.Assert(example != null);
			Debug.Assert(statistics != null);
			this.question = question as FlashCardItemBase;
			Debug.Assert(question != null);
			this.answers = answers;
			this.status = FlashCardStatus.Question;
			this.example = example;
			this.statistics = statistics;
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public IFlashCardItem Question
		{
			get
			{
                return this.question;
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public IEnumerable<IFlashCardItem> Answers
		{
			get
			{
				return this.answers;
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public String Example
		{
			get
			{
				return this.example;
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public FlashCardStatus Status
		{
			get
			{
				return this.status;
			}
		}

        public void Answer(IFlashCardItem answer)
		{
			this.status = FlashCardStatus.Right;
			foreach (FlashCardItemBase item in this.answers)
			{
				if (item.Equals(this.question))
				{
					item.Status = FlashCardItemStatus.Right;
					this.statistics.RightAnswer();
				}
				else if (item.Equals(answer))
				{
					item.Status = FlashCardItemStatus.Wrong;
					this.status = FlashCardStatus.Wrong;
					this.statistics.WrongAnswer();
				}
			}
			this.question.Status = FlashCardItemStatus.Right;
			RaisePropertyChangedEvent("Status");
		}

        public void Prompt()
		{
			foreach (FlashCardItemBase item in this.answers)
			{
				if (item.Equals(this.question))
				{
					item.Status = FlashCardItemStatus.Right;
				}
			}
			this.question.Status = FlashCardItemStatus.Right;
			this.status = FlashCardStatus.Prompt;
			this.statistics.PromptAnswer();
			RaisePropertyChangedEvent("Status");
		}

		#endregion

		#region INotifyPropertyChanged

        [System.Reflection.Obfuscation(Exclude = true)]
        public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChangedEvent(String propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion
	}

    internal class AD_Catalog : AutoDictionary<FlashCardItemType, List<IMeaning>>
    {
    }

    internal class AD_Catalogs : AutoDictionary<String, AD_Catalog>
    {
    }

	public class FlashCardsGenerator : IFlashCardsGenerator
	{
		const int COUNT = 5;

		private Random rnd;
		private IVocabulary vocabulary;
		private IUserProfile profile;
		private List<IMeaning> meanings;
		private Queue<IMeaning> cache;
        private bool registered = false;

        private AD_Catalogs catalogs;

		class MeaningsComparer : IComparer<IMeaning>
		{
            private int count;

			public MeaningsComparer()
			{
                this.count = 0;
			}

			public int Compare(IMeaning x, IMeaning y)
			{
                ++this.count;
                IStatistics sx = x.Statistics;
				IStatistics sy = y.Statistics;
				return - sx.Score.CompareTo(sy.Score);
			}

            public int Count
            {
                get
                {
                    return this.count;
                }
            }
		}

		public FlashCardsGenerator()
		{
		}

        public void Initialize(IUserProfile profile, IVocabulary vocabulary, IProgressReport progress, bool registered)
        {
            Debug.Assert(vocabulary != null);
            Debug.Assert(profile != null);
            this.vocabulary = vocabulary;
            this.profile = profile;
            this.rnd = new Random(DateTime.Now.Millisecond);
            this.meanings = new List<IMeaning>();
            this.cache = new Queue<IMeaning>();
            this.registered = registered;
            this.catalogs = new AD_Catalogs();
            CollectMeanings(progress);
        }

		protected void CollectMeanings(IProgressReport progress)
		{
            int wordsCount = this.vocabulary.Words.Count;
            int status = 0;
            ProgressReportHelper.Report(progress, ProgressType.Start, 3 * wordsCount);
            
            List<IMeaning> meanings = new List<IMeaning>();
            
            // collect all the meanings
			foreach (IWord word in this.vocabulary.Words)
			{
				foreach (IMeaning meaning in word.Meanings)
				{
                    AD_Catalog catalog = this.catalogs[meaning.Word.Type];

					if (!StringUtils.IsEmpty(meaning.Word.Text))
					{
						catalog[FlashCardItemType.Word].Add(meaning);
					}

					if (meaning.Translations.Count > 0)
					{
						catalog[FlashCardItemType.Translations].Add(meaning);
					}

					if (meaning.Synonyms.Count > 0)
					{
						catalog[FlashCardItemType.Synonyms].Add(meaning);
					}

					if (meaning.Antonyms.Count > 0)
					{
						catalog[FlashCardItemType.Antonyms].Add(meaning);
					}

					if (!StringUtils.IsEmpty(meaning.Definition))
					{
						catalog[FlashCardItemType.Definition].Add(meaning);
					}

                    meanings.Add(meaning);
				}
                ProgressReportHelper.Report(progress, ProgressType.Status, ++status);
			}

            // filter out those that cannot be run through flash cards
            int meaningsCount1 = meanings.Count;
            int meaningIndex1 = 0;
            foreach (IMeaning meaning in meanings)
            {
                if (GetFlashCardItemTypes(meaning) != FlashCardItemType.None)
                {
                    this.meanings.Add(meaning);
                }
                ++meaningIndex1;
                ProgressReportHelper.Report(progress, ProgressType.Status, status + meaningIndex1 * wordsCount / meaningsCount1);
            }

            // shuffle the meanings to avoid alphabetical order for new vocabularies
            int meaningsCount2 = this.meanings.Count;
            int meaningIndex2 = 0;
			for (int i = this.meanings.Count - 1; i > 0; --i)
			{
				int j = this.rnd.Next(i);
				IMeaning temp = this.meanings[i];
				this.meanings[i] = this.meanings[j];
				this.meanings[j] = temp;
                ++meaningIndex2;
                ProgressReportHelper.Report(progress, ProgressType.Status, 2 * status + meaningIndex2 * wordsCount / meaningsCount2);
			}

            // sort the list by score
            DateTime time1 = DateTime.Now;
            MeaningsComparer comparer = new MeaningsComparer();
			this.meanings.Sort(comparer);
            int count = comparer.Count;
            DateTime time2 = DateTime.Now;
            TimeSpan span = time2 - time1;
            int s = span.Seconds;

            ProgressReportHelper.Report(progress, ProgressType.End, 0);
		}

        private FlashCardItemType GetFlashCardItemTypes(IMeaning meaning)
        {
            FlashCardItemType type = FlashCardItemType.None;

            if (!StringUtils.IsEmpty(meaning.Word.Text))
            {
                AD_Catalog catalog = this.catalogs[meaning.Word.Type];

                // word question
                if (meaning.Translations.Count > 0 && catalog[FlashCardItemType.Translations].Count >= COUNT)
                {
                    type |= FlashCardItemType.Translations;
                }

                if (meaning.Synonyms.Count > 0 && catalog[FlashCardItemType.Synonyms].Count >= COUNT && meaning.Synonyms.Count > 0)
                {
                    type |= FlashCardItemType.Synonyms;
                }

                if (meaning.Antonyms.Count > 0 && catalog[FlashCardItemType.Antonyms].Count >= COUNT)
                {
                    type |= FlashCardItemType.Antonyms;
                }

                if (!StringUtils.IsEmpty(meaning.Definition) && catalog[FlashCardItemType.Definition].Count >= COUNT)
                {
                    type |= FlashCardItemType.Definition;
                }

                type = (FlashCardItemType)((int)type << 4);

                // word answers
                if (catalog[FlashCardItemType.Word].Count >= COUNT)
                {
                    if (meaning.Translations.Count > 0)
                    {
                        type |= FlashCardItemType.Translations;
                    }

                    if (meaning.Synonyms.Count > 0)
                    {
                        type |= FlashCardItemType.Synonyms;
                    }

                    if (meaning.Antonyms.Count > 0)
                    {
                        type |= FlashCardItemType.Antonyms;
                    }

                    if (!StringUtils.IsEmpty(meaning.Definition))
                    {
                        type |= FlashCardItemType.Definition;
                    }
                }
            }

            return type;
        }

        private FlashCardItemType GetRandomFlashCardType(IMeaning meaning)
        {
            FlashCardItemType types = GetFlashCardItemTypes(meaning);
            Debug.Assert(types != FlashCardItemType.None);
            List<FlashCardItemType> list = new List<FlashCardItemType>();
            for (int i = 0; i < 8; ++i)
            {
                FlashCardItemType mask = (FlashCardItemType)(0x01 << i);
                FlashCardItemType t = types & mask;
                if (t != FlashCardItemType.None)
                {
                    list.Add(t);
                }
            }
            Debug.Assert(list.Count > 0);
            return list[this.rnd.Next(list.Count)];
        }

        private void ScoreChanged(Object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Score")
			{
				IStatistics statistics = (IStatistics)sender;

                IMeaning meaning = null;
				foreach (IMeaning m in this.meanings)
                {
                    if (m.Statistics == statistics)
                    {
                        meaning = m;
                        break;
                    }
                }
                Debug.Assert(meaning != null);

                this.meanings.Remove(meaning);
				int index = this.meanings.BinarySearch(meaning, new MeaningsComparer());
				index = (index < 0) ? ~index : index + 1;
				this.meanings.Insert(index, meaning);
				(statistics as INotifyPropertyChanged).PropertyChanged -= new PropertyChangedEventHandler(ScoreChanged);
			}
		}

		private void Cache()
		{
			if (this.cache.Count == 0)
			{
				List<IMeaning> temp = new List<IMeaning>();
				foreach (IMeaning meaning in this.meanings)
				{
					temp.Add(meaning);
					if (temp.Count >= 15)
					{
						break;
					}
				}

				while (temp.Count > 0)
				{
					int index = this.rnd.Next(temp.Count);
					IMeaning meaning = temp[index];
					this.cache.Enqueue(meaning);
					temp.RemoveAt(index);
				}
			}
		}

		private FlashCardItemBase ConstructFlashCardItem(IMeaning meaning, FlashCardItemType cardType)
		{
			Debug.Assert(cardType != FlashCardItemType.None);

			FlashCardItemBase item = null;
			switch (cardType)
			{
				case FlashCardItemType.Word:
					item = new FlashCardItemWord(meaning);
					break;
				case FlashCardItemType.Translations:
					item = new FlashCardItemTranslations(meaning);
					break;
				case FlashCardItemType.Synonyms:
					item = new FlashCardItemSynonyms(meaning);
					break;
				case FlashCardItemType.Antonyms:
					item = new FlashCardItemAntonyms(meaning);
					break;
				case FlashCardItemType.Definition:
					item = new FlashCardItemDefinition(meaning);
					break;
			}
			return item;
		}

		public IFlashCard Generate()
		{
            if (this.meanings.Count == 0)
                return null;

			Cache();

            if (this.cache.Count == 0)
                return null;

			IMeaning meaningQuestion = this.cache.Dequeue();

            DateTime now = DateTime.Now;
            if (!this.registered && (this.profile.Saved > now || (now - profile.Created).Days > 30))
            {
                this.cache.Enqueue(meaningQuestion);
            }

            FlashCardItemType typeQuestion = FlashCardItemType.None;
            FlashCardItemType typeAnswers = FlashCardItemType.None;

            FlashCardItemType type = GetRandomFlashCardType(meaningQuestion);
             
            if (type > FlashCardItemType.Word)
            {
                typeQuestion = FlashCardItemType.Word;
                typeAnswers = (FlashCardItemType)((int)type >> 4);
            }
            else
            {
                typeQuestion = type;
                typeAnswers = FlashCardItemType.Word;
            }

			IFlashCardItem itemQuestion = ConstructFlashCardItem(meaningQuestion, typeQuestion);

			List<IFlashCardItem> itemAnswers = new List<IFlashCardItem>();

			IList<IMeaning> meaningAnswers = this.catalogs[meaningQuestion.Word.Type][typeAnswers];

			while (itemAnswers.Count < COUNT - 1)
			{
				IMeaning meaningAnswer = meaningAnswers[this.rnd.Next(meaningAnswers.Count)];
				if (!meaningAnswer.Equals(meaningQuestion))
				{
                    IFlashCardItem item = ConstructFlashCardItem(meaningAnswer, typeAnswers);
                    if (!Contains(itemAnswers, item))
                    {
                        itemAnswers.Add(item);
                    }
				}
			}

			IFlashCardItem itemAnswer = ConstructFlashCardItem(meaningQuestion, typeAnswers);
			int indexAnswer = this.rnd.Next(itemAnswers.Count);
			itemAnswers.Insert(indexAnswer, itemAnswer);

			IStatistics statisticsQuestion = meaningQuestion.Statistics;
			(statisticsQuestion as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ScoreChanged);
			return new FlashCard(itemQuestion, itemAnswers, meaningQuestion.Example, statisticsQuestion);
		}

        private bool Contains(IList<IFlashCardItem> items, IFlashCardItem item)
        {
            bool contains = false;
            foreach (IFlashCardItem it in items)
            {
                if (CompareDisplay(it, item))
                {
                    contains = true;
                    break;
                }
            }
            return contains;
        }

        private bool CompareDisplay(IFlashCardItem item1, IFlashCardItem item2)
        {
            return (item1.Equals(item2) || item1.Text.Equals(item2.Text));
        }
	}   
}
