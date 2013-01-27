using System;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Linq;
using System.Linq.Expressions;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Common.Data;
using LinguaSpace.Common.Diagnostics;
using LinguaSpace.Common.Text;
using LinguaSpace.Words.Data;
using LinguaSpace.Words.Resources;

namespace LinguaSpace.Words.ComponentModel
{
	public class TextModel : DataPresentationModel
	{
		private String columnName;

		public TextModel(DataRowView rowView, String columnName)
			: base(rowView)
		{
			Debug.Assert(columnName != null);
			this.columnName = columnName;
		}

		public String Name
		{
			get
			{
				return String.Format("_{0}:", columnName);
			}
		}

		public String Text
		{
			get
			{
				return (String)RowView[columnName];
			}

			set
			{
				Debug.Assert(value != null);
				RowView[columnName] = value;
			}
		}

		public override void NotifyPropertyChanged()
		{
			RaisePropertyChangedEvent("Text");
		}
	}

	public class TypeModel : TextModel
	{
		public TypeModel(DataRowView rv)
			: base(rv, Strings.TYPE)
		{
		}

		public String Type
		{
			get
			{
				return (String)this.RowView[Strings.TYPE];
			}
			set
			{
				Debug.Assert(value != null);
				this.RowView[Strings.TYPE] = value;
			}
		}

		public override void NotifyPropertyChanged()
		{
			RaisePropertyChangedEvent(Strings.TYPE);
		}
	}

	public class VocabularyModel : DataPresentationModel
	{
		private PresentationBindingList<TypeModel> types;

		public VocabularyModel(DataRowView rv)
			: base(rv)
		{
			LinguaSpaceDataSet dataSet = (LinguaSpaceDataSet)rv.DataView.Table.DataSet;
			this.types = new PresentationBindingList<TypeModel>(dataSet.Tables[Strings.TYPES].DefaultView);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
				this.types.Dispose();
		}

		public String Name
		{
			get
			{
				return (String)this.RowView[Strings.NAME];
			}
			set
			{
				Debug.Assert(value != null);
				this.RowView[Strings.NAME] = value;
			}
		}

		public String Description
		{
			get
			{
				return (String)this.RowView[Strings.DESCRIPTION];
			}
			set
			{
				Debug.Assert(value != null);
				this.RowView[Strings.DESCRIPTION] = value;
			}
		}

		public CultureInfo TargetLang
		{
			get
			{
				return AdoUtils.GetCultureInfo(RowView, Strings.TARGET_LANG);
			}

			set
			{
				AdoUtils.SetCultureInfo(RowView, Strings.TARGET_LANG, value);
			}
		}

		public CultureInfo NativeLang
		{
			get
			{
				return AdoUtils.GetCultureInfo(RowView, Strings.NATIVE_LANG);
			}

			set
			{
				AdoUtils.SetCultureInfo(RowView, Strings.NATIVE_LANG, value);
			}
		}

		public IPresentationList<TypeModel> Types
		{
			get
			{
				return this.types;
			}
		}

		public override void NotifyPropertyChanged()
		{
			RaisePropertyChangedEvent(Strings.NAME);
			RaisePropertyChangedEvent(Strings.DESCRIPTION);
			RaisePropertyChangedEvent(Strings.TARGET_LANG);
			RaisePropertyChangedEvent(Strings.NATIVE_LANG);
		}
	}

	public class WordItemModel : DataPresentationModel
	{
		public WordItemModel(DataRowView rv)
			: base(rv)
		{
		}
		
		public Guid WordId
		{
			get
			{
				return (Guid)this.RowView[Strings.WORD_ID];
			}
		}
		
		public String Text
		{
			get
			{
				String prefix = (String)this.RowView[Strings.PREFIX];
				String word = (String)this.RowView[Strings.WORD];
				return (prefix.Length > 0) ? String.Format("{0} {1}", prefix, word) : word;
			}
		}

		public String Type
		{
			get
			{
				Object typeId = this.RowView[Strings.TYPE_ID];
				if (typeId == DBNull.Value)
					return String.Empty;

				DataRowView type = AdoUtils.FindRelatedRow(this.RowView, Strings.TYPES, Strings.TYPE_ID);
				return String.Format("({0})", (String)type[Strings.TYPE]);
			}
		}

		public String Translations
		{
			get
			{
				StringBuilder sb = new StringBuilder(512);

				DataRowView[] meanings = AdoUtils.FindRelatedRows(this.RowView, Strings.MEANINGS, Strings.WORD_ID);
				
				if (meanings != null)
				{
					for (int i = 0; i < meanings.Length; ++i)
					{
						DataRowView[] translations = AdoUtils.FindRelatedRows(meanings[i], Strings.TRANSLATIONS, Strings.MEANING_ID);
					
						if (translations != null)
						{
							if (i > 0)
								sb.Append("\n");

							sb.Append("- ");
						
							for (int j = 0; j < translations.Length; ++j)
							{
								if (j > 0)
									sb.Append(", ");

								sb.Append(translations[j][Strings.TRANSLATION]);
							}
						}
					}
				}
				
				return sb.ToString();
			}
		}

