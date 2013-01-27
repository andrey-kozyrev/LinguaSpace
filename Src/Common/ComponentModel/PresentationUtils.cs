using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Data;
using LinguaSpace.Common.Data;

namespace LinguaSpace.Common.ComponentModel
{
	public static class PresentationUtils
	{
		public static PresentationBindingList<T> CreateFilteredPresentationList<T>(DataView view, String filterPropertyName, Object filterPropertyValue) where T : IPresentationModel
		{
			PropertyDescriptor property = BindingListUtils.GetProperty(view, filterPropertyName);
			FilterBindingList subview = new FilterBindingList(view, property);
			subview.FilterValue = filterPropertyValue;
			return new PresentationBindingList<T>(subview);
		}
	}
}
