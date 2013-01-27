using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;

using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Common.Data;
using LinguaSpace.Words.Data;
using LinguaSpace.Words.ComponentModel;

namespace LinguaSpace.Words.UI
{
    public partial class TextWnd : System.Windows.Window
    {
		public TextWnd(TextModel text, IValidationStatusProvider vsp)
        {
            Debug.Assert(text != null);
            Debug.Assert(vsp != null);

            InitializeComponent();
            
            this.DataContext = text;
            this.status.DataContext = vsp;

			this.GotFocus += new RoutedEventHandler(TextWnd_GotFocus);
        }

		private void TextWnd_GotFocus(Object sender, RoutedEventArgs e)
		{
			InputLanguageUtils.SetNativeInputLanguage();
		}
    }
}