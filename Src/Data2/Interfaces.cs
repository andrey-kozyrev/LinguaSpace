using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics;
using System.IO;
using LinguaSpace.Common;

namespace LinguaSpace.Data
{
	public interface IBinarySerializable
	{
		void PrepareLoad();
		void Load(BinaryReader reader);
        void Load(BinaryReader reader, IProgressReport progress);
		void FinalizeLoad();
		void PrepareSave();
		void Save(BinaryWriter writer);
        void Save(BinaryWriter writer, IProgressReport progress);
		void FinalizeSave();
	}

	public interface IVocabularyObject
	{
		IVocabulary Vocabulary
		{
			get;
		}

		IVocabularyObject Parent
		{
			get;
		}
	}

    //////////////////////////////////////////////////////////////////////////////////////////
    // Word
    public interface IWord : IVocabularyObject
    {
        String Type
        {
            get;
            set;
        }

        String Prefix
        {
            get;
            set;
        }

        String Data
        {
            get;
            set;
        }

        String Text
        {
            get;
        }

        MeaningCollection Meanings
        {
            get;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    // Meaning
    public interface IMeaning : IVocabularyObject
    {
		Guid Guid
		{
			get;
		}
    
		IWord Word
		{
			get;
		}

		String Prefix
		{
			get;
			set;
		}

		String Postfix
		{
			get;
			set;
		}

		String Usage
		{
			get;
		}

        int Index
        {
            get;
        }

        StringCollection Categories
        {
            get;
        }

        StringCollection Translations
        {
            get;
        }

        WordCollection Synonyms
        {
            get;
        }

        WordCollection Antonyms
        {
            get;
        }

        String Definition
        {
            get;
            set;
        }

        String Example
        {
            get;
            set;
        }

        IStatistics Statistics
        {
            get;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    // Language
    public interface ILanguage : IVocabularyObject
    {
        CultureInfo InputLocale
        {
            get;
            set;
        }
    }

	//////////////////////////////////////////////////////////////////////////////////////////
	// Vocabulary
    public interface IVocabulary
    {
        IWord CreateWord();
		IMeaning CreateMeaning();

        String Name
        {
            get;
            set;
        }

        String Description
        {
            get;
            set;
        }

        StringCollection WordTypes
        {
            get;
        }

        StringCollection Categories
        {
            get;
        }

        ILanguage TargetLanguage
        {
            get;
        }

        ILanguage NativeLanguage
        {
            get;
        }

        WordCollection Words
        {
            get;
        }

        void Load(String source, IProgressReport progress);

        void Save(String source, IProgressReport progress);

		bool IsModified
		{
			get;
		}

        IUserProfile UserProfile
        {
            get;
            set;
        }
    }

    public abstract class VocabularyCollection<T> : ObservableCollection<T>, IBinarySerializable
    {
        #region IBinarySerializable

        public virtual void PrepareLoad()
        {
            ;
        }

        public abstract void Load(BinaryReader reader);

        public virtual void Load(BinaryReader reader, IProgressReport progress)
        {
            Load(reader);
        }

        public virtual void FinalizeLoad()
        {
            ;
        }

        public virtual void PrepareSave()
        {
            ;
        }

        public abstract void Save(BinaryWriter writer);

        public virtual void Save(BinaryWriter writer, IProgressReport progress)
        {
            Save(writer);
        }
        
        public virtual void FinalizeSave()
        {
            ;
        }

        #endregion

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (T v in this.Items)
			{
				if (sb.Length > 0)
				{
					sb.Append(", ");
				}
				sb.Append(v.ToString());

				if (sb.Length > 100)
				{
					break;
				}
			}
			return sb.ToString();
		}
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    // StringCollection
    public abstract class StringCollection : VocabularyCollection<String>
    {
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    // WordCollection
    public abstract class WordCollection : VocabularyCollection<IWord>
    {
		public abstract int IndexOf(String prefix, String data);
		public abstract int IndexOf(String type, String prefix, String data);
        public abstract new int IndexOf(IWord item);
        public abstract new bool Remove(IWord item);
        public abstract new bool Contains(IWord item);
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    // MeaningCollection
    public abstract class MeaningCollection : VocabularyCollection<IMeaning>
    {
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    // Statistics
    public interface IStatistics
    {
		Guid Guid
		{
			get;
		}
    
        DateTime Modified
        {
            get;
        }

		int Score
		{
			get;
		}

		void RightAnswer();
		void WrongAnswer();
		void PromptAnswer();
		void Modify();
		
		IEnumerable EnumerateRightAnswers();
		IEnumerable EnumerateWrongAnswers();
		IEnumerable EnumeratePromptAnswers();
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    // StatisticsStore
    public interface IUserProfile
    {
        String Name
        {
            get;
            set;
        }

		String DefaultVocabulary
		{
			get;
			set;
		}

		int SleepInterval
		{
			get;
			set;
		}

		bool Beep
		{
			get;
			set;
		}

        IStatistics this[IMeaning meaning]
        {
            get;
        }

        IStatistics GetStatistics(IMeaning meaning);

        DateTime Created
        {
            get;
        }

        DateTime Saved
        {
            get;
        }

        void Load(String source, IProgressReport progress);
        void Save(String target, IProgressReport progress);
    }

    public class UserProfileFactory
    {
        public IUserProfile CreateUserProfile()
        {
            return new UserProfileImpl();
        }
    }

	public class VocabularyFactory
	{
		public IVocabulary CreateVocabulary()
		{
			return new VocabularyImpl();
		}
	}
}
