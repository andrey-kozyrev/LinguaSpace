using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

using LinguaSpace.Common;
using LinguaSpace.Data.Resources;

namespace LinguaSpace.Data
{
    internal abstract class VocabularyObject : NotifyPropertyChangedImpl, IVocabularyObject, IBinarySerializable
	{
		#region Fields

		private VocabularyImpl vocabulary;
		private VocabularyObject parent;

		#endregion

		#region Constructors

		protected VocabularyObject()
		{
			this.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
		}

		protected VocabularyObject(VocabularyImpl vocabulary)
		{
			Debug.Assert(vocabulary != null);
			this.vocabulary = vocabulary;
			this.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
		}

		#endregion

		#region IVocabularyObject

        [System.Reflection.Obfuscation(Exclude = true)]
		public virtual IVocabulary Vocabulary
		{
			get
			{
				return this.vocabulary;
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public virtual IVocabularyObject Parent
		{
			get
			{
				return this.parent;
			}

			set
			{
				this.parent = (VocabularyObject)value;
				RaisePropertyChangedEvent("Parent");
			}
		}

		#endregion

        #region IBinarySerializable

        public virtual void PrepareLoad()
        {
			this.RaiseEvents = false;
        }

        public abstract void Load(BinaryReader reader);

        public virtual void Load(BinaryReader reader, IProgressReport progress)
        {
            Load(reader);
        }

        public virtual void FinalizeLoad()
        {
			this.RaiseEvents = true;
        }

        public virtual void PrepareSave()
        {
			this.RaiseEvents = false;
        }

        public abstract void Save(BinaryWriter writer);

        public virtual void Save(BinaryWriter writer, IProgressReport progress)
        {
            Save(writer);
        }

        public virtual void FinalizeSave()
        {
			this.RaiseEvents = true;
		}

		#endregion

		#region Internal

		protected internal virtual void OnPropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			VocabularyObject parentImpl = this.Parent as VocabularyObject;
			if (parentImpl != null)
			{
				parentImpl.OnPropertyChanged(sender, e);
			}
		}

		#endregion
	}

    // helper class that allow to do search given the prefix, data and type
    // without creation of the heavyweight WordImpl
	[DebuggerDisplay("{Text}")]
    class WordDesc : IWord, IComparable
    {
        #region Fields

        private String type = String.Empty;
        private String prefix = String.Empty;
        private String data = String.Empty;

        #endregion

        #region Word

        internal WordDesc(String type, String prefix, String data)
        {
            Debug.Assert(type != null);
            Debug.Assert(prefix != null);
            Debug.Assert(data != null);
            this.type = type;
            this.prefix = prefix;
            this.data = data;
        }

