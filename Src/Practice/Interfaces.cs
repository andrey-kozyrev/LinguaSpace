using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSpace.Words.Practice
{
	public enum FlashCardItemType
	{
		None,
		Translations,
		Synonyms,
		Antonyms,
		Definition,
		Word
	}

	public enum FlashCardItemStatus
	{
		Unknown,
		Wrong,
		Right,
		Other
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

	public class StatusEventArgs : EventArgs
	{
		private readonly FlashCardStatus status;

		public StatusEventArgs(FlashCardStatus status)
		{
			this.status = status;
		}
		
		public FlashCardStatus Status
		{
			get
			{
				return this.status;
			}
		}
	}

	public delegate void StatusChangedHandler(Object sender, StatusEventArgs args);

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

		event StatusChangedHandler StatusChanged;

		void Answer(IFlashCardItem item);
		void Prompt();
	}

	public interface IFlashCardsGenerator : IDisposable
	{
		IFlashCard Generate();
	}
}
