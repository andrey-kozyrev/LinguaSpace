using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Documents;
using System.Globalization;
using System.Diagnostics;
using System.Data;

using LinguaSpace.Common.UI;
using LinguaSpace.Common.Text;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Words.Data;
using LinguaSpace.Words.Resources;

namespace LinguaSpace.Words.UI
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class MeaningItemRowHeightConverter : ViewConverter
    {
        public override Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(GridLength));
			Debug.Assert(value != null);
			GridLength gl = GridLength.Auto;
			if (value is int)
			{
				int count = (int)value;
				gl = (count > 0) ? GridLength.Auto : new GridLength(0);
			}
            return gl;
        }
    }

    [System.Reflection.Obfuscation(Exclude = true)]
	internal class IntegerConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			Debug.Assert(targetType == typeof(String));
			return value.ToString();
		}

		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			Debug.Assert(targetType == typeof(int));
			
			int i;
			
			if (value is int)
			{
				i = (int)value;
			}
			else if (value is String && int.TryParse((String)value, out i))
			{
				;
			}
			else 
			{
				i = 0;
			}
			
			return i;
		}
	}

    [System.Reflection.Obfuscation(Exclude = true)]
    internal class Modification2Visibility : ViewConverter
    {
        public override Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(Visibility));
            Debug.Assert(value is DateTime);
            DateTime date = (DateTime)value;
            DateTime now = DateTime.Now;
            TimeSpan span = now - date;
            return (span.Days > 7) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
    
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class Position2Position : ViewConverter
    {
        public override Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(String));
            Debug.Assert(value is int);
            int position = (int)value;
            return (position + 1).ToString();
        }
    }

    [System.Reflection.Obfuscation(Exclude = true)]
    internal class StringBox2String : ViewConverter
    {
        public override Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(String));
            Debug.Assert(value is StringBox);
            StringBox box = (StringBox)value;
            return box.ToString();
        }
    }

    [System.Reflection.Obfuscation(Exclude = true)]
    internal class WidthRatioDispenser : ViewConverter
    {
        public override Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(Double));
            Debug.Assert(value is Double);
            Double width = (Double)value;
            NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberDecimalSeparator = ".";
            Double ratio = Double.Parse((String)parameter, nfi);
            return ratio * (width - 35);
        }
    }

    [System.Reflection.Obfuscation(Exclude = true)]
    internal class WordTextConverter : ViewConverter
    {
        public override object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(String));
            Debug.Assert(value is IDataRecord);
            IDataRecord record = (IDataRecord)value;

            String prefix = (String)record["prefix"];
            String word = (String)record["word"];
            
            StringBuilder text = new StringBuilder();
            if (!StringUtils.IsEmpty(prefix))
            {
                text.Append(prefix);
                text.Append(" ");
            }
            text.Append(word);
            return text.ToString();
        }
    }

    [System.Reflection.Obfuscation(Exclude = true)]
    internal class WordTypeConverter : ViewConverter
    {
        public override object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    
    //////////////////////////////////////////////////////////////////////////////////////////////////////

	[System.Reflection.Obfuscation(Exclude = true)]
	internal class DataRowViewIndexConverter : ViewConverter
	{
		public override object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			DataRowView rowView = (DataRowView)value;
			DataView tableView = rowView.DataView;
			IList listTableView = (IList)tableView;
			return listTableView.IndexOf(rowView) + 1;
		}
	}

	[System.Reflection.Obfuscation(Exclude = true)]
	internal class TranslationsListConverter : ViewConverter
	{
		public override object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			DataView viewTable = (DataView)value;
			StringBuilder sb = new StringBuilder();
			foreach (DataRowView viewRow in viewTable)
			{
				String text = viewRow[Strings.TRANSLATION].ToString();
				if (sb.Length > 0)
					sb.Append(", ");
				sb.Append(text);
			}
			return sb.ToString();
		}
	}
}
