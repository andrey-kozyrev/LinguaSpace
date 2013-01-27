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

namespace LinguaSpace.Common.UI
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public abstract class ViewConverter : IValueConverter
    {
        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            Debug.Assert(false);
            return null;
        }

        public abstract Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture);
    }

    [System.Reflection.Obfuscation(Exclude = true)]
    public class ExampleConverter : ViewConverter
    {
        private FontFamily fontFamily = new FontFamily("Verdana");
        private Double fontSize = 15;
        private Brush brushHightlight;

        public Double FontSize
        {
            get
            {
                return this.fontSize;
            }
            set
            {
                Debug.Assert(value > 0);
                this.fontSize = value;
            }
        }

        public FontFamily FontFamily
        {
            get
            {
                return this.fontFamily;
            }

            set
            {
                Debug.Assert(value != null);
                this.fontFamily = value;
            }
        }

        public Brush Highlight
        {
            get
            {
                return this.brushHightlight;
            }

            set
            {
                Debug.Assert(value != null);
                this.brushHightlight = value;
            }
		}

        public override object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(FlowDocument));
            Debug.Assert(value is String);
            String text = (String)value;

            FlowDocument doc = FlowDocumentUtils.Text2FlowDocument(text, false, this.brushHightlight);
            doc.FontFamily = this.fontFamily;
            doc.FontSize = this.fontSize;
            doc.PagePadding = new Thickness(5);
            return doc;
        }
    }
}
