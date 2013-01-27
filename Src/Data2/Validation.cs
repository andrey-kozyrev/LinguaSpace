using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Specialized;
using LinguaSpace.Common;


namespace LinguaSpace.Data
{
	[DebuggerDisplay("{Text}")]
	public class TextData : NotifyPropertyChangedImpl
	{
		private String text = String.Empty;

        [System.Reflection.Obfuscation(Exclude = true)]
        public String Text
		{
			get
			{
				return this.text;
			}
			set
			{
				Debug.Assert(value != null);
				this.text = value.Trim();
				RaisePropertyChangedEvent("Text");
			}
		}
	}

    public class LookupData : NotifyPropertyChangedImpl
    {
        protected String text = String.Empty;
        protected Object data;

        public LookupData(String text, Object data)
        {
            Debug.Assert(text != null);
            Debug.Assert(data != null);
            this.text = text;
            this.data = data;
        }

        public LookupData(Object data)
            : this(data.ToString(), data)
        {
        }

        public LookupData()
        {
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public String Text
        {
            get
            {
                return this.text;
            }
            set
            {
                Debug.Assert(value != null);
                this.text = value;
                RaisePropertyChangedEvent("Text");
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public Object Data 
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
                RaisePropertyChangedEvent("Data");
            }
        }
    }

    public class WordItem
    {
        private IWord word;

        public WordItem(IWord word)
        {
            Debug.Assert(word != null);
            this.word = word;
        }

        public WordItem()
        {
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public IWord Word
        {
            get
            {
                return this.word;
            }
        }

        public override string ToString()
        {
            return this.word.Data;
        }
    }

    public class TranslationItem
    {
        private String text;
        private IMeaning meaning;

