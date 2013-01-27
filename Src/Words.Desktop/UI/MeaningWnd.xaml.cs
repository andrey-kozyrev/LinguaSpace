using System;
using System.Collections;
using System.Collections.ObjectModel;
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
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Threading;
using System.Data;
using LinguaSpace.Common.UI;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Words.Data;
using LinguaSpace.Words.Resources;
using LinguaSpace.Words.ComponentModel;

namespace LinguaSpace.Words.UI
{
    public partial class MeaningWnd : System.Windows.Window
    {
		private bool m_dirty = false;

        public MeaningWnd(MeaningEditModel model, IValidationStatusProvider validator)
        {
			Debug.Assert(model != null);
			Debug.Assert(validator != null);
			InitializeComponent();
			this.DataContext = model;
			this.status.DataContext = validator;

			INotifyPropertyChanged npc = (INotifyPropertyChanged)validator;
			npc.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
			
			this.listTranslations.MouseDoubleClick += new MouseButtonEventHandler(list_MouseDoubleClick);
			this.listSynonyms.MouseDoubleClick += new MouseButtonEventHandler(list_MouseDoubleClick);
			this.listAntonyms.MouseDoubleClick += new MouseButtonEventHandler(list_MouseDoubleClick);

            this.Closing += new CancelEventHandler(OnClosing);
			this.GotFocus += new RoutedEventHandler(MeaningWnd_GotFocus);
        }

		private void MeaningWnd_GotFocus(object sender, RoutedEventArgs e)
		{
			InputLanguageUtils.SetTargetInputLangage();
		}

		private void list_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
		{
			EditCommands.Edit.Execute(e, (IInputElement)sender);
		}

		private void OnPropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			this.m_dirty = true;
		}

        private void OnClosing(Object sender, CancelEventArgs e)
        {
			if (this.DialogResult == null || !(bool)this.DialogResult)
			{
				e.Cancel = (this.m_dirty && MessageBox.Show(this, "Discard changes ?", this.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel);
			}
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public void OnCanDelete(Object sender, CanExecuteRoutedEventArgs e)
        {
			WPFUtils.ListBox_OnCanDelete(sender, e);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnDelete(Object sender, ExecutedRoutedEventArgs e)
        {
			 WPFUtils.ListBox_OnDelete(sender, e);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public void OnCanNew(Object sender, CanExecuteRoutedEventArgs e)
        {
			WPFUtils.ListBox_OnCanNew(sender, e);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnNewTranslation(Object sender, ExecutedRoutedEventArgs e)
        {
			WPFUtils.ListBox_OnNew<TranslationModel, TranslationModel, TextWnd, TranslationValidator>(sender, e);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnNewWord(Object sender, ExecutedRoutedEventArgs e)
        {
			WPFUtils.ListBox_OnNew<WordRefItemModel, WordRefEditModel, WordFindWnd, WordRefValidator>(sender, e);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public void OnCanEdit(Object sender, CanExecuteRoutedEventArgs e)
        {
			WPFUtils.ListBox_OnCanEdit(sender, e);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnEditTranslation(Object sender, ExecutedRoutedEventArgs e)
        {
			WPFUtils.ListBox_OnEdit<TranslationModel, TranslationModel, TextWnd, TranslationValidator>(sender, e);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnEditWord(Object sender, ExecutedRoutedEventArgs e)
        {
			WPFUtils.ListBox_OnEdit<WordRefItemModel, WordRefEditModel, WordFindWnd, WordRefValidator>(sender, e);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public void OnTranslationCanMoveUp(Object sender, CanExecuteRoutedEventArgs e)
        {
            WPFUtils.ListBox_OnCanMove<TranslationModel>(sender, e, -1);
        }

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnTranslationMoveUp(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnMove<TranslationModel>(sender, e, -1, Strings.POSITION);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public void OnTranslationCanMoveDown(Object sender, CanExecuteRoutedEventArgs e)
        {
			WPFUtils.ListBox_OnCanMove<TranslationModel>(sender, e, 1);
        }

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnTranslationMoveDown(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnMove<TranslationModel>(sender, e, 1, Strings.POSITION);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public void OnWordCanMoveUp(Object sender, CanExecuteRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnCanMove<WordRefItemModel>(sender, e, -1);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnWordMoveUp(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnMove<WordRefItemModel>(sender, e, -1, Strings.POSITION);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public void OnWordCanMoveDown(Object sender, CanExecuteRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnCanMove<WordRefItemModel>(sender, e, 1);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnWordMoveDown(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnMove<WordRefItemModel>(sender, e, 1, Strings.POSITION);
		}
	}
}