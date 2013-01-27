using System;
using System.ComponentModel;
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
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Common.UI;
using LinguaSpace.Grammar.ComponentModel;
using LinguaSpace.Grammar.Resources;

namespace LinguaSpace.Grammar.UI
{
	public partial class RuleWnd : Window
	{
		public RuleWnd(RuleEditModel model, Validator validator)
		{
			InitializeComponent();
			this.status.DataContext = validator;
			this.DataContext = model;

			this.comment.IsVisibleChanged += new DependencyPropertyChangedEventHandler(comment_IsVisibleChanged);
			this.comment.GotFocus += new RoutedEventHandler(comment_GotFocus);
			this.comment.LostFocus += new RoutedEventHandler(comment_LostFocus);

			new WindowClosingGuard(this, validator);
		}

		public void comment_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Dispatcher.CurrentDispatcher.BeginInvoke((Func<bool>)this.comment.Focus, DispatcherPriority.Background);
			this.comment.IsVisibleChanged -= new DependencyPropertyChangedEventHandler(comment_IsVisibleChanged);
		}

		private void comment_GotFocus(object sender, RoutedEventArgs e)
		{
			InputLanguageUtils.SetNativeInputLanguage();
		}

		private void comment_LostFocus(object sender, RoutedEventArgs e)
		{
			InputLanguageUtils.SetTargetInputLangage();
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanExampleNew(Object sender, CanExecuteRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnCanNew(sender, e);			
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnExampleNew(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnNew<ExampleItemModel, ExampleEditModel, ExampleWnd, ExampleValidator>(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanExampleEdit(Object sender, CanExecuteRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnCanEdit(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnExampleEdit(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnEdit<ExampleItemModel, ExampleEditModel, ExampleWnd, ExampleValidator>(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanExampleDelete(Object sender, CanExecuteRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnCanDelete(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnExampleDelete(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnDelete(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanExampleUp(Object sender, CanExecuteRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnCanMove<ExampleItemModel>(sender, e, -1);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnExampleUp(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnMove<ExampleItemModel>(sender, e, -1, Strings.COL_POSITION);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanExampleDown(Object sender, CanExecuteRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnCanMove<ExampleItemModel>(sender, e, 1);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnExampleDown(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnMove<ExampleItemModel>(sender, e, 1, Strings.COL_POSITION);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanToggleActive(Object sender, CanExecuteRoutedEventArgs e)
		{
			ListBox listBox = e.Source as ListBox;
			e.CanExecute = (listBox != null && listBox.SelectedItem != null);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnToggleActive(Object sender, ExecutedRoutedEventArgs e)
		{
			ListBox listBox = (ListBox)e.Source;			
			ExampleItemModel model = (ExampleItemModel)listBox.SelectedItem;
			model.Active = !model.Active;
			model.NotifyPropertyChanged();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanCutExample(Object sender, CanExecuteRoutedEventArgs e)
		{
            WPFUtils.ListBox_OnCanCut(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCutExample(Object sender, ExecutedRoutedEventArgs e)
		{
            WPFUtils.ListBox_OnCut(sender, e, "Grammar:Example");
		}
	
        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanCopyExample(Object sender, CanExecuteRoutedEventArgs e)
		{
            WPFUtils.ListBox_OnCanCopy(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCopyExample(Object sender, ExecutedRoutedEventArgs e)
		{
            WPFUtils.ListBox_OnCopy(sender, e, "Grammar:Example");
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanPasteExample(Object sender, CanExecuteRoutedEventArgs e)
		{
            WPFUtils.ListBox_OnCanPaste<ExampleItemModel>(sender, e, "Grammar:Example");
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnPasteExample(Object sender, ExecutedRoutedEventArgs e)
		{
            WPFUtils.ListBox_OnPaste<ExampleItemModel>(sender, e, "Grammar:Example");
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnCanHighlight(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = comment.SelectedText != null && comment.SelectedText != String.Empty;
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnHighlight(Object sender, ExecutedRoutedEventArgs e)
		{
			comment.SelectedText = "[" + comment.SelectedText + "]";
		}
    }
}
