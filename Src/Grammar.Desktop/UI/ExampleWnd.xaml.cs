using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using LinguaSpace.Common.UI;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Grammar.ComponentModel;

namespace LinguaSpace.Grammar.UI
{
	public partial class ExampleWnd : Window
	{
		public ExampleWnd(ExampleEditModel model, Validator validator)
		{
			InitializeComponent();
			this.status.DataContext = validator;
			this.DataContext = model;
			this.native.GotFocus += new RoutedEventHandler(native_GotFocus);
			this.target.GotFocus += new RoutedEventHandler(target_GotFocus);
			new WindowClosingGuard(this, validator);
		}

		private void native_GotFocus(object sender, RoutedEventArgs e)
		{
			InputLanguageUtils.SetNativeInputLanguage();
		}

		private void target_GotFocus(object sender, RoutedEventArgs e)
		{
			InputLanguageUtils.SetTargetInputLangage();
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnCanHighlight(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = target.SelectedText != null && target.SelectedText != String.Empty;
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnHighlight(Object sender, ExecutedRoutedEventArgs e)
		{
			target.SelectedText = "[" + target.SelectedText + "]";
		}
	}
}
