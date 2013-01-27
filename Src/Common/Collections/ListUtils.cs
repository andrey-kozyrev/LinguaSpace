using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSpace.Common.Collections
{
	public static class ListUtils
	{
		private static Random _rnd = new Random();
	
		public static void Shuffle<T> (IList<T> list)
		{
			for (int i = list.Count - 1; i > 0; --i)
			{
				int j = _rnd.Next(i);
				T item = list[i];
				list[i] = list[j];
				list[j] = item;
			}
		}
	}
}