        public TranslationItem(String text, IMeaning meaning)
        {
            Debug.Assert(text != null);
            Debug.Assert(meaning != null);
            this.text = text;
            this.meaning = meaning;
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public String Text
        {
            get
            {
                return this.text;
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public IMeaning Meaning
        {
            get
            {
                return this.meaning;
            }
        }

        public override string ToString()
        {
            return this.Text;
        }
    }

    public enum VocabularyUseCases
    {
        EditVocabularyProperties,
		FindWord,
        FindMeaning,
        InsertWord,
        EditWord,
        InsertMeaning,
        EditMeaning,
        InsertTranslation,
        EditTranslation,
        InsertSynonym,
        EditSynonym,
        InsertAntonym,
        EditAntonym,
		FindText,
        InsertText,
        EditText,
		InsertMeaningWord,
		EditMeaningWord
    }
	
    public class ValidatorFactory : IValidatorFactory
    {
        public IValidationStatusProvider CreateValidator(Object useCaseObj, params Object[] parameters)
        {
            Debug.Assert(useCaseObj is VocabularyUseCases);
            VocabularyUseCases useCase = (VocabularyUseCases)useCaseObj;
            IValidationStatusProvider vsp = null;
            switch (useCase)
            {
                case VocabularyUseCases.EditVocabularyProperties:
                    vsp = new EditVocabularyPropertiesValidator((IVocabulary)parameters[0]);
                    break;
				case VocabularyUseCases.FindWord:
					vsp = new FindWordLookupValidator((LookupData)parameters[0]);
					break;
                case VocabularyUseCases.InsertWord:
                    vsp = new InsertWordValidator((IWord)parameters[0]);
                    break;
                case VocabularyUseCases.EditWord:
                    vsp = new EditWordValidator((IWord)parameters[0], (IWord)parameters[1]);
                    break;
                case VocabularyUseCases.InsertMeaning:
                    vsp = new InsertMeaningValidator((IWord)parameters[0], (IMeaning)parameters[1]);
                    break;
                case VocabularyUseCases.EditMeaning:
					vsp = new EditMeaningValidator((IWord)parameters[0], (IMeaning)parameters[1], (IMeaning)parameters[2]);
                    break;
                case VocabularyUseCases.FindMeaning:
                    vsp = new FindMeaningLookupValidator((LookupData)parameters[0]);
                    break;
				case VocabularyUseCases.InsertText:
                    vsp = new InsertTextValidator((TextData)parameters[0], (ICollection)parameters[1], (String)parameters[2]);
                    break;
                case VocabularyUseCases.EditText:
					vsp = new EditTextValidator((TextData)parameters[0], (ICollection)parameters[1], (String)parameters[2]);
                    break;
                case VocabularyUseCases.FindText:
					vsp = new FindTextValidator((TextData)parameters[0]);
                    break;
				case VocabularyUseCases.InsertMeaningWord:
					vsp = new InsertMeaningWordLookupValidator((IWord)parameters[0], (IMeaning)parameters[1], (WordCollection)parameters[2], (LookupData)parameters[3]);
					break;
				case VocabularyUseCases.EditMeaningWord:
					vsp = new EditMeaningWordLookupValidator((IWord)parameters[0], (IMeaning)parameters[1], (WordCollection)parameters[2], (LookupData)parameters[3]);
					break;
            }
            return vsp;
        }
    }

	abstract class WordValidator : Validator
    {
        protected IWord word = null;

        protected WordValidator(IWord word)
        {
            Debug.Assert(word != null);
            this.word = word;
            Hook(word);
        }

        protected override void ValidateOverride()
        {
            base.ValidateOverride();
            if (this.IsValid)
            {
                if (StringUtils.IsEmpty(this.word.Text))
                {
                    SetStatus(false, "Text cannot be empty", ValidationMessageType.Error);
                }
                else if (StringUtils.IsEmpty(this.word.Type))
                {
                    SetStatus(false, "Type cannot be empty", ValidationMessageType.Error);
                }
            }
        }
    }

    class InsertWordValidator : WordValidator
    {
        protected internal InsertWordValidator(IWord word)
            : base(word)
        {
            Validate();
        }

        protected override void ValidateOverride()
        {
            base.ValidateOverride();
            if (this.IsValid)
            {
                if (this.word.Vocabulary.Words.IndexOf(this.word) >= 0)
                {
                    SetStatus(false, "This word already exists", ValidationMessageType.Error);
                }
            }
        }
    }

    class EditWordValidator : WordValidator
    {
        protected IWord wordOriginal = null;

        protected internal EditWordValidator(IWord word, IWord wordOriginal)
            : base(word)
        {
            Debug.Assert(wordOriginal != null);
            this.wordOriginal = wordOriginal;
            Validate();
        }

        protected override void ValidateOverride()
        {
            base.ValidateOverride();
            if (this.IsValid)
            {
                int index = this.word.Vocabulary.Words.IndexOf(this.word);
                if (index >= 0)
                {
                    if (!this.word.Vocabulary.Words[index].Equals(this.wordOriginal))
                    {
                        SetStatus(false, "This word already exists", ValidationMessageType.Error);
                    }
                }
            }
        }
    }

    abstract class MeaningValidator : Validator
    {
		protected IWord word = null;
        protected IMeaning meaning = null;

        protected MeaningValidator(IWord word, IMeaning meaning)
        {
			Debug.Assert(word != null);
			this.word = word;
			Debug.Assert(meaning != null);
            this.meaning = meaning;
            Hook(meaning.Translations);
            Hook(meaning.Synonyms);
            Hook(meaning.Antonyms);
            Hook(meaning.Categories);
            Hook(meaning);
        }

        protected override void ValidateOverride()
        {
            base.ValidateOverride();
            if (this.IsValid)
            {
                if (meaning.Translations.Count == 0)
                {
                    SetStatus(false, "At least one translation is required", ValidationMessageType.Error);
                }
                else if (meaning.Synonyms.Count == 0)
                {
					SetStatus(true, "Synonyms help LinguaSpace generate better flashcards", ValidationMessageType.Warning);
                }
                else if (meaning.Antonyms.Count == 0)
                {
					SetStatus(true, "Antonyms help LinguaSpace generate better flashcards", ValidationMessageType.Warning);
                }
                else if (StringUtils.IsEmpty(meaning.Definition))
                {
					SetStatus(true, "Definition help LinguaSpace generate better flashcards", ValidationMessageType.Warning);                    
                }
                else if (StringUtils.IsEmpty(meaning.Example))
                {
					SetStatus(true, "Example help LinguaSpace generate better flashcards", ValidationMessageType.Warning);
                }
				else if (meaning.Categories.Count == 0)
				{
					SetStatus(true, "Categories help LinguaSpace generate better flashcards", ValidationMessageType.Warning);
				}
            }
        }

        protected IMeaning FindSameMeaning(IMeaning meaning)
        {
            IMeaning same = null;
            foreach (IMeaning m in this.word.Meanings)
            {
                if (m.Translations.Count == meaning.Translations.Count)
                {
                    int i = 0;
                    for (i = 0; i < m.Translations.Count; ++i)
                    {
                        if (m.Translations[i] != meaning.Translations[i])
                        {
                            break;
                        }
                    }

                    if (i >= m.Translations.Count)
                    {
                        same = m;
                        break;
                    }
                }
            }
            return same;
        }
    }

    class InsertMeaningValidator : MeaningValidator
    {
        protected internal InsertMeaningValidator(IWord word, IMeaning meaning)
            : base(word, meaning)
        {
            Validate();
        }

        protected override void ValidateOverride()
        {
            base.ValidateOverride();
            if (this.IsValid)
            {
                if (FindSameMeaning(this.meaning) != null)
                {
                    SetStatus(false, "The meaning with the same translations already exists", ValidationMessageType.Error);
                }
            }
        }
    }

    class EditMeaningValidator : MeaningValidator
    {
        protected IMeaning meaningOriginal = null;

		protected internal EditMeaningValidator(IWord word, IMeaning meaning, IMeaning meaningOriginal)
            : base(word, meaning)
        {
            Debug.Assert(meaningOriginal != null);
            this.meaningOriginal = meaningOriginal;
            Validate();
        }

        protected override void ValidateOverride()
        {
            base.ValidateOverride();
            if (this.IsValid)
            {
                IMeaning m = FindSameMeaning(this.meaning);
                if (m != null && m != this.meaningOriginal)
                {
                    SetStatus(false, "The meaning with the same translations already exists", ValidationMessageType.Error);
                }
            }
        }
    }

    class EditVocabularyPropertiesValidator : Validator
    {
        protected IVocabulary vocabulary = null;

        protected internal EditVocabularyPropertiesValidator(IVocabulary vocabulary)
        {
            Debug.Assert(vocabulary != null);
            this.vocabulary = vocabulary;
            Hook(vocabulary);
            Hook(vocabulary.WordTypes);
            Hook(vocabulary.Categories);
            Hook(vocabulary.NativeLanguage);
            Hook(vocabulary.TargetLanguage);
            Validate();
        }

        protected override void ValidateOverride()
        {
            base.ValidateOverride();
            if (this.IsValid)
            {
                if (StringUtils.IsEmpty(this.vocabulary.Name))
                {
                    SetStatus(false, "Vocabulary name cannot be blank", ValidationMessageType.Error);
                }
                else if (this.vocabulary.WordTypes.Count == 0)
                {
                    SetStatus(false, "Word types are required", ValidationMessageType.Error);
                }
                else if (this.vocabulary.Categories.Count == 0)
                {
                    SetStatus(false, "Word categories are required", ValidationMessageType.Error);
                }
                else if (this.vocabulary.TargetLanguage.InputLocale == null)
                {
                    SetStatus(false, "Target language keyboard is required", ValidationMessageType.Error);
                }
                else if (this.vocabulary.NativeLanguage.InputLocale == null)
                {
                    SetStatus(false, "Native language keyboard is required", ValidationMessageType.Error);
                }
            }
        }
    }

	class FindTextValidator : Validator
	{
		protected TextData text;

		protected internal FindTextValidator(TextData text)
		{
			Debug.Assert(text != null);
			this.text = text;
			Hook(text);
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();
			if (this.IsValid)
			{
				if (StringUtils.IsEmpty(this.text.Text))
				{
					SetStatus(false, "Text cannot be blank", ValidationMessageType.Error);
				}
			}
		}
	}
	
	abstract class TextValidator : Validator
    {
		protected TextData text;
        protected ICollection list;
        protected String self;

		protected TextValidator(TextData text, ICollection list, String self)
        {
            Debug.Assert(text != null);
            Debug.Assert(list != null);
            this.text = text;
            this.list = list;
            this.self = self;
            Hook(text);
        }

        protected int IndexOf(ICollection list, String str)
        {
            int index = -1;
            int i = 0;
            foreach (Object obj in list)
            {
                if (obj.ToString().Equals(str))
                {
                    index = i;
                    break;
                }
                ++i;
            }
            return index;
        }

        protected bool Contains(ICollection list, String str)
        {
            return (IndexOf(list, str) >= 0);
        }

        protected override void ValidateOverride()
        {
            base.ValidateOverride();
            if (this.IsValid)
            {
                if (StringUtils.IsEmpty(this.text.Text))
                {
                    SetStatus(false, "Text cannot be blank", ValidationMessageType.Error);
                }
                else if (this.text.Text.Equals(this.self))
                {
                    SetStatus(false, "Circular reference", ValidationMessageType.Error);
                }
            }
        }
    }

    class InsertTextValidator : TextValidator
    {
		protected internal InsertTextValidator(TextData text, ICollection list, String self)
            : base(text, list, self)
        {
            Validate();
        }

        protected override void ValidateOverride()
        {
            base.ValidateOverride();
            if (this.IsValid)
            {
                if (Contains(this.list, this.text.Text))
                {
                    SetStatus(false, "Item already exists", ValidationMessageType.Error);
                }
            }
        }
    }

    class EditTextValidator : TextValidator
    {
        protected int index = -1;

		protected internal EditTextValidator(TextData text, ICollection list, String self)
            : base(text, list, self)
        {
            this.index = IndexOf(this.list, this.text.Text);
            Debug.Assert(this.index >= 0);
            Validate();
        }

        protected override void ValidateOverride()
        {
            base.ValidateOverride();
            if (this.IsValid)
            {
                int i = IndexOf(this.list, this.text.Text);
                if (i >= 0 && i != this.index)
                {
                    SetStatus(false, "Item already exists", ValidationMessageType.Error);
                }
            }
        }
    }

    class FindMeaningLookupValidator : Validator
    {
        protected LookupData lookup;

        protected internal FindMeaningLookupValidator(LookupData lookup)
        {
            Debug.Assert(lookup != null);
            this.lookup = lookup;
            Hook(lookup);
            Validate();
        }

        protected override void ValidateOverride()
        {
            base.ValidateOverride();
            if (this.IsValid)
            {
                if (StringUtils.IsEmpty(this.lookup.Text))
                {
                    SetStatus(false, "Text cannot be blank", ValidationMessageType.Error);
                }
                else if (this.lookup.Data == null)
                {
                    SetStatus(false, "Translation must be selected from the list", ValidationMessageType.Error);
                }
            }
        }
    }

	abstract class WordLookupValidator : Validator
	{
		protected LookupData lookup;

		protected internal WordLookupValidator(LookupData lookup)
		{
			Debug.Assert(lookup != null);
			this.lookup = lookup;
			Hook(lookup);
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();
			if (this.IsValid)
			{
				if (StringUtils.IsEmpty(this.lookup.Text))
				{
					SetStatus(false, "Text cannot be blank", ValidationMessageType.Error);
				}
				else if (this.lookup.Data == null)
				{
					SetStatus(false, "Word must be selected from the list", ValidationMessageType.Error);
				}
			}
		}
	}

	class FindWordLookupValidator : WordLookupValidator
	{
		protected internal FindWordLookupValidator(LookupData lookup)
			: base(lookup)
		{
			Validate();
		}
	}

	abstract class MeaningWordLookupValidator : WordLookupValidator
	{
		protected IWord parentWord;
		protected IMeaning meaning;
		protected WordCollection childrenWords;

		protected internal MeaningWordLookupValidator(IWord parentWord, IMeaning meaning, WordCollection childrenWords, LookupData lookup)
			: base(lookup)
		{
			Debug.Assert(parentWord != null);
			Debug.Assert(meaning != null);
			Debug.Assert(childrenWords != null);
			this.parentWord = parentWord;
			this.meaning = meaning;
			this.childrenWords = childrenWords;
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();
			if (this.IsValid)
			{
                WordItem item = (WordItem)this.lookup.Data;
				if (this.parentWord == item.Word)
				{
					SetStatus(false, "Circular reference", ValidationMessageType.Error);
				}
			}
		}
	}

	class InsertMeaningWordLookupValidator : MeaningWordLookupValidator
	{
		protected internal InsertMeaningWordLookupValidator(IWord parentWord, IMeaning meaning, WordCollection childrenWords, LookupData lookup)
			: base(parentWord, meaning, childrenWords, lookup)
		{
			Validate();
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();
			if (this.IsValid)
			{
                WordItem item = (WordItem)this.lookup.Data;
                if (this.childrenWords.Contains(item.Word))
				{
					SetStatus(false, String.Format("Word {0} already exists", this.lookup.Data.ToString()), ValidationMessageType.Error);
				}
			}
		}
	}

	class EditMeaningWordLookupValidator : MeaningWordLookupValidator
	{
		protected IWord originalWord;

		protected internal EditMeaningWordLookupValidator(IWord parentWord, IMeaning meaning, WordCollection childrenWords, LookupData lookup)
			: base(parentWord, meaning, childrenWords, lookup)
		{
            WordItem item = (WordItem)this.lookup.Data;
            this.originalWord = item.Word;
			Validate();
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();
			if (this.IsValid)
			{
                WordItem item = (WordItem)this.lookup.Data;
				if (this.childrenWords.Contains(item.Word) && this.originalWord != item.Word)
				{
					SetStatus(false, String.Format("Word {0} already exists", item.Word.ToString()), ValidationMessageType.Error);
				}
			}
		}
	}
}