		public override void NotifyPropertyChanged()
		{
			Tracer.WriteLine("WordItemModel.NotifyPropertyChanged");
			RaisePropertyChangedEvent(Strings.TEXT);
			RaisePropertyChangedEvent(Strings.TYPE);
			RaisePropertyChangedEvent(Strings.TRANSLATIONS);
		}
	}

	public class WordEditModel : DataPresentationModel
	{
		public WordEditModel(DataRowView rv)
			: base(rv)
		{
		}

		public String Prefix
		{
			get
			{
				return (String)this.RowView[Strings.PREFIX];
			}
			set
			{
				Debug.Assert(value != null);
				this.RowView[Strings.PREFIX] = value;
			}
		}

		public String Word
		{
			get
			{
				return (String)this.RowView[Strings.WORD];
			}
			set
			{
				Debug.Assert(value != null);
				this.RowView[Strings.WORD] = value;
			}
		}

		public Guid TypeId
		{
			get
			{
				Object typeId = this.RowView[Strings.TYPE_ID];
				return (typeId == DBNull.Value) ? Guid.Empty : (Guid)typeId;
			}
			set
			{
				this.RowView[Strings.TYPE_ID] = value;
			}
		}
		
		public DataView Types
		{
			get
			{
				return AdoUtils.GetDataView(this.RowView, Strings.TYPES);
			}
		}
	}

	public class MeaningItemModel : DataPresentationModel
	{
		public MeaningItemModel(DataRowView rv)
			: base(rv)
		{
		}

		public Guid MeaningId
		{
			get
			{
				return (Guid)this.RowView[Strings.MEANING_ID];
			}
		}

		public String Usage
		{
			get
			{
				StringBuilder sb = new StringBuilder(128);

				StringUtils.AppendToken(sb, (String)this.RowView[Strings.PREFIX]);

				DataRowView word = AdoUtils.FindRelatedRow(this.RowView, Strings.WORDS, Strings.WORD_ID);
				StringUtils.AppendToken(sb, (String)word[Strings.PREFIX]);
				StringUtils.AppendToken(sb, (String)word[Strings.WORD]);
				
				StringUtils.AppendToken(sb, (String)this.RowView[Strings.POSTFIX]);
				
				return sb.ToString();				
			}
		}

		public GridLength HasUsage
		{
			get
			{
				String prefix = (String)this.RowView[Strings.PREFIX];
				String postfix = (String)this.RowView[Strings.POSTFIX];
				return (prefix.Length + postfix.Length) > 0 ? GridLength.Auto : new GridLength(0);
			}
		}
		
		public String Translations
		{
			get
			{
				DataRowView[] translations = AdoUtils.FindRelatedRows(this.RowView, Strings.TRANSLATIONS, Strings.MEANING_ID);

				if (translations == null)
					return String.Empty;

				StringBuilder sb = new StringBuilder(20 * translations.Length);

				foreach (DataRowView t in translations)
				{
					String text = (String)t[Strings.TRANSLATION];
					if (text != String.Empty)
					{
						if (sb.Length > 0)
							sb.Append(", ");

						sb.Append(text);
					}
				}

				return sb.ToString();
			}
		}
		
		public GridLength HasTranslations
		{
			get
			{
				DataRowView translation = AdoUtils.FindRelatedRow(this.RowView, Strings.TRANSLATIONS, Strings.MEANING_ID);
				return (translation != null) ? GridLength.Auto : new GridLength(0);
			}
		}
		
		public String Synonyms
		{
			get
			{
				DataRowView[] synonyms = AdoUtils.FindRelatedRows(this.RowView, Strings.SYNONYMS, Strings.MEANING_ID);
				
				if (synonyms == null)
					return String.Empty;
				
				StringBuilder sb = new StringBuilder(synonyms.Length * 20);
				
				foreach (DataRowView synonym in synonyms)
				{
					DataRowView rowWord = AdoUtils.FindRelatedRow(synonym, Strings.WORDS, Strings.WORD_ID);

					String prefix = (String)rowWord[Strings.PREFIX];
					String word = (String)rowWord[Strings.WORD];
					
					if (sb.Length > 0)
						sb.Append(", ");
						
					if (prefix != String.Empty)
					{
						sb.Append(prefix);
						sb.Append(" ");
					}
					sb.Append(word);
				}
				
				return sb.ToString();
			}
		}

		public GridLength HasSynonyms
		{
			get
			{
				DataRowView synonym = AdoUtils.FindRelatedRow(this.RowView, Strings.SYNONYMS, Strings.MEANING_ID);
				return (synonym != null) ? GridLength.Auto : new GridLength(0);
			}
		}

		public String Antonyms
		{
			get
			{
				DataRowView[] antonyms = AdoUtils.FindRelatedRows(this.RowView, Strings.ANTONYMS, Strings.MEANING_ID);

				if (antonyms == null)
					return String.Empty;

				StringBuilder sb = new StringBuilder(antonyms.Length * 20);

				foreach (DataRowView antonym in antonyms)
				{
					DataRowView rowWord = AdoUtils.FindRelatedRow(antonym, Strings.WORDS, Strings.WORD_ID);

					String prefix = (String)rowWord[Strings.PREFIX];
					String word = (String)rowWord[Strings.WORD];

					if (sb.Length > 0)
						sb.Append(", ");

					if (prefix != String.Empty)
					{
						sb.Append(prefix);
						sb.Append(" ");
					}
					sb.Append(word);
				}

				return sb.ToString();
			}
		}

