using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.Diagnostics;
using System.Windows.Threading;
using System.Data;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Common.Data;
using LinguaSpace.Words.Data;
using LinguaSpace.Words.ComponentModel;

namespace LinguaSpace.Words.UI
{
    public partial class WordWnd : System.Windows.Window
    {
		public WordWnd(WordEditModel word, IValidationStatusProvider vsp)
        {
			Debug.Assert(word != null);
            Debug.Assert(vsp != null);
            InitializeComponent();
			this.DataContext = word;
            this.status.DataContext = vsp;
			this.GotFocus += new RoutedEventHandler(WordWnd_GotFocus);
        }

		private void WordWnd_GotFocus(object sender, RoutedEventArgs e)
		{
			InputLanguageUtils.SetTargetInputLangage();
		}
    }
}

