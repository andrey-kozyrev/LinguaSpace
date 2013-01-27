using System;
using System.Diagnostics;
using System.ComponentModel;
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
using LinguaSpace.Common.ComponentModel;

namespace LinguaSpace.Words.UI
{
	public partial class WordFindWnd : Window
    {
        public WordFindWnd(Object model, IValidationStatusProvider validator)
        {
			Debug.Assert(model != null);
			Debug.Assert(validator != null);

            InitializeComponent();

			this.listWords.SelectionChanged += new SelectionChangedEventHandler(listWords_SelectionChanged);
			
			this.DataContext = model;
			this.status.DataContext = validator;

			this.GotFocus += new RoutedEventHandler(WordFindWnd_GotFocus);
        }

		private void WordFindWnd_GotFocus(object sender, RoutedEventArgs e)
		{
			InputLanguageUtils.SetTargetInputLangage();
		}

		private void listWords_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;
			Object item = e.AddedItems[0];
			this.listWords.ScrollIntoView(item);
		}

		private void Execute_GoToListBox(Object sender, ExecutedRoutedEventArgs e)
		{
			FrameworkElement fe = e.OriginalSource as FrameworkElement;
			if (fe != null)
				fe.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
		}
    }
}