		public GridLength HasAntonyms
		{
			get
			{
				DataRowView antonym = AdoUtils.FindRelatedRow(this.RowView, Strings.ANTONYMS, Strings.MEANING_ID);
				return (antonym != null) ? GridLength.Auto : new GridLength(0);
			}
		}
		
		public String Definition
		{
			get
			{
				return (String)this.RowView[Strings.DEFINITION];
			}
		}

		public GridLength HasDefinition
		{
			get
			{
				return (this.Definition.Length > 0) ? GridLength.Auto : new GridLength(0);
			}
		}
		
		public String Example
		{
			get
			{
				return (String)this.RowView[Strings.EXAMPLE];
			}
		}

		public GridLength HasExample
		{
			get
			{
				return (this.Example.Length > 0) ? GridLength.Auto : new GridLength(0);
			}
		}

        public Visibility IsNew
        {
            get
            {
                Visibility isNew = Visibility.Collapsed;
                Guid meaningId = (Guid)RowView[Strings.MEANING_ID];
                DataRow history = RowView.Row.GetChildRows(Strings.FK_HISTORY_MEANINGS).FirstOrDefault<DataRow>();
                if (history != null)
                {
                    TimeSpan age = DateTime.UtcNow - history.Field<DateTime>(Strings.SYNC_MOD);
                    if (age.Days <= 3)
                    {
                        isNew = Visibility.Visible;
                    }
                }
                return isNew;
            }
        }

		public override void NotifyPropertyChanged()
		{
			UpdateHistory();
		
			RaisePropertyChangedEvent("Usage");
			RaisePropertyChangedEvent("HasUsage");
			RaisePropertyChangedEvent("Translations");
			RaisePropertyChangedEvent("HasTranslations");
			RaisePropertyChangedEvent("Synonyms");
			RaisePropertyChangedEvent("HasSynonyms");
			RaisePropertyChangedEvent("Antonyms");
			RaisePropertyChangedEvent("HasAntonyms");
			RaisePropertyChangedEvent("Definition");
			RaisePropertyChangedEvent("HasDefinition");
			RaisePropertyChangedEvent("Example");
			RaisePropertyChangedEvent("HasExample");
		}
		
		public void UpdateHistory()
		{
			Guid meaningId = (Guid)RowView[Strings.MEANING_ID];
			DataRow history = RowView.Row.GetChildRows(Strings.FK_HISTORY_MEANINGS).FirstOrDefault<DataRow>();
			if (history == null)
			{
				DataTable tableHistory = RowView.DataView.Table.DataSet.Tables[Strings.HISTORY];
				DataTable tableVocabularies = RowView.DataView.Table.DataSet.Tables[Strings.VOCABULARIES];
				history = tableHistory.NewRow();
				history.SetField<Guid>(Strings.VOCABULARY_ID, tableVocabularies.Rows[0].Field<Guid>(Strings.VOCABULARY_ID));
				history.SetField<Guid>(Strings.MEANING_ID, meaningId);
				tableHistory.Rows.Add(history);
			}
			history.SetField<DateTime>(Strings.SYNC_MOD, DateTime.UtcNow);
            RaisePropertyChangedEvent("IsNew");
		}
	}
	
	public class TranslationModel : TextModel
	{
		public TranslationModel(DataRowView rv)
			: base(rv, Strings.TRANSLATION)
		{
		}

		public String Translation
		{
			get
			{
				return (String)this.RowView[Strings.TRANSLATION];
			}
			set
			{
				Debug.Assert(value != null);
				this.RowView[Strings.TRANSLATION] = value;
			}
		}

		public override void NotifyPropertyChanged()
		{
			RaisePropertyChangedEvent(Strings.TRANSLATION);
		}
	}

	public class WordRefItemModel : DataPresentationModel
	{
		public WordRefItemModel(DataRowView rv)
			: base(rv)
		{
		}
		
		public String Word
		{
			get
			{
				DataRowView w = AdoUtils.FindRelatedRow(this.RowView, Strings.WORDS, Strings.WORD_ID);
				if (w == null)
					return String.Empty;
				
				String prefix = (String)w[Strings.PREFIX];
				String word = (String)w[Strings.WORD];
				return (prefix.Length > 0) ? String.Format("{0} {1}", prefix, word) : word;
			}
		}

		public override void NotifyPropertyChanged()
		{
			RaisePropertyChangedEvent(Strings.WORD);
		}
	}

	public class WordRefEditModel : DataPresentationModel
	{
		private WordFinderModel finder;
	
