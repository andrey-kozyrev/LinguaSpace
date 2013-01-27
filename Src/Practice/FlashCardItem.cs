using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinguaSpace.Common.ComponentModel;

namespace LinguaSpace.Words.Practice
{
#if PLATFORM_DESKTOP
	[DebuggerDisplay("{Type} : {Text}")]
#endif
	public class FlashCardItem : NotifyPropertyChangedImpl, IFlashCardItem
	{
		private readonly Guid _id;
		private readonly FlashCardItemType _type;
		private readonly String _text;
		private readonly String _example;
		private FlashCardItemStatus _status;
		
		public FlashCardItem(Guid id, FlashCardItemType type, String text)
			: this(id, type, text, String.Empty)
		{
		}
		
		public FlashCardItem(Guid id, FlashCardItemType type, String text, String example)
		{
			Debug.Assert(text != null);
			_id = id;
			_type = type;
			_text = text;
			_example = example;
		}
		
		public Guid Id
		{
			get
			{
				return _id;
			}
		}
		
		public FlashCardItemType Type
		{
			get
			{
				return _type;
			}
		}

		public String Caption
		{
			get
			{
				return _type.ToString();
			}
		}

		public String Text
		{
			get
			{
				return _text;
			}
		}

		public String Example
		{
			get
			{
				return _example;
			}
		}

		public FlashCardItemStatus Status
		{
			get
			{
				return _status;
			}
			
			set
			{
				_status = value;
				RaisePropertyChangedEvent("Status");
			}
		}

		public override bool Equals(Object obj)
		{
			FlashCardItem other = obj as FlashCardItem;
			if (other == null)
				return false;
				
			return _id.Equals(other._id);
		}
	}
}
