using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace LinguaSpace.Words.Practice
{
#if PLATFORM_DESKTOP
	[DebuggerDisplay("{MeaningId} : {Weight}")]
#endif
	class MeaningData : IComparable<MeaningData>
	{
		private Guid _meaningId;
		private Int32 _weight;
		
		public MeaningData(Guid meaningId)
		{
			Debug.Assert(meaningId != Guid.Empty);
			_meaningId = meaningId;
			_weight = Int32.MinValue;
		}
		
		public MeaningData(Guid meaningId, Int32 weight)
		{
			Debug.Assert(meaningId != Guid.Empty);
			_meaningId = meaningId;
			_weight = weight;
		}
		
		public Guid MeaningId
		{
			get
			{
				return _meaningId;
			}
		}
		
		public int Weight
		{
			get
			{
				return _weight;	
			}
			set
			{
				_weight = value;
			}
		}

		int IComparable<MeaningData>.CompareTo(MeaningData data)
		{
			return data.Weight.CompareTo(_weight);
		}
	}
}