		public WordRefEditModel(DataRowView rv)
			: base(rv)
		{
			// prepare filter type id
			DataRowView rowMeaning = AdoUtils.FindRelatedRow(rv, Strings.MEANINGS, Strings.MEANING_ID);
			DataRowView rowWord = AdoUtils.FindRelatedRow(rowMeaning, Strings.WORDS, Strings.WORD_ID);
			Guid filterTypeId = (Guid)rowWord[Strings.TYPE_ID];
			
			// prepare filter word text
			DataRowView rowRefWord = AdoUtils.FindRelatedRow(rv, Strings.WORDS, Strings.WORD_ID);
			String filterWordText = (rowRefWord != null) ? (String)rowRefWord[Strings.WORD] : String.Empty;
			
			DataTable tableWords = AdoUtils.GetDataView(rv, Strings.WORDS).Table;
			
			this.finder = new WordFinderModel(tableWords, filterTypeId, filterWordText);
			this.finder.Words.CurrentChanged += new EventHandler(FinderWords_CurrentChanged);
			SynchCurrent();
		}

		private void FinderWords_CurrentChanged(object sender, EventArgs e)
		{
			SynchCurrent();
		}

		private void SynchCurrent()
		{
			DataRowView rowWord = (DataRowView)this.finder.Words.CurrentItem;
			this.RowView[Strings.WORD_ID] = rowWord != null ? rowWord[Strings.WORD_ID] : DBNull.Value;
			RaisePropertyChangedEvent("WordText");
		}
		
		public String WordText
		{
			get
			{
				return WordsTextUtils.GetWordText(AdoUtils.FindRelatedRow(this.RowView, Strings.WORDS, Strings.WORD_ID));
			}
		}
		
		public String FilterWordText
		{
			get
			{
				return this.finder.FilterWordText;
			}
			
			set
			{
				Debug.Assert(value != null);
				this.finder.FilterWordText = value;
			}
		}
		
		public ICollectionView Words
		{
			get
			{
				return this.finder.Words;
			}
		}
	}

	public class MeaningEditModel : DataPresentationModel
	{
		private PresentationBindingList<TranslationModel> translations;
		private PresentationBindingList<WordRefItemModel> synonyms;
		private PresentationBindingList<WordRefItemModel> antonyms;

		public MeaningEditModel(DataRowView row)
			: base(row)
		{
			this.translations = CreateFilteredPresentationList<TranslationModel>(Strings.TRANSLATIONS, Strings.MEANING_ID);
			this.synonyms = CreateFilteredPresentationList<WordRefItemModel>(Strings.SYNONYMS, Strings.MEANING_ID);
			this.antonyms = CreateFilteredPresentationList<WordRefItemModel>(Strings.ANTONYMS, Strings.MEANING_ID);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this.translations.Dispose();
				this.synonyms.Dispose();
				this.antonyms.Dispose();
			}
		}

		public String Prefix
		{
			get
			{
				return (String)this.RowView[Strings.PREFIX];
			}

			set
			{
				Debug.Assert(value != null);
				this.RowView[Strings.PREFIX] = value;
			}
		}

		public String Word
		{
			get
			{
				if (this.RowView.Row.IsNull(Strings.WORD_ID))
					return String.Empty;

				DataRowView w = AdoUtils.FindRelatedRow(this.RowView, Strings.WORDS, Strings.WORD_ID);
				String prefix = (String)w[Strings.PREFIX];
				String word = (String)w[Strings.WORD];
				return (prefix.Length > 0) ? String.Format("{0} {1}", prefix, word) : word;
			}
		}

		public String Postfix
		{
			get
			{
				return (String)this.RowView[Strings.POSTFIX];
			}

			set
			{
				Debug.Assert(value != null);
				this.RowView[Strings.POSTFIX] = value;
			}
		}

		public PresentationBindingList<TranslationModel> Translations
		{
			get
			{
				return this.translations;
			}
		}

		public PresentationBindingList<WordRefItemModel> Synonyms
		{
			get
			{
				return this.synonyms;
			}
		}

		public PresentationBindingList<WordRefItemModel> Antonyms
		{
			get
			{
				return this.antonyms;
			}
		}

		public String Definition
		{
			get
			{
				return (String)this.RowView[Strings.DEFINITION];
			}

			set
			{
				Debug.Assert(value != null);
				this.RowView[Strings.DEFINITION] = value;
			}
		}

