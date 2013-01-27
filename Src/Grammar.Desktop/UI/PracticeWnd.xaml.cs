using System;
using System.Diagnostics;
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
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Interop;
using LinguaSpace.Common.UI;
using LinguaSpace.Grammar.ComponentModel;
using LinguaSpace.Grammar.Resources;

namespace LinguaSpace.Grammar.UI
{
	/// <summary>
	/// Interaction logic for PracticeWnd.xaml
	/// </summary>
	public partial class PracticeWnd : Window
	{
		private DoubleAnimation _animationOpacity;
		private ColorAnimation _animationColor;

		private PracticeModel _model;
	
		public PracticeWnd(PracticeModel model)
		{
			Debug.Assert(model != null);
			_model = model;
			_animationOpacity = new DoubleAnimation(0, 1.0, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
			_animationOpacity.RepeatBehavior = new RepeatBehavior(3.5);
			_animationOpacity.AutoReverse = true;
			InitializeComponent();
			DataContext = _model;
			Loaded += new RoutedEventHandler(PracticeWnd_Loaded);
			_model.RuleTextChanged += new EventHandler(Model_RuleTextChanged);
			_answer.TextInput += new TextCompositionEventHandler(Answer_TextInput);
			_answer.TextChanged += new TextChangedEventHandler(Answer_TextChanged);
		}

		private void Answer_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextChange change = e.Changes.FirstOrDefault<TextChange>();
			if (change != null && change.AddedLength == 1)
			{
				if (change.Offset + 1 < _answer.Text.Length && _answer.Text[change.Offset + 1] == '.')
				{
					_answer.SelectionStart = change.Offset + 1;
					_answer.SelectionLength = 1;
					_answer.SelectedText = "";
				}
			}
		}

		private void Answer_TextInput(Object sender, TextCompositionEventArgs e)
		{
			;
		}

		private void PracticeWnd_Loaded(Object sender, RoutedEventArgs e)
		{
			_answer.Focus();
			int index = _answer.Text.IndexOf("..");
			if (index >= 0)
			{
				_answer.SelectionStart = index;
			}

			WINDOWPLACEMENT wp;
			Win32.GetWindowPlacement(new WindowInteropHelper(this).Handle, out wp); 
		}

		private void Model_RuleTextChanged(Object sender, EventArgs e)
		{
			_ruleViewer.BeginAnimation(FlowDocumentScrollViewer.OpacityProperty, _animationOpacity);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanNext(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _model.IsNextEnabled;
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnNext(Object sender, ExecutedRoutedEventArgs e)
		{
			if (!_model.IsEmpty)
            {
				_model.Next();
				_answer.Focus();
				int index = _answer.Text.IndexOf("..");
				if (index >= 0)
				{
					_answer.SelectionStart = index;
				}
            }
            else
            {
                MessageBox.Show("You are done!", Strings.WINDOW_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanAnswer(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _model.IsAnswerEnabled;
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnAnswer(Object sender, ExecutedRoutedEventArgs e)
		{
			_model.Answer();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanStop(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnStop(Object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanSkip(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !_model.IsEmpty;
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnSkip(Object sender, ExecutedRoutedEventArgs e)
		{
            if (!_model.IsEmpty)
            {
                _model.Skip();
                _answer.Focus();
				int index = _answer.Text.IndexOf("..");
				if (index >= 0)
				{
					_answer.SelectionStart = index;
				}
            }
            else
            {
                MessageBox.Show("You are done!", Strings.WINDOW_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
		}
	}
}
