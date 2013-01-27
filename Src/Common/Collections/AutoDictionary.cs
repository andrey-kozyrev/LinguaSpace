using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSpace.Common.Collections
{
	public class AutoDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TValue: new()
	{
		public new TValue this[TKey key] 
		{
			get
			{
				TValue v;
				if (!base.TryGetValue(key, out v))
				{
					v = new TValue();
					base.Add(key, v);
				}
				return v;
			}
			set
			{
				base[key] = value;
			}
		}
	}
}