		public String Example
		{
			get
			{
				return (String)this.RowView[Strings.EXAMPLE];
			}

			set
			{
				Debug.Assert(value != null);
				this.RowView[Strings.EXAMPLE] = value;
			}
		}
	}
	
	public class ActionModel : DataPresentationModel
	{
		public ActionModel(DataRowView row)
			: base(row)
		{
		}
		
		public String Action
		{
			get
			{
				return (String)this.RowView[Strings.ACTION];
			}
		}

		public String Description
		{
			get
			{
				return (String)this.RowView[Strings.DESCRIPTION];
			}
		}
		
		public int Weight
		{
			get
			{
				return AdjustSign((int)this.RowView[Strings.WEIGHT]);
			}
			set
			{
				this.RowView[Strings.WEIGHT] = AdjustSign(value);
			}
		}
		
		private int AdjustSign(int i)
		{
			return ((String)this.RowView[Strings.ACTION_ID] != "R") ? i : -i;
		}
	}

	public class ProfileModel : DataPresentationModel
	{
		public ProfileModel(DataRowView row)
			: base(row)
		{
			DataView actions = AdoUtils.GetDataView(this.RowView, Strings.ACTIONS);
			if (actions.Count == 0)
			{
				CreateAction(actions, Strings.ACTION_ID_MOD, Strings.ACTION_MOD, 10);
				CreateAction(actions, Strings.ACTION_ID_RIGHT, Strings.ACTION_RIGHT, -1);
				CreateAction(actions, Strings.ACTION_ID_WRONG, Strings.ACTION_WRONG, 3);
				CreateAction(actions, Strings.ACTION_ID_PROMPT, Strings.ACTION_PROMPT, 5);
			}
		}

		private void CreateAction(DataView actions, String id, String action, int weight)
		{
			DataRowView row = actions.AddNew();
			row[Strings.ACTION_ID] = id;
			row[Strings.ACTION] = action;
			row[Strings.WEIGHT] = weight;
			row.EndEdit();			
		}

		public String Name
		{
			get
			{
				return (String)this.RowView[Strings.NAME];
			}

			set
			{
				Debug.Assert(value != null);
				this.RowView[Strings.NAME] = value;
			}
		}

		public String Description
		{
			get
			{
				return (String)this.RowView[Strings.NAME];
			}

			set
			{
				Debug.Assert(value != null);
				this.RowView[Strings.NAME] = value;
			}
		}
		
		public String DefaultVocabularyFileName
		{
			get
			{
				return (String)this.RowView[Strings.DEFAULT_VOCABULARY_PATH];
			}
			
			internal set
			{
				Debug.Assert(value != null);
				this.RowView[Strings.DEFAULT_VOCABULARY_PATH] = value;
			}
		}
		
		public int Sleep
		{
			get
			{
				return (int)this.RowView[Strings.SLEEP];
			}
			set
			{
				this.RowView[Strings.SLEEP] = value;
			}
		}

		public bool Beep
		{
			get
			{
				return (bool)this.RowView[Strings.BEEP];
			}
			set
			{
				this.RowView[Strings.BEEP] = value;
			}
		}
		
		public ActionModel ModificationAction
		{
			get
			{
				return this.GetActionHelper("M");
			}
		}

		public ActionModel RightAnswerAction
		{
			get
			{
				return this.GetActionHelper("R");
			}
		}

		public ActionModel WrongAnswerAction
		{
			get
			{
				return this.GetActionHelper("W");
			}
		}

		public ActionModel PromptAnswerAction
		{
			get
			{
				return this.GetActionHelper("P");
			}
		}

		private ActionModel GetActionHelper(String actionId)
		{
			DataView view = AdoUtils.GetDataView(this.RowView, Strings.ACTIONS);
			DataRowView row = AdoUtils.FindRow(view, Strings.ACTION_ID, actionId);
			return new ActionModel(row);
		}
	}

	public class LinguaSpaceModel : PresentationModel, ITransactionContextAware
	{
		private String pathVocabulary;
		private String pathProfile;
	
		private bool										isVocabularyDirty;
		
		private PresentationBindingList<VocabularyModel>	modelVocabularies;
		private PresentationBindingList<WordItemModel>		modelWords;
		private PresentationBindingList<MeaningItemModel>	modelMeanings;

		private PresentationBindingList<ProfileModel>		modelProfiles;

		public LinguaSpaceModel(LinguaSpaceDataSet dataSet)
			: base(dataSet)
		{
			this.pathVocabulary = String.Empty;
			this.pathProfile = String.Empty;
			this.isVocabularyDirty = false;
			HookListChanged(Strings.VOCABULARIES);
			HookListChanged(Strings.TYPES);
			HookListChanged(Strings.WORDS);
			HookListChanged(Strings.MEANINGS);
			HookListChanged(Strings.TRANSLATIONS);
			HookListChanged(Strings.SYNONYMS);
			HookListChanged(Strings.ANTONYMS);

			this.modelProfiles = new PresentationBindingList<ProfileModel>(this.DataSet.Tables[Strings.PROFILES].DefaultView);

			this.modelVocabularies = new PresentationBindingList<VocabularyModel>(this.DataSet.Tables[Strings.VOCABULARIES].DefaultView);

			this.modelWords = new PresentationBindingList<WordItemModel>(this.DataSet.Tables[Strings.WORDS].DefaultView);

			DataView meanings = this.DataSet.Tables[Strings.MEANINGS].DefaultView;
			ITypedList typedList = (ITypedList)meanings;
			PropertyDescriptorCollection properties = typedList.GetItemProperties(null);
			PropertyDescriptor property = properties.Find(Strings.WORD_ID, false);
			this.modelMeanings = new PresentationBindingList<MeaningItemModel>(new FilterBindingList(meanings, property));
		}

		private void HookListChanged(String tableName)
		{
			this.DataSet.Tables[tableName].DefaultView.ListChanged += new ListChangedEventHandler(OnListChanged);
		}

		private void OnListChanged(Object sender, ListChangedEventArgs e)
		{
			if (!this.isVocabularyDirty)
			{
				this.isVocabularyDirty = true;
				RaisePropertyChangedEvent("IsVocabularyDirty");
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this.modelVocabularies.Dispose();
				this.modelWords.Dispose();
				this.modelMeanings.Dispose();
			}
		}
		
		public LinguaSpaceDataSet DataSet
		{
			get
			{
				return (LinguaSpaceDataSet)this.Data;
			}
		}

		public ITransactionContext TransactionContext
		{
			get
			{
				return (ITransactionContext)this.DataSet;
			}
		}
		
		public VocabularyModel Vocabulary
		{
			get
			{
				if (!this.IsVocabularyOpen)
					return null;
				
				return (this.Vocabularies as IList<VocabularyModel>)[0];
			}
		}
		
		public IPresentationList<VocabularyModel> Vocabularies
		{
			get
			{
				return this.modelVocabularies;
			}
		}
		
		public IPresentationList<WordItemModel> Words
		{
			get
			{
				return this.modelWords;
			}						
		}

		public ProfileModel Profile
		{
			get
			{
				if (!this.IsProfileOpen)
					return null;

				return (this.Profiles as IList<ProfileModel>)[0];
			}
		}

		public IPresentationList<ProfileModel> Profiles
		{
			get
			{
				return this.modelProfiles;
			}
		}

		public IPresentationList<MeaningItemModel> Meanings
		{
			get
			{
				return this.modelMeanings;
			}
		}
		
		public WordFinderModel CreateWordFinder()
		{
			return new WordFinderModel(this.DataSet.Tables[Strings.WORDS]);
		}
		
		public MeaningFinderModel CreateMeaningFinder()
		{
			return new MeaningFinderModel(this.DataSet.Tables[Strings.TRANSLATIONS]);
		}
		
		public String ProfileFileName
		{
			get
			{
				return this.pathProfile;
			}
		}
		
		public String VocabularyFileName
		{
			get
			{
				return this.pathVocabulary;
			}
		}
		
		public bool IsVocabularyOpen
		{
			get
			{
				return (this.DataSet.Tables[Strings.VOCABULARIES].Rows.Count > 0);
			}
		}
		
		public bool IsVocabularyDirty
		{
			get
			{
				return this.isVocabularyDirty;
			}
		}
		
		public bool IsProfileOpen
		{
			get
			{
				return (this.DataSet.Tables[Strings.PROFILES].Rows.Count > 0);
			}
		}
		
		public String VocabularyName
		{
			get
			{
				if (!this.IsVocabularyOpen)
					return String.Empty;
					
				return (String)this.DataSet.Tables[Strings.VOCABULARIES].DefaultView[0][Strings.NAME];
			}
		}

		public String ProfileName
		{
			get
			{
				if (!this.IsProfileOpen)
					return String.Empty;

				return (String)this.DataSet.Tables[Strings.PROFILES].DefaultView[0][Strings.NAME];
			}
		}
		
		public void LoadVocabulary(String path)
		{
			Debug.Assert(!this.IsVocabularyOpen);
			Debug.Assert(this.IsProfileOpen);
			this.DataSet.LoadVocabulary(path);
			this.pathVocabulary = path;
			this.isVocabularyDirty = false;
			this.Profile.DefaultVocabularyFileName = path;
			RaisePropertyChangedEvent("VocabularyFileName");
			RaisePropertyChangedEvent("VocabularyName");
			RaisePropertyChangedEvent("IsVocabularyOpen");
			RaisePropertyChangedEvent("IsVocabularyDirty");
		}

		public void SaveVocabularyAs(String path)
		{
			Debug.Assert(this.IsVocabularyOpen);
			Debug.Assert(this.IsProfileOpen);

			if (this.pathVocabulary != path && File.Exists(this.pathVocabulary))
				File.Copy(this.pathVocabulary, path);
			
			this.DataSet.SaveVocabulary(path);
			this.pathVocabulary = path;
			this.isVocabularyDirty = false;
			this.Profile.DefaultVocabularyFileName = path;
			RaisePropertyChangedEvent("IsVocabularyDirty");
			RaisePropertyChangedEvent("VocabularyFileName");
		}
		
		public void SaveVocabulary()
		{
			Debug.Assert(this.pathVocabulary != String.Empty);
			SaveVocabularyAs(this.pathVocabulary);
		}
		
		public void CloseVocabulary()
		{
			Debug.Assert(this.IsVocabularyOpen);
			this.DataSet.CloseVocabulary();
			this.isVocabularyDirty = false;
			this.pathVocabulary = String.Empty;
			RaisePropertyChangedEvent("IsVocabularyOpen");
			RaisePropertyChangedEvent("IsVocabularyDirty");
			RaisePropertyChangedEvent("VocabularyFileName");
			RaisePropertyChangedEvent("VocabularyName");
		}
		
		public void LoadProfile(String path)
		{
			this.DataSet.LoadProfile(path);
			this.pathProfile = path;
			RaisePropertyChangedEvent("ProfileFileName");
			RaisePropertyChangedEvent("ProfileName");
			RaisePropertyChangedEvent("IsProfileOpen");
		}
		
		public void SaveProfileAs(String path)
		{
			if (this.pathProfile != path && File.Exists(this.pathProfile))
				File.Copy(this.pathProfile, path);
			this.DataSet.SaveProfile(path);
			this.pathProfile = path;
			RaisePropertyChangedEvent("ProfileFileName");
		}
		
		public void SaveProfile()
		{
			Debug.Assert(this.pathProfile != String.Empty);
			SaveProfileAs(this.pathProfile);
		}
		
		public void CloseProfile()
		{
			this.DataSet.CloseProfile();
			this.pathProfile = String.Empty;
			RaisePropertyChangedEvent("ProfileFileName");
			RaisePropertyChangedEvent("ProfileName");
			RaisePropertyChangedEvent("IsProfileOpen");
			RaisePropertyChangedEvent("Profile");
			RaisePropertyChangedEvent("Profiles");
		}

		public override void NotifyPropertyChanged()
		{
			base.NotifyPropertyChanged();
			
			RaisePropertyChangedEvent("ProfileFileName");
			RaisePropertyChangedEvent("ProfileName");
			RaisePropertyChangedEvent("VocabularyFileName");
			RaisePropertyChangedEvent("VocabularyName");
			RaisePropertyChangedEvent("IsVocabularyOpen");
			RaisePropertyChangedEvent("IsVocabularyDirty");
			RaisePropertyChangedEvent("IsProfileOpen");
		}
	}

	public class WordFinderModel : NotifyPropertyChangedImpl
	{
		private Guid wordId;
		private String wordText;

		private String filterWordText;
		private Guid filterTypeId;
		private DataView viewWords;
		private BindingListCollectionView collectionWords;
		private PropertyDescriptor propertyWord;

		public WordFinderModel(DataTable tableWords, Guid filterTypeId, String filterWordText)
		{
			Debug.Assert(tableWords != null);
			Debug.Assert(filterWordText != null);

			this.viewWords = new DataView(tableWords);
			this.viewWords.Sort = "Word, Prefix";
			this.propertyWord = BindingListUtils.GetProperty(this.viewWords, Strings.WORD);

			this.filterTypeId = filterTypeId;
			this.filterWordText = filterWordText;
			ApplyFilter();

			this.collectionWords = new BindingListCollectionView(this.viewWords);
			this.collectionWords.CurrentChanged += new EventHandler(collectionWords_CurrentChanged);
			AdjustCurrent();
		}
		
		public WordFinderModel(DataTable tableWords)
			: this(tableWords, Guid.Empty, String.Empty)
		{
		}

		private void collectionWords_CurrentChanged(Object sender, EventArgs e)
		{
			SynchCurrent();
		}

		private void SynchCurrent()
		{
			DataRowView row = (DataRowView)this.collectionWords.CurrentItem;

			if (row != null)
			{
				this.wordId = (Guid)row[Strings.WORD_ID];
				this.wordText = WordsTextUtils.GetWordText(row);
			}
			else
			{
				this.wordId = Guid.Empty;
				this.wordText = String.Empty;
			}

			RaisePropertyChangedEvent("WordId");
			RaisePropertyChangedEvent("WordText");
		}

		public Guid WordId
		{
			get
			{
				return this.wordId;
			}
		}

		public String WordText
		{
			get
			{
				return this.wordText;
			}
		}

		public Guid FilterTypeId
		{
			get
			{
				return this.filterTypeId;
			}
			set
			{
				this.filterTypeId = value;
				ApplyFilter();
				AdjustCurrent();
			}
		}

		public String FilterWordText
		{
			get
			{
				return this.filterWordText;
			}

			set
			{
				Debug.Assert(value != null);
				this.filterWordText = value;
				ApplyFilter();
				AdjustCurrent();
			}
		}

		protected void ApplyFilter()
		{
			StringBuilder filter = new StringBuilder();

			if (this.filterTypeId != Guid.Empty)
				filter.Append(String.Format("TypeId = '{0}'", this.filterTypeId));

			if (!StringUtils.IsEmpty(this.filterWordText))
			{
				if (filter.Length > 0)
					filter.Append(" AND ");

				filter.Append(String.Format("Word LIKE '%{0}%'", this.filterWordText));
			}

			this.viewWords.RowFilter = filter.ToString();
		}

		protected void AdjustCurrent()
		{
			DataRowView rowWord = BindingListUtils.FindItem<DataRowView>(this.viewWords, this.propertyWord, this.filterWordText);
			if (rowWord != null)
				this.collectionWords.MoveCurrentTo(rowWord);
			else
				this.collectionWords.MoveCurrentToFirst();
		}

		public ICollectionView Words
		{
			get
			{
				return this.collectionWords;
			}
		}
	}

	public class TranslationFinderItemModel : DataPresentationModel
	{
		public TranslationFinderItemModel(DataRowView row)
			: base(row)
		{
		}
		
		public Guid WordId
		{
			get
			{
				DataRowView meaning = AdoUtils.FindRelatedRow(this.RowView, Strings.MEANINGS, Strings.MEANING_ID);
				if (meaning == null)
					return Guid.Empty;
				return (Guid)meaning[Strings.WORD_ID];
			}
		}
		
		public Guid MeaningId
		{
			get
			{
				return (Guid)this.RowView[Strings.MEANING_ID];
			}
		}		
		
		public String Translation
		{
			get
			{
				return (String)this.RowView[Strings.TRANSLATION];
			}
		}
		
		public String Translations
		{
			get
			{
				DataRowView meaning = AdoUtils.FindRelatedRow(this.RowView, Strings.MEANINGS, Strings.MEANING_ID);
				if (meaning == null)
					return String.Empty;
				return WordsTextUtils.GetMeaningTranslations(meaning);
			}
		}
		
		public String Word
		{
			get
			{
				DataRowView meaning = AdoUtils.FindRelatedRow(this.RowView, Strings.MEANINGS, Strings.MEANING_ID);
				if (meaning == null)
					return String.Empty;
					
				DataRowView word = AdoUtils.FindRelatedRow(meaning, Strings.WORDS, Strings.WORD_ID);
				if (word == null)
					return String.Empty;
					
				return WordsTextUtils.GetWordText(word);
			}
		}
	}

	public class MeaningFinderModel : NotifyPropertyChangedImpl
	{
		private Guid wordId;
		private Guid meaningId;
		private String wordText;
		private String translationsText;
		
		private String filterTranslationText;
		
		private DataView viewTranslations;
		private PropertyDescriptor propertyTranslation;
		
		private DataPresentationList<TranslationFinderItemModel> modelTranslations;
		
		private BindingListCollectionView collectionTranslations;
		
		public MeaningFinderModel(DataTable tableTranslations)
		{
			Debug.Assert(tableTranslations != null);

			this.viewTranslations = new DataView(tableTranslations);
			this.viewTranslations.Sort = "Translation";
			this.propertyTranslation = BindingListUtils.GetProperty(this.viewTranslations, Strings.TRANSLATION);
			
			this.modelTranslations = new DataPresentationList<TranslationFinderItemModel>(this.viewTranslations);
			
			this.collectionTranslations = new BindingListCollectionView(this.modelTranslations);
			this.collectionTranslations.CurrentChanged += new EventHandler(collectionTranslations_CurrentChanged);
			
			AdjustCurrent();
		}

		private void collectionTranslations_CurrentChanged(object sender, EventArgs e)
		{
			SynchCurrent();
		}

		private void SynchCurrent()
		{
			TranslationFinderItemModel modelTranslation = (TranslationFinderItemModel)this.collectionTranslations.CurrentItem;
			
			if (modelTranslation != null)
			{
				this.wordId = modelTranslation.WordId;
				this.meaningId = modelTranslation.MeaningId;
				this.wordText = modelTranslation.Word;
				this.translationsText = modelTranslation.Translations;
			}
			else
			{
				this.wordId = Guid.Empty;
				this.meaningId = Guid.Empty;
				this.wordText = String.Empty;
				this.translationsText = String.Empty;
			}
			
			RaisePropertyChangedEvent("WordText");
			RaisePropertyChangedEvent("TranslationsText");
		}

		public Guid WordId
		{
			get
			{
				return this.wordId;
			}
		}

		public Guid MeaningId
		{
			get
			{
				return this.meaningId;
			}
		}
		
		public String WordText
		{
			get
			{
				return this.wordText;
			}
		}
		
		public String TranslationsText
		{
			get
			{
				return this.translationsText;
			}
		}
		
		public String FilterTranslationText
		{
			get
			{
				return this.filterTranslationText;
			}
			set
			{
				Debug.Assert(value != null);
				this.filterTranslationText = value;
				ApplyFilter();
				AdjustCurrent();
			}
		}

		protected void ApplyFilter()
		{
			String filter = String.Empty;
			if (!StringUtils.IsEmpty(this.filterTranslationText))
			{
				filter = String.Format("Translation LIKE '%{0}%'", this.filterTranslationText);	
			}
			this.viewTranslations.RowFilter = filter;
		}

		protected void AdjustCurrent()
		{
			DataRowView rowTranslation = BindingListUtils.FindItem<DataRowView>(this.viewTranslations, this.propertyTranslation, this.filterTranslationText);
			if (rowTranslation != null)
				this.collectionTranslations.MoveCurrentTo(new TranslationFinderItemModel(rowTranslation));
			else
				this.collectionTranslations.MoveCurrentToFirst();
		}

		public ICollectionView Translations
		{
			get
			{
				return this.collectionTranslations;
			}
		}
	}

	public class WordFinderModelValidator : Validator
	{
		private readonly WordFinderModel model;

		public WordFinderModelValidator(WordFinderModel model)
		{
			Debug.Assert(model != null);
			this.model = model;
			Hook(model);
			Validate();
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();
			
			if (this.IsValid)
			{
				if (this.model.WordId == Guid.Empty)
				{
					SetStatus(false, "Please select a word", ValidationMessageType.Error);
				}
			}
		}
	}
	
	public class MeaningFinderModelValidator : Validator
	{
		private readonly MeaningFinderModel model;
		
		public MeaningFinderModelValidator(MeaningFinderModel model)
		{
			Debug.Assert(model != null);
			this.model = model;
			Hook(model);
			Validate();
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();
			
			if (this.IsValid)
			{
				if (this.model.MeaningId == Guid.Empty)
				{
					SetStatus(false, "Please select a translation", ValidationMessageType.Error);
				}
			}
		}
	}	
}
