using System;
using System.Collections.Generic;
using System.Text;
using LinguaSpace.Common;
using LinguaSpace.Data;

namespace LinguaSpace.Practice
{
    [Flags]
    public enum FlashCardItemType
    {
        None = 0x00,
        Translations = 0x01,
        Synonyms = 0x02,
        Antonyms = 0x04,
        Definition = 0x08,
        Word = 0x0F,
        Maximum = 0xFF
    }

	public enum FlashCardItemStatus
	{
		Unknown,
		Wrong,
		Right
	}

	public enum FlashCardStatus
	{
		Question,
		Prompt,
		Wrong,
		Right
	}

	public interface IFlashCardItem
	{
        FlashCardItemType Type
        {
            get;
        }

		String Caption
		{
			get;
		}

		String Text
		{
			get;
		}

		FlashCardItemStatus Status
		{
			get;
		}
	}

	public interface IFlashCard
	{
		IFlashCardItem Question
		{
			get;
		}

		IEnumerable<IFlashCardItem> Answers
		{
			get;
		}

		String Example
		{
			get;
		}

		FlashCardStatus Status
		{
			get;
		}

		void Answer(IFlashCardItem item);
		void Prompt();
	}

	public interface IFlashCardsGenerator
	{
        void Initialize(IUserProfile profile, IVocabulary vocabulary, IProgressReport progress, bool registered);
		IFlashCard Generate();
	}
}
