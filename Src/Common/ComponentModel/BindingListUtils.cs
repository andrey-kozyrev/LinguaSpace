using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace LinguaSpace.Common.ComponentModel
{
	public class BindingListUtils
	{
		public static PropertyDescriptor GetProperty(IBindingList bindingList, String propertyName)
		{
			//Tracer.WriteLine("BindingListUtils.GetProperty");
			ITypedList typedList = bindingList as ITypedList;
			if (typedList == null)
				return null;
			PropertyDescriptorCollection properties = typedList.GetItemProperties(null);
			return properties.Find(propertyName, false);
		}

		public static TItem FindItem<TItem>(IBindingList list, PropertyDescriptor property, Object value)
		{
			//Tracer.WriteLine("BindingListUtils.FindItem<TItem>");
			int pos = list.Find(property, value);
			return (TItem)(pos >= 0 ? list[pos] : default(TItem));
		}

		public static TItem[] FindItems<TItem>(IBindingList list, PropertyDescriptor property, Object value)
		{
			//Tracer.WriteLine("BindingListUtils.FindItems<TItem>");
			//using (Tracer.Indent())
			{
				int pos = list.Find(property, value);
				if (pos < 0)
					return null;

				int begin = 0;
				int end = 0;

				int pos1 = pos;
				while (pos1 >= 0 && property.GetValue(list[pos1]).Equals(value))
					begin = pos1--;

				int pos2 = pos;
				while (pos2 < list.Count && property.GetValue(list[pos2]).Equals(value))
					end = ++pos2;

				TItem[] items = new TItem[end - begin];
				for (int i = begin; i < end; ++i)
					items[i - begin] = (TItem)list[i];
				return items;
			}
		}
	}
}
