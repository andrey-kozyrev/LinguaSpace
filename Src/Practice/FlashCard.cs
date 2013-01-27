using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Common.Collections;

namespace LinguaSpace.Words.Practice
{
#if PLATFORM_DESKTOP
	[DebuggerDisplay("{Status} : {Question.Text} : {Example}")]
#endif
	public class FlashCard : NotifyPropertyChangedImpl, IFlashCard
	{
		private static Random _rnd = new Random();
		private FlashCardItem _question;
		private readonly IList<FlashCardItem> _answers;
		private FlashCardStatus _status = FlashCardStatus.Question;
		private event StatusChangedHandler _statusChanged;
	
		public FlashCard()
		{
			_answers = new List<FlashCardItem>();
		}

		public IList<FlashCardItem> InnerAnswers
		{
			get
			{
				return _answers;
			}
		}
	
		public void Shuffle()
		{
			ListUtils.Shuffle<FlashCardItem>(_answers);
		}
	
		#region IFlashCard
	
		public IFlashCardItem Question
		{
			get
			{
				return _question;
			}
			set
			{
				Debug.Assert(value != null);
				_question = (FlashCardItem)value;
			}
		}

		public IEnumerable<IFlashCardItem> Answers
		{
			get
			{
				foreach (IFlashCardItem item in _answers)
					yield return item;
			}
		}

		public String Example
		{
			get
			{
				return _question.Example;
			}
		}

		public FlashCardStatus Status
		{
			get
			{
				return _status;
			}
		}

		public event StatusChangedHandler StatusChanged
		{
			add
			{
				_statusChanged += value;
			}
			remove
			{
				_statusChanged -= value;
			}
		}

		public void Answer(IFlashCardItem answer)
		{
			_question.Status = FlashCardItemStatus.Right;
			
			foreach (FlashCardItem item in _answers)
			{
				if (item.Equals(_question))
					item.Status = FlashCardItemStatus.Right;
				else if (item.Equals(answer))
					item.Status = FlashCardItemStatus.Wrong;
				else
					item.Status = FlashCardItemStatus.Other;
			}
			
			_status = (_question.Equals(answer)) ? FlashCardStatus.Right : FlashCardStatus.Wrong;
			
			RaisePropertyChangedEvent("Status");
			RaiseStatusChanged();
		}
		
		public void Prompt()
		{
			_question.Status = FlashCardItemStatus.Right;
		
			foreach (FlashCardItem item in _answers)
			{
				if (item.Equals(_question))
					item.Status = FlashCardItemStatus.Right;
				else
					item.Status = FlashCardItemStatus.Other;
			}
			
			_status = FlashCardStatus.Prompt;

			RaisePropertyChangedEvent("Status");
			RaiseStatusChanged();
		}
		
		#endregion
		
		protected void RaiseStatusChanged()
		{
			if (_statusChanged != null)
				_statusChanged(this, new StatusEventArgs(_status));
		}
	}
}