		internal WordDesc(String prefix, String data)
		{
			Debug.Assert(prefix != null);
			Debug.Assert(data != null);
			this.prefix = prefix;
			this.data = data;
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public IVocabulary Vocabulary
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
		public IVocabularyObject Parent
		{
			get
			{
				throw new NotImplementedException();
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public String Type
        {
            get
            {
                return this.type;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public String Prefix
        {
            get
            {
                return this.prefix;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public String Data
        {
            get
            {
                return this.data;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public String Text
        {
            get
            {
				return StringUtils.ListToString(this.prefix, this.data);
			}
            set
            {
                throw new NotImplementedException();
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public MeaningCollection Meanings
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IMeaning CreateMeaning()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Comparison

        public override bool Equals(object obj)
        {
            Debug.Assert(obj != null);
            bool equals = false;
            WordDesc other = (obj as WordDesc);
            if (other != null)
            {
                equals = (this.data.Equals(other.data) &&
                           this.prefix.Equals(other.prefix) &&
                           this.type.Equals(other.type));
            }
            return equals;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            Debug.Assert(obj != null);
            IWord other = (obj as IWord);
            Debug.Assert(other != null);
            int result = this.data.CompareTo(other.Data);
            if (result == 0)
            {
                result = this.prefix.CompareTo(other.Prefix);
                if (result == 0)
                {
                    result = this.type.CompareTo(other.Type);
                }
            }
            return result;
        }

        #endregion
    }

	[DebuggerDisplay("{Text}")]
    class WordImpl : VocabularyObject, IWord, IComparable, IBinarySerializable, INotifyPropertyChanging
    {
        #region Fields

		private String prefix = String.Empty;
        private String data = String.Empty;
        private String type = String.Empty;
        private MeaningCollectionImpl meanings;

        #endregion

		#region Word

		internal WordImpl(VocabularyImpl vocabulary)
			: base(vocabulary)
        {
            this.meanings = new MeaningCollectionImpl(this);
            this.meanings.CollectionChanged += new NotifyCollectionChangedEventHandler(new NotifyCollectionPropertyChangedWrapper(this, "Meanings").CollectionChanged);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public String Type
        {
            get
            {
                return this.type;
            }
            set
            {
                Debug.Assert(value != null);
                RaisePropertyChangingEvent("Type");
                this.type = value.Trim();
                RaisePropertyChangedEvent("Type");
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public String Prefix
        {
            get
            {
                return this.prefix;
            }
            set
            {
                Debug.Assert(value != null);
                RaisePropertyChangingEvent("Prefix");
				this.prefix = value.Trim();
                RaisePropertyChangedEvent("Prefix");
				RaisePropertyChangedEvent("Text");
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public String Data
        {
            get
            {
                return this.data;
            }
            set
            {
                Debug.Assert(value != null);
                RaisePropertyChangingEvent("Data");
				this.data = value.Trim();
                RaisePropertyChangedEvent("Data");
				RaisePropertyChangedEvent("Text");
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public String Text
        {
            get
            {
				return StringUtils.ListToString(this.prefix, this.data);
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public MeaningCollection Meanings
        {
            get
            {
                return this.meanings;
            }
        }

        #endregion

        #region INotifyPropertyChanging

        public event PropertyChangingEventHandler PropertyChanging;

        protected void RaisePropertyChangingEvent(String propertyName)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion

		#region Override

		public override bool Equals(object obj)
		{
			Debug.Assert(obj != null);
			bool equals = false;
			WordImpl other = (obj as WordImpl);
			Debug.Assert(other != null);
			if (other != null)
			{
				equals = (this.data.Equals(other.data) &&
						   this.prefix.Equals(other.prefix) &&
						   this.type.Equals(other.type));
			}
			return equals;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return this.Text;
		}

		#endregion

		#region Comparison

        public int CompareTo(object obj)
        {
            Debug.Assert(obj != null);
            IWord other = (obj as IWord);
            Debug.Assert(other != null);
            int result = this.data.CompareTo(other.Data);
            if (result == 0)
            {
                result = this.prefix.CompareTo(other.Prefix);
                if (result == 0)
                {
                    result = this.type.CompareTo(other.Type);
                }
            }
            return result;
        }

        #endregion

        #region Serialization

		public override void PrepareLoad()
		{
			base.PrepareLoad();
			this.meanings.PrepareLoad();
		}

        public override void Load(BinaryReader reader)
        {
            this.prefix = reader.ReadString();
            this.data = reader.ReadString();
            this.type = reader.ReadString();
            this.meanings.Load(reader);
        }

        public override void FinalizeLoad()
        {
            this.meanings.FinalizeLoad();
			base.FinalizeLoad();
        }

		public override void PrepareSave()
		{
			base.PrepareSave();
			this.meanings.PrepareSave();
		}

        public override void Save(BinaryWriter writer)
        {
            writer.Write(this.prefix);
            writer.Write(this.data);
            writer.Write(this.type);
            this.meanings.Save(writer);
        }

		public override void FinalizeSave()
		{
			this.meanings.FinalizeSave();
			base.FinalizeSave();
		}

        #endregion
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    // Meaning
	[DebuggerDisplay("{Translations.ToString()}")]
    class MeaningImpl : VocabularyObject, IMeaning, IBinarySerializable
    {
        #region Fields

		private String prefix = String.Empty;
		private String postfix = String.Empty;
		private Guid guid = Guid.Empty;
        private UniqueStringCollectionImpl categories;
        private UniqueStringCollectionImpl translations;
        private WordCollectionImpl synonyms;
        private WordCollectionImpl antonyms;
        private String definition = String.Empty;
        private String example = String.Empty;
        private IStatistics statistics;

        #endregion

		#region Internal

		internal void OnMeaningsCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChangedEvent("Index");
		}

        private void OnUserProfileChanged(Object sender, PropertyChangedEventArgs e)
        {
            if (sender == this.Vocabulary && e.PropertyName == "UserProfile")
            {
                if (this.statistics != null)
                {
                    this.statistics = null;
                    RaisePropertyChangedEvent("Statistics");
                }
            }
        }

		#endregion

        #region Meaning

        internal MeaningImpl(VocabularyImpl vocabulary)
			: base(vocabulary)
        {
			this.guid = Guid.NewGuid();
            this.categories = new UniqueStringCollectionImpl();
            this.categories.CollectionChanged += new NotifyCollectionChangedEventHandler(new NotifyCollectionPropertyChangedWrapper(this, "Categories").CollectionChanged);
            this.translations = new UniqueStringCollectionImpl();
            this.translations.CollectionChanged += new NotifyCollectionChangedEventHandler(new NotifyCollectionPropertyChangedWrapper(this, "Translations").CollectionChanged);
            this.synonyms = new WordCollectionImpl(this);
            this.synonyms.CollectionChanged += new NotifyCollectionChangedEventHandler(new NotifyCollectionPropertyChangedWrapper(this, "Synonyms").CollectionChanged);
            this.antonyms = new WordCollectionImpl(this);
            this.antonyms.CollectionChanged += new NotifyCollectionChangedEventHandler(new NotifyCollectionPropertyChangedWrapper(this, "Antonyms").CollectionChanged);

            INotifyPropertyChanged npc = (INotifyPropertyChanged)vocabulary;
            npc.PropertyChanged += new PropertyChangedEventHandler(OnUserProfileChanged);
        }

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public IWord Word
		{
			get
			{
				return (IWord)this.Parent;
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public int Index
        {
            get
            {
				Debug.Assert(this.Parent != null);
				IWord parent = (IWord)this.Parent;
				return parent.Meanings.IndexOf(this) + 1;
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
		public String Prefix
		{
			get
			{
				return this.prefix;
			}
			set
			{
				Debug.Assert(value != null);
				this.prefix = value.Trim();
				RaisePropertyChangedEvent("Prefix");
				RaisePropertyChangedEvent("Usage");
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public override IVocabularyObject Parent
		{
			get
			{
				return base.Parent;
			}
			set
			{
				base.Parent = value;
				RaisePropertyChangedEvent("Usage");
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public String Postfix
		{
			get
			{
				return this.postfix;
			}
			set
			{
				Debug.Assert(value != null);
				this.postfix = value.Trim();
				RaisePropertyChangedEvent("Postfix");
				RaisePropertyChangedEvent("Usage");
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public String Usage
		{
			get
			{
				String usage = String.Empty;
				if (this.Parent != null && !(StringUtils.IsEmpty(this.prefix) && StringUtils.IsEmpty(this.postfix)))
				{
					IWord word = (IWord)this.Parent;
					usage = StringUtils.ListToString(this.Prefix, word.Text, this.Postfix);
				}
				return usage;
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public StringCollection Categories
        {
            get
            {
                return this.categories;
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public StringCollection Translations
        {
            get
            {
                return this.translations;
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public WordCollection Synonyms
        {
            get
            {
                return this.synonyms;
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public WordCollection Antonyms
        {
            get
            {
                return this.antonyms;
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public String Definition
        {
            get
            {
                return this.definition;
            }
            set
            {
                Debug.Assert(value != null);
				this.definition = value.Trim();
                RaisePropertyChangedEvent("Definition");
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public String Example
        {
            get
            {
                return this.example;
            }
            set
            {
                Debug.Assert(value != null);
				this.example = value.Trim();
                RaisePropertyChangedEvent("Example");
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public IStatistics Statistics
        {
            get
            {
                if (this.statistics == null)
                {
                    IVocabulary voc = this.Vocabulary;
                    IUserProfile profile = voc.UserProfile;
                    this.statistics = profile.GetStatistics(this);
                }
                return this.statistics;
            }
        }

        #endregion

        #region Serialization

		public override void PrepareLoad()
		{
			base.PrepareLoad();
			this.categories.PrepareLoad();
			this.translations.PrepareLoad();
			this.synonyms.PrepareLoad();
			this.antonyms.PrepareLoad();
		}

        public override void Load(BinaryReader reader)
        {
			byte[] gbytes = reader.ReadBytes(16);
			this.guid = new Guid(gbytes);
			this.categories.Load(reader);
			this.prefix = reader.ReadString();
			this.postfix = reader.ReadString();
            this.translations.Load(reader);
            this.synonyms.Load(reader);
            this.antonyms.Load(reader);
            this.definition = reader.ReadString();
            this.example = reader.ReadString();
        }

        public override void FinalizeLoad()
        {
			this.categories.FinalizeLoad();
			this.translations.FinalizeLoad();
            this.synonyms.FinalizeLoad();
            this.antonyms.FinalizeLoad();
			base.FinalizeLoad();
        }

		public override void PrepareSave()
		{
			base.PrepareSave();
			this.categories.PrepareSave();
			this.translations.PrepareSave();
			this.synonyms.PrepareSave();
			this.antonyms.PrepareSave();
		}

        public override void Save(BinaryWriter writer)
        {
			byte[] gbytes = this.guid.ToByteArray();
			writer.Write(gbytes);
            this.categories.Save(writer);
			writer.Write(this.prefix);
			writer.Write(this.postfix);
            this.translations.Save(writer);
            this.synonyms.Save(writer);
            this.antonyms.Save(writer);
            writer.Write(this.definition);
            writer.Write(this.example);
        }

		public override void FinalizeSave()
		{
			this.categories.FinalizeSave();
			this.translations.FinalizeSave();
			this.synonyms.FinalizeSave();
			this.antonyms.FinalizeSave();
			base.FinalizeSave();
		}

        #endregion
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    // Language
	[DebuggerDisplay("{InputLocale.NativeName}")]
    class LanguageImpl : VocabularyObject, ILanguage, IBinarySerializable
    {
        #region Fields

        private String cultureInfoName = CultureInfo.CurrentCulture.Name;

        #endregion

		#region Language

		internal LanguageImpl(VocabularyImpl vocabulary)
			: base(vocabulary)
        {
			this.Parent = vocabulary;
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public CultureInfo InputLocale
        {
            get
            {
                CultureInfo ci = null;
				if (!StringUtils.IsEmpty(this.cultureInfoName))
				{
					try
					{
						ci = CultureInfo.GetCultureInfo(this.cultureInfoName);
					}
					catch
					{
					}
				}
                return ci;
            }
            set
            {
                Debug.Assert(value != null);
                this.cultureInfoName = value.Name;
                RaisePropertyChangedEvent("InputLocale");
            }
        }

        #endregion

        #region Serialization

        public override void Load(BinaryReader reader)
        {
            this.cultureInfoName = reader.ReadString();
        }

        public override void Save(BinaryWriter writer)
        {
            writer.Write(this.cultureInfoName);
        }

        #endregion
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    // Vocabulary
	[DebuggerDisplay("{Name}")]
    class VocabularyImpl : VocabularyObject, IVocabulary, IBinarySerializable
    {
        #region Fields

        private String name = String.Empty;
        private String description = String.Empty;
        private UniqueStringCollectionImpl wordTypes;
        private UniqueStringCollectionImpl categories;
        private LanguageImpl targetLangauge;
        private LanguageImpl nativeLanguage;
        private VocabularyWordCollectionImpl words;
		private bool isModified = true;
        private IUserProfile profile;

        #endregion

		#region VocabularyObject

        [System.Reflection.Obfuscation(Exclude = true)]
		public override IVocabulary Vocabulary
		{
			get
			{
				return this;
			}
		}

		private void Modify(bool isModified)
		{
			if (this.isModified ^ isModified)
			{
				this.isModified = isModified;
				RaisePropertyChangedEvent("IsModified");
			}
		}

		protected internal override void OnPropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			if (sender != this || (e.PropertyName != "IsModified" && e.PropertyName != "UserProfile"))
			{
				Modify(true);
			}
		}

		#endregion

		#region IVocabulary

		internal VocabularyImpl()
		{
			this.nativeLanguage = new LanguageImpl(this);
			this.targetLangauge = new LanguageImpl(this);
            this.wordTypes = new UniqueStringCollectionImpl();
            this.wordTypes.CollectionChanged += new NotifyCollectionChangedEventHandler(new NotifyCollectionPropertyChangedWrapper(this, "WordTypes").CollectionChanged);
            this.categories = new UniqueStringCollectionImpl();
            this.categories.CollectionChanged += new NotifyCollectionChangedEventHandler(new NotifyCollectionPropertyChangedWrapper(this, "Categories").CollectionChanged);
            this.words = new VocabularyWordCollectionImpl(this);
            this.words.CollectionChanged += new NotifyCollectionChangedEventHandler(new NotifyCollectionPropertyChangedWrapper(this, "Words").CollectionChanged);
			this.Modify(false);
		}

        public IWord CreateWord()
        {
            return new WordImpl(this);
        }

		public IMeaning CreateMeaning()
		{
			return new MeaningImpl(this);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public String Name
		{
			get
			{
				return this.name;
			}
			set
			{
				Debug.Assert(value != null);
				this.name = value.Trim();
                RaisePropertyChangedEvent("Name");
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public String Description
		{
			get
			{
				return this.description;
			}
			set
			{
				Debug.Assert(value != null);
				this.description = value.Trim();
                RaisePropertyChangedEvent("Description");
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public StringCollection WordTypes
        {
            get
            {
                return this.wordTypes;
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public StringCollection Categories
        {
            get
            {
                return this.categories;
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public ILanguage NativeLanguage
		{
			get
			{
				return this.nativeLanguage;
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public ILanguage TargetLanguage
		{
			get
			{
				return this.targetLangauge;
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public WordCollection Words 
		{
			get
			{
				return this.words;
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public bool IsModified
		{
			get
			{
				return this.isModified;
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public IUserProfile UserProfile
        {
            get
            {
                return this.profile;
            }
            set
            {
                if (this.profile != value)
                {
                    this.profile = value;
                    RaisePropertyChangedEvent("UserProfile");
                }
            }
        }


        #endregion

        #region Serialization

		public override void PrepareLoad()
		{
			base.PrepareLoad();
			this.targetLangauge.PrepareLoad();
			this.nativeLanguage.PrepareLoad();
			this.wordTypes.PrepareLoad();
			this.categories.PrepareLoad();
			this.words.PrepareLoad();
		}

        public override void Load(BinaryReader reader)
        {
            Load(reader, null);
        }

        public override void Load(BinaryReader reader, IProgressReport progress)
        {
            this.name = reader.ReadString();
            this.description = reader.ReadString();
            this.targetLangauge.Load(reader);
            this.nativeLanguage.Load(reader);
            this.wordTypes.Load(reader);
            this.categories.Load(reader);
            this.words.Load(reader, progress);
        }

        public override void FinalizeLoad()
        {
			this.targetLangauge.FinalizeLoad();
			this.nativeLanguage.FinalizeLoad();
			this.wordTypes.FinalizeLoad();
			this.categories.FinalizeLoad();
            this.words.FinalizeLoad();
			base.FinalizeLoad();
        }

		public override void PrepareSave()
		{
			base.PrepareSave();
			this.targetLangauge.PrepareSave();
			this.nativeLanguage.PrepareSave();
			this.wordTypes.PrepareSave();
			this.categories.PrepareSave();
			this.words.PrepareSave();
		}

        public override void Save(BinaryWriter writer)
        {
            Save(writer, null);
        }

        public override void Save(BinaryWriter writer, IProgressReport progress)
        {
            writer.Write(this.name);
            writer.Write(this.description);
            this.targetLangauge.Save(writer);
            this.nativeLanguage.Save(writer);
            this.wordTypes.Save(writer);
            this.categories.Save(writer);
            this.words.Save(writer, progress);
        }

		public override void FinalizeSave()
		{
			this.targetLangauge.FinalizeSave();
			this.nativeLanguage.FinalizeSave();
			this.wordTypes.FinalizeSave();
			this.categories.FinalizeSave();
			this.words.FinalizeSave();
			base.FinalizeSave();
		}

		public void Load(String source, IProgressReport progress)
		{
			Debug.Assert(File.Exists(source));
            using (FileStream stream = File.OpenRead(source))
			{
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    String tag = reader.ReadString();
                    if (tag != StringRes.VOCABULARY_TAG)
                    {
                        throw new FileFormatException(source, StringRes.NOT_VOCABULARY_FILE);
                    }

                    String version = reader.ReadString();
                    if (version != StringRes.VOCABULARY_VERSION)
                    {
                        throw new FileFormatException(source, String.Format(StringRes.VERSION_NOT_SUPPORTED, version));
                    }

                    using (XBinaryReader xreader = new XBinaryReader(stream))
                    {
                        PrepareLoad();
                        Load(xreader, progress);
                        FinalizeLoad();
                    }
                }
			}
			Modify(false);
		}

        public void Save(String target, IProgressReport progress)
		{
			using (FileStream stream = new FileStream(target, FileMode.OpenOrCreate, FileAccess.Write))
			{
				using (BinaryWriter writer = new BinaryWriter(stream))
				{
					writer.Write(StringRes.VOCABULARY_TAG);
                    writer.Write(StringRes.VOCABULARY_VERSION);
					using (XBinaryWriter xwriter = new XBinaryWriter(stream))
					{
                        PrepareSave();
                        Save(xwriter, progress);
                        FinalizeSave();
					}
				}
			}
			Modify(false);
        }

        #endregion
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    // UniqueStringCollectionImpl
    // - unique, unsorted, small collection of strings
    class UniqueStringCollectionImpl : StringCollection, IBinarySerializable
	{
		#region Collection

		private void VerifyDuplicateItem(String item)
        {
            if (this.IndexOf(item) >= 0)
            {
                throw new DuplicateStringException(item);
            }
        }

		private void VarifyBlank(String item)
		{
			if (StringUtils.IsEmpty(item))
			{
				throw new BlankStringException();
			}
		}

        protected override void InsertItem(int index, String item)
        {
			item = item.Trim();
			VarifyBlank(item);
			VerifyDuplicateItem(item);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, String item)
        {
			item = item.Trim();
			VarifyBlank(item);
            if (!this[index].Equals(item))
            {
                VerifyDuplicateItem(item);
                base.SetItem(index, item);
            }
        }

        #endregion

        #region Serialization

        public override void Load(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                String str = reader.ReadString();
                this.Items.Add(str);
            }
        }

        public override void Save(BinaryWriter writer)
        {
            writer.Write(this.Items.Count);
            foreach (String str in this.Items)
            {
                writer.Write(str);
            }
        }

        #endregion
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    // WordColllectionImpl
    // - unique, unsorted, small collection of Words
    class WordCollectionImpl : WordCollection, IBinarySerializable
    {
        #region Fields

        private MeaningImpl meaning = null;

        #endregion

        #region Collection

        internal WordCollectionImpl(MeaningImpl meaning)
        {
            Debug.Assert(meaning != null);
            this.meaning = meaning;
			this.meaning.Vocabulary.Words.CollectionChanged += new NotifyCollectionChangedEventHandler(Words_CollectionChanged);
        }

		private void Words_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
			{
				foreach (IWord word in e.OldItems)
				{
					Remove(word);
				}
			}
		}

        public override int IndexOf(IWord item)
        {
            return (this as ObservableCollection<IWord>).IndexOf(item);
        }

		public override int IndexOf(String type, String prefix, String data)
        {
            throw new NotImplementedException();
        }

		public override int IndexOf(String prefix, String data)
		{
			throw new NotImplementedException();
		}

        public override bool Remove(IWord item)
        {
            return (this as ObservableCollection<IWord>).Remove(item);
        }

        public override bool Contains(IWord item)
        {
            return (this as ObservableCollection<IWord>).Contains(item);
        }

        private void VerifySameVocabulary(IWord item)
        {
            Debug.Assert(item.Vocabulary == this.meaning.Vocabulary);
            if (item.Vocabulary != this.meaning.Vocabulary)
            {
                throw new InternalDataException();
            }
        }

        private void VerifyDuplicateItem(IWord item)
        {
            if (this.IndexOf(item) >= 0)
            {
                throw new DuplicateWordException(item);
            }
        }

        protected override void InsertItem(int index, IWord item)
        {
            VerifySameVocabulary(item);
            VerifyDuplicateItem(item);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, IWord item)
        {
            if (!this[index].Equals(item))
            {
                VerifySameVocabulary(item);
                VerifyDuplicateItem(item);
                base.SetItem(index, item);
            }
        }

        #endregion

        #region Serialization

        public override void Load(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                String type = reader.ReadString();
                String prefix = reader.ReadString();
                String data = reader.ReadString();
                VocabularyWordCollectionImpl words = (VocabularyWordCollectionImpl)meaning.Vocabulary.Words;
                IWord wd = new WordDesc(type, prefix, data);
                this.Items.Add(wd);
            }
        }

        public override void FinalizeLoad()
        {
            IVocabulary vocabulary = this.meaning.Vocabulary;
            for (int i = 0; i < this.Count; ++i)
            {
                IWord wd = this[i];
                int index = vocabulary.Words.IndexOf(wd);
                Debug.Assert(index >= 0);
				if (index >= 0)
				{
					IWord word = vocabulary.Words[index];
                    this.Items[i] = word;
				}
            }
        }

        public override void Save(BinaryWriter writer)
        {
            writer.Write(this.Items.Count);
            foreach (WordImpl word in this.Items)
            {
                writer.Write(word.Type);
                writer.Write(word.Prefix);
                writer.Write(word.Data);
            }
        }

        #endregion
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    // VocabularyWordCollectionImpl
    // - unique, sorted, big collection of Words
    class VocabularyWordCollectionImpl : WordCollection, IBinarySerializable
    {
        #region Fields

        private VocabularyImpl vocabulary = null;

        #endregion

        #region Collection

        internal VocabularyWordCollectionImpl(VocabularyImpl vocabulary)
        {
            Debug.Assert(vocabulary != null);
            this.vocabulary = vocabulary;
        }

		public override int IndexOf(String prefix, String data)
		{
			Debug.Assert(prefix != null);
			Debug.Assert(data != null);
			prefix = prefix.Trim();
			data = data.Trim();
			Debug.Assert(data.Length > 0);
			WordDesc wd = new WordDesc(prefix, data);
			return IndexOf(wd);
		}

		public override int IndexOf(String type, String prefix, String data)
        {
            Debug.Assert(type != null);
			Debug.Assert(prefix != null);
			Debug.Assert(data != null);
            type = type.Trim();
			prefix = prefix.Trim();
			data = data.Trim();
            Debug.Assert(type.Length > 0);
			Debug.Assert(data.Length > 0);
            WordDesc wd = new WordDesc(type, prefix, data);
            return IndexOf(wd);
        }

        public override int IndexOf(IWord item)
        {
            Debug.Assert(this.Items is List<IWord>);
            return this.Items.BinarySearch(item);
        }

        public override bool Remove(IWord item)
        {
            Debug.Assert(this.Items is List<IWord>);
			VerifySameVocabulary(item);
            bool removed = false;
            int index = this.Items.BinarySearch(item);
            if (index >= 0 || this.Items[index].Equals(item))
            {
                this.RemoveAt(index);
                (item as INotifyPropertyChanged).PropertyChanged -= new PropertyChangedEventHandler(OnWordPropertyChanged);
                (item as INotifyPropertyChanging).PropertyChanging -= new PropertyChangingEventHandler(OnWordPropertyChanging);
				(item as WordImpl).Parent = null;
                removed = true;
            }
            return removed;
        }

        public override bool Contains(IWord item)
        {
            Debug.Assert(this.Items is List<IWord>);
            return (this.Items.BinarySearch(item) >= 0);
        }

        protected new List<IWord> Items
        {
            get
            {
                return (base.Items as List<IWord>);
            }
        }

        private void VerifySameVocabulary(IWord item)
        {
            Debug.Assert(item.Vocabulary == this.vocabulary);
            if (item.Vocabulary != this.vocabulary)
            {
                throw new InternalDataException();
            }
        }

        protected override void InsertItem(int index, IWord item)
        {
            VerifySameVocabulary(item);
            Debug.Assert(this.Items != null);
            index = this.Items.BinarySearch(item);
            base.InsertItem(~index, item);
            (item as INotifyPropertyChanging).PropertyChanging += new PropertyChangingEventHandler(OnWordPropertyChanging);
            (item as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(OnWordPropertyChanged);
			(item as WordImpl).Parent = this.vocabulary;
        }

        protected void OnWordPropertyChanging(Object sender, PropertyChangingEventArgs e)
        {
            IWord word = (IWord)sender;
            int index = this.Items.BinarySearch(word);
            if (index >= 0 || this.Items[index].Equals(word))
            {
                this.RemoveAt(index);
            }
        }

        protected void OnWordPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            IWord word = (IWord)sender;
            int index = this.Items.BinarySearch(word);
			if (index < 0)
			{
				base.InsertItem(~index, word);
			}
        }

        protected override void SetItem(int index, IWord item)
        {
            throw new InvalidOperationException();
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            throw new InvalidOperationException();
        }

        #endregion

        #region Serialization

        public override void Load(BinaryReader reader)
        {
            Load(reader, null);
        }

        public override void Load(BinaryReader reader, IProgressReport progress)
        {
            int count = reader.ReadInt32();
            ProgressReportHelper.Report(progress, ProgressType.Start, count);
            for (int i = 0; i < count; ++i)
            {
                WordImpl word = (WordImpl)this.vocabulary.CreateWord();
                word.PrepareLoad();
                word.Load(reader);
                this.Items.Add(word);
                word.Parent = this.vocabulary;
                ProgressReportHelper.Report(progress, ProgressType.Status, i);
            }
            this.Items.Sort();
            ProgressReportHelper.Report(progress, ProgressType.End, 0);
        }

        public override void FinalizeLoad()
        {
            foreach (WordImpl word in this.Items)
            {
                word.FinalizeLoad();
                word.PropertyChanging += new PropertyChangingEventHandler(OnWordPropertyChanging);
                word.PropertyChanged += new PropertyChangedEventHandler(OnWordPropertyChanged);
            }
        }

        public override void Save(BinaryWriter writer)
        {
            Save(writer, null);
        }

        public override void Save(BinaryWriter writer, IProgressReport progress)
        {
            ProgressReportHelper.Report(progress, ProgressType.Start, this.Items.Count);
            int i = 0;
            writer.Write(this.Items.Count);
            foreach (WordImpl word in this.Items)
            {
                word.Save(writer);
                ProgressReportHelper.Report(progress, ProgressType.Status, ++i);
            }
            ProgressReportHelper.Report(progress, ProgressType.End, 0);
        }

        #endregion
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    // MeaningCollection
    // - unique, unsorted, small collection of Meanings
    class MeaningCollectionImpl : MeaningCollection, IBinarySerializable
    {
        #region Fields

        private WordImpl word = null;

        #endregion

        #region Collection

        internal MeaningCollectionImpl(WordImpl word)
        {
            Debug.Assert(word != null);
            this.word = word;
        }

        private void VerifyDuplicateItem(IMeaning item)
        {
            Debug.Assert(this.IndexOf(item) < 0);
            if (this.IndexOf(item) >= 0)
            {
                throw new InternalDataException();
            }
        }

        private void VerifySameVocabulary(IMeaning item)
        {
            Debug.Assert(item.Vocabulary == this.word.Vocabulary);
            if (item.Vocabulary != this.word.Vocabulary)
            {
                throw new InternalDataException();
            }
        }

        protected override void InsertItem(int index, IMeaning item)
        {
            VerifySameVocabulary(item);
            VerifyDuplicateItem(item);
            this.CollectionChanged += new NotifyCollectionChangedEventHandler((item as MeaningImpl).OnMeaningsCollectionChanged);
            base.InsertItem(index, item);
			(item as VocabularyObject).Parent = this.word;
        }

        protected override void RemoveItem(int index)
        {
            IMeaning item = this[index];
            base.RemoveItem(index);
			this.CollectionChanged -= new NotifyCollectionChangedEventHandler((item as MeaningImpl).OnMeaningsCollectionChanged);
			(item as VocabularyObject).Parent = null;
        }

        protected override void SetItem(int index, IMeaning item)
        {
            throw new InvalidOperationException();
        }

        #endregion

        #region Serialization

        public override void Load(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                MeaningImpl meaning = (MeaningImpl)this.word.Vocabulary.CreateMeaning();
				meaning.PrepareLoad();
                meaning.Load(reader);
                this.Items.Add(meaning);
                this.CollectionChanged += new NotifyCollectionChangedEventHandler(meaning.OnMeaningsCollectionChanged);
				meaning.Parent = this.word;
            }
        }

        public override void FinalizeLoad()
        {
            foreach (MeaningImpl meaning in this.Items)
            {
                meaning.FinalizeLoad();
            }
        }

        public override void Save(BinaryWriter writer)
        {
            writer.Write(this.Items.Count);
            foreach (MeaningImpl meaning in this.Items)
            {
                meaning.Save(writer);
            }
        }

        #endregion

    }

    //////////////////////////////////////////////////////////////////////////////////////////
    // Statistics implementation
	internal class StatisticsImpl : NotifyPropertyChangedImpl, IStatistics, IComparable, IBinarySerializable
    {
        #region Fields

        private int score;

        private UserProfileImpl profile;

		private Guid guid = Guid.Empty;

		private DateTime modified = DateTime.MinValue;

		private List<DateTime> rightAnswers;
		private List<DateTime> wrongAnswers;
		private List<DateTime> promptAnswers;

		#endregion

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public IUserProfile UserProfile
		{
			get
			{
				return this.profile;
			}
		}


        #region Statistics

		internal StatisticsImpl(UserProfileImpl profile, Guid guid)
        {
            Debug.Assert(profile != null);
            this.score = int.MinValue;
			this.profile = profile;
			this.guid = guid;
			this.rightAnswers = new List<DateTime>();
			this.wrongAnswers = new List<DateTime>();
			this.promptAnswers = new List<DateTime>();
        }

        [System.Reflection.Obfuscation(Exclude = true)]
		public DateTime Modified
		{
			get
			{
                return this.modified.ToLocalTime();
			}
		}

		public void RightAnswer()
		{
			this.rightAnswers.Add(DateTime.UtcNow);
            this.score = int.MinValue;
			RaisePropertyChangedEvent("Score");
		}

		public void WrongAnswer()
		{
			this.wrongAnswers.Add(DateTime.UtcNow);
            this.score = int.MinValue;
            RaisePropertyChangedEvent("Score");
		}

		public void PromptAnswer()
		{
			this.promptAnswers.Add(DateTime.UtcNow);
            this.score = int.MinValue;
			RaisePropertyChangedEvent("Score");
		}

		public void Modify()
		{
			this.modified = DateTime.UtcNow;
            this.score = int.MinValue;
            RaisePropertyChangedEvent("Modified");
            RaisePropertyChangedEvent("Score");
		}

		public IEnumerable EnumerateRightAnswers()
		{
			return this.rightAnswers;
		}

		public IEnumerable EnumerateWrongAnswers()
		{
			return this.wrongAnswers;
		}

		public IEnumerable EnumeratePromptAnswers()
		{
			return this.promptAnswers;
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public int Score
		{
			get
			{
                if (this.score == int.MinValue)
                {
                    this.score = CalculateScore();
                }
                return this.score;
			}
		}

		private int CalculateScore(DateTime time, DateTime now, int weight)
		{
			TimeSpan span = now - time;
			return weight / (span.Days + 1);
		}

		private int CalculateScore(List<DateTime> times, DateTime now, int weight)
		{
			int score = 0;
			foreach (DateTime time in times)
			{
				score += CalculateScore(time, now, weight);
			}
			return score;
		}

		protected int CalculateScore()
		{
			const int initial = 10 * 365;
			const int right = 1 * 365;
			const int wrong = 3 * 365;
			const int prompt = 5 * 365;
			DateTime now = DateTime.Now;
			int score = CalculateScore(this.modified, now, initial)
					  - CalculateScore(this.rightAnswers, now, right)
					  + CalculateScore(this.wrongAnswers, now, wrong)
					  + CalculateScore(this.promptAnswers, now, prompt);
			return score;
		}

        #endregion

        #region IComparable

        public int CompareTo(object obj)
        {
            int result = 0;
			StatisticsImpl statisticsImpl = (obj as StatisticsImpl);
            Debug.Assert(statisticsImpl != null);
			if (statisticsImpl != null)
			{
				result = this.guid.CompareTo(statisticsImpl.Guid);
			}
			return result;
        }

        #endregion

        #region Serialization

		public void PrepareLoad()
		{
			;
		}
		
        public void Load(BinaryReader reader)
        {
			this.guid = new Guid(reader.ReadBytes(16));
			this.modified = new DateTime(reader.ReadInt64());
			ReadDateTimeList(reader, this.rightAnswers);
			ReadDateTimeList(reader, this.wrongAnswers);
			ReadDateTimeList(reader, this.promptAnswers);
        }

        public void Load(BinaryReader reader, IProgressReport progress)
        {
            Load(reader);
        }

		public void FinalizeLoad()
		{
			;
		}

		public void PrepareSave()
		{
			;
		}

        public void Save(BinaryWriter writer)
        {
			writer.Write(this.guid.ToByteArray());
			writer.Write(this.modified.Ticks);
			WriteDateTimeList(writer, this.rightAnswers);
			WriteDateTimeList(writer, this.wrongAnswers);
			WriteDateTimeList(writer, this.promptAnswers);
        }

        public void Save(BinaryWriter writer, IProgressReport progress)
        {
            Save(writer);
        }

		public void FinalizeSave()
		{
			;
		}

		private void WriteDateTimeList(BinaryWriter writer, List<DateTime> list)
		{
			writer.Write(list.Count);
			foreach (DateTime dt in list)
			{
				writer.Write(dt.Ticks);
			}
		}

		private void ReadDateTimeList(BinaryReader reader, List<DateTime> list)
		{
			int count = reader.ReadInt32();
			for (int i = 0; i < count; ++i)
			{
                DateTime dt = new DateTime(reader.ReadInt64());
                // DateTime dt = DateTime.FromBinary(reader.ReadInt64());
				list.Add(dt);
			}
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////
	// StatisticsStore implementation
	internal class UserProfileImpl : NotifyPropertyChangedImpl, IUserProfile, IBinarySerializable, IEnumerable
    {
        #region Fields

        private String name = String.Empty;

        private DateTime created;
        private DateTime saved;
		private String defaultVocabulary = String.Empty;
		private int sleepInterval = 10;
		private bool beep = true;

        private ArrayList statistics = new ArrayList();

        #endregion

		public IEnumerator GetEnumerator()
		{
			return this.statistics.GetEnumerator();
		}

        #region IUserProfile

        public UserProfileImpl()
        {
            DateTime now = DateTime.Now;
            this.created = now;
            this.saved = now;
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
				Debug.Assert(value != null);
                this.name = value.Trim();
				RaisePropertyChangedEvent("Name");
            }
        }

		public String DefaultVocabulary
		{
			get
			{
				return this.defaultVocabulary;
			}

			set
			{
				Debug.Assert(value != null);
				this.defaultVocabulary = value.Trim();
				RaisePropertyChangedEvent("DefaultVocabulary");
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public int SleepInterval
		{
			get
			{
				return this.sleepInterval;
			}
			set
			{
				this.sleepInterval = value;
				RaisePropertyChangedEvent("SleepInterval");
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public bool Beep
		{
			get
			{
				return this.beep;
			}
			set
			{
				this.beep = value;
				RaisePropertyChangedEvent("Beep");
			}
		}

        class Statistics_vs_MeaningComparer : IComparer
        {
            private Guid Object2Guid(Object x)
            {
                StatisticsImpl statistics = x as StatisticsImpl;
                if (statistics != null)
                {
                    return statistics.Guid;
                }

                MeaningImpl meaning = x as MeaningImpl;
                if (meaning != null)
                {
                    return meaning.Guid;
                }

                Debug.Assert(false);
                return Guid.Empty;
            }

            public int Compare(Object x, Object y)
            {
                Guid guidX = Object2Guid(x);
                Guid guidY = Object2Guid(y);
                return guidX.CompareTo(guidY);
            }
        }

        public IStatistics GetStatistics(IMeaning meaning)
        {
            MeaningImpl meaningImpl = (MeaningImpl)meaning;
            StatisticsImpl statistics = null;
            int index = this.statistics.BinarySearch(0, this.statistics.Count, meaningImpl, new Statistics_vs_MeaningComparer());
            if (index < 0)
            {
                statistics = new StatisticsImpl(this, meaningImpl.Guid);
                this.statistics.Insert(~index, statistics);
            }
            else
            {
                statistics = (StatisticsImpl)this.statistics[index];
            }
            return statistics;
        }

		public IStatistics this[IMeaning meaning]
        {
            get
            {
                return this.GetStatistics(meaning);
            }
        }

        public DateTime Created
        {
            get
            {
                return this.created;
            }
        }

        public DateTime Saved
        {
            get
            {
                return this.saved;
            }
        }

        #endregion

        #region Serialization

        public void Load(String source, IProgressReport progress)
        {
            Debug.Assert(File.Exists(source));
            using (FileStream stream = File.OpenRead(source))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    String tag = reader.ReadString();
                    if (tag != StringRes.USER_PROFILE_TAG)
                    {
                        throw new FileFormatException(source, StringRes.NOT_USER_STATISTICS_FILE);
                    }

                    String version = reader.ReadString();
                    if (version != StringRes.USER_PROFILE_VERSION)
                    {
                        throw new FileFormatException(source, String.Format(StringRes.VERSION_NOT_SUPPORTED, version));
                    }

                    using (XBinaryReader xreader = new XBinaryReader(stream))
                    {
                        Load(xreader, progress);
                    }
                }
            }
        }

        public void Save(String target, IProgressReport progress)
        {
            using (FileStream stream = new FileStream(target, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(StringRes.USER_PROFILE_TAG);
                    writer.Write(StringRes.USER_PROFILE_VERSION);
                    using (XBinaryWriter xwriter = new XBinaryWriter(stream))
                    {
                        Save(xwriter, progress);
                    }
                }
            }
        }

		public void PrepareLoad()
		{
		}

        public void Load(BinaryReader reader)
        {
            Load(reader, null);
        }

        public void Load(BinaryReader reader, IProgressReport progress)
        {
            this.name = reader.ReadString();

            this.created = new DateTime(reader.ReadInt64());
            this.saved = new DateTime(reader.ReadInt64());

            this.defaultVocabulary = reader.ReadString();
            this.sleepInterval = reader.ReadInt32();
            this.beep = reader.ReadBoolean();

            int count = reader.ReadInt32();
            ProgressReportHelper.Report(progress, ProgressType.Start, count);
            for (int i = 0; i < count; ++i)
            {
                StatisticsImpl statistics = new StatisticsImpl(this, Guid.Empty);
                statistics.Load(reader);
                this.statistics.Add(statistics);
                ProgressReportHelper.Report(progress, ProgressType.Status, i);
            }
            this.statistics.Sort();

            ProgressReportHelper.Report(progress, ProgressType.End, 0);
        }

		public void FinalizeLoad()
		{
		}

		public void PrepareSave()
		{
		}

        public void Save(BinaryWriter writer)
        {
            Save(writer, null);
        }

        public void Save(BinaryWriter writer, IProgressReport progress)
        {
            this.saved = DateTime.Now;

            writer.Write(this.name);

            writer.Write(this.created.Ticks);
            writer.Write(this.saved.Ticks);

            writer.Write(this.defaultVocabulary);
            writer.Write(this.sleepInterval);
            writer.Write(this.beep);

            ProgressReportHelper.Report(progress, ProgressType.Start, this.statistics.Count);
            int i = 0;

            writer.Write(this.statistics.Count);
            foreach (StatisticsImpl statistics in this.statistics)
            {
                statistics.Save(writer);
                ProgressReportHelper.Report(progress, ProgressType.Status, i++);
            }

            ProgressReportHelper.Report(progress, ProgressType.Start, 0);
        }

		public void FinalizeSave()
		{
		}

        #endregion
    }
}
