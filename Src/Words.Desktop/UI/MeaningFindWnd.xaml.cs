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
using System.ComponentModel;
using LinguaSpace.Words.ComponentModel;
using LinguaSpace.Common.ComponentModel;

namespace LinguaSpace.Words.UI
{
    public partial class MeaningFindWnd : Window
    {
        public MeaningFindWnd(MeaningFinderModel model, IValidationStatusProvider validator)
        {
			Debug.Assert(model != null);
			Debug.Assert(validator != null);

			InitializeComponent();

			this.DataContext = model;
			this.status.DataContext = validator;

			this.GotFocus += new RoutedEventHandler(MeaningFindWnd_GotFocus);
		}

		private void MeaningFindWnd_GotFocus(object sender, RoutedEventArgs e)
		{
			InputLanguageUtils.SetNativeInputLanguage();
		}

		private void Execute_GoToListBox(Object sender, ExecutedRoutedEventArgs e)
		{
			FrameworkElement fe = e.OriginalSource as FrameworkElement;
			if (fe != null)
				fe.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
		}
    }
}
