using System;
using System.Globalization;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using LinguaSpace.Common.Text;
using LinguaSpace.Common.ComponentModel;

namespace LinguaSpace.Common.Data
{
	public class AdoUtils
	{
		public static Int16 GetNextPosition(DataView dv, String columName)
		{
			Int16 positionNext = 0;
			DataColumn columnPosition = dv.Table.Columns[columName];
			foreach (DataRowView drv in dv)
			{
				Int16 position = (Int16)drv[columnPosition.Ordinal];
				if (positionNext < position)
					positionNext = position;
			}
			return ++positionNext;
		}

		public static void Swap(DataView dv, String columName, int index1, int index2)
		{
			Debug.Assert(0 <= index1 && index1 < dv.Count);
			Debug.Assert(0 <= index2 && index2 < dv.Count);

			DataRowView drv1 = dv[index1];
			DataRowView drv2 = dv[index2];

			Object pos = drv1[columName];
			drv1[columName] = drv2[columName];
			drv2[columName] = pos;
		}

		public static void Swap(IBindingList list, String columnName, int index1, int index2)
		{
			Debug.Assert(0 <= index1 && index1 < list.Count);
			Debug.Assert(0 <= index2 && index2 < list.Count);

			DataRowView drv1 = (DataRowView)list[index1];
			DataRowView drv2 = (DataRowView)list[index2];

			Object pos = drv1[columnName];
			drv1[columnName] = drv2[columnName];
			drv2[columnName] = pos;
		}

		private static PropertyDescriptor GetProperty(DataView view, String propName)
		{
			ITypedList tl = (ITypedList)view;
			PropertyDescriptorCollection pds = tl.GetItemProperties(null);
			return pds.Find(propName, false);
		}
		
		public static void AddIndex(DataView view, String propName)
		{
			IBindingList bl = (IBindingList)view;
			PropertyDescriptor pd = GetProperty(view, propName);
			bl.AddIndex(pd);
		}

		public static DataView GetDataView(DataRowView rv, String name)
		{
			return rv.DataView.Table.DataSet.Tables[name].DefaultView;
		}
		
		public static DataRowView FindRelatedRow(DataRowView rv, String relatedName, String column)
		{
			return FindRelatedRow(rv, relatedName, column, column);
		}

		public static DataRowView FindRelatedRow(DataRowView rv, String relatedName, String column, String relatedColumn)
		{
			DataView relatedView = rv.DataView.Table.DataSet.Tables[relatedName].DefaultView;
			Debug.Assert(relatedView != null);
			PropertyDescriptor relatedProperty = GetProperty(relatedView, relatedColumn);
			Debug.Assert(relatedProperty != null);
			return BindingListUtils.FindItem<DataRowView>(relatedView, relatedProperty, rv[column]);
		}
		
		public static DataRowView[] FindRelatedRows(DataRowView rv, String relatedName, String propertyFK)
		{
			return FindRelatedRows(rv, relatedName, propertyFK, propertyFK);
		}
		
		public static DataRowView[] FindRelatedRows(DataRowView rv, String relatedName, String propertyParent, String propertyChild)
		{
			DataView relatedView = rv.DataView.Table.DataSet.Tables[relatedName].DefaultView;
			Debug.Assert(relatedView != null);
			PropertyDescriptor property = GetProperty(relatedView, propertyChild);
			Debug.Assert(property != null);
			return BindingListUtils.FindItems<DataRowView>(relatedView, property, rv[propertyParent]);			
		}
		
		public static DataRowView FindRow(DataView view, String columnName, Object data)
		{
			foreach (DataRowView row in view)
				if (row[columnName].Equals(data))
					return row;
			return null;
		}

		public static CultureInfo GetCultureInfo(DataRowView row, String columnName)
		{
			CultureInfo ci = null;
			String name = (String)row[columnName];
			if (!StringUtils.IsEmpty(name))
			{
				try
				{
					ci = CultureInfo.GetCultureInfo(name);
				}
				catch
				{
				}
			}
			return ci;
		}

		public static void SetCultureInfo(DataRowView row, String columnName, CultureInfo info)
		{
			row[columnName] = info != null ? info.Name : null;
		}
		
	}
}
