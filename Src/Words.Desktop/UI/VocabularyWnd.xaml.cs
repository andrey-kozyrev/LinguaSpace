using System;
using System.Globalization;
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
using System.Diagnostics;
using System.Windows.Threading;
using System.Data;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Common.Data;
using LinguaSpace.Common.UI;
using LinguaSpace.Words.Data;
using LinguaSpace.Words.ComponentModel;
using LinguaSpace.Words.Resources;

namespace LinguaSpace.Words.UI
{
    public partial class VocabularyWnd : System.Windows.Window
    {
		public VocabularyWnd(VocabularyModel model, IValidationStatusProvider vsp)
        {
			Debug.Assert(model != null);
			Debug.Assert(vsp != null);
        
            InitializeComponent();

			this.DataContext = model;
            this.status.DataContext = vsp;

			this.GotFocus += new RoutedEventHandler(VocabularyWnd_GotFocus);           
        }

		private void VocabularyWnd_GotFocus(object sender, RoutedEventArgs e)
		{
			InputLanguageUtils.SetTargetInputLangage();
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnCanDelete(Object sender, CanExecuteRoutedEventArgs e)
        {
			WPFUtils.ListBox_OnCanDelete(sender, e);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnDelete(Object sender, ExecutedRoutedEventArgs e)
        {
			WPFUtils.ListBox_OnDelete(sender, e);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public void OnCanEdit(Object sender, CanExecuteRoutedEventArgs e)
        {
			WPFUtils.ListBox_OnCanEdit(sender, e);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnEdit(Object sender, ExecutedRoutedEventArgs e)
        {
			WPFUtils.ListBox_OnEdit<TypeModel, TypeModel, TextWnd, TypeValidator>(sender, e);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public void OnCanNew(Object sender, CanExecuteRoutedEventArgs e)
        {
            WPFUtils.ListBox_OnCanNew(sender, e);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnNew(Object sender, ExecutedRoutedEventArgs e)
        {
			WPFUtils.ListBox_OnNew<TypeModel, TypeModel, TextWnd, TypeValidator>(sender, e);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public void OnCanMoveUp(Object sender, CanExecuteRoutedEventArgs e)
        {
            WPFUtils.ListBox_OnCanMove<TypeModel>(sender, e, -1);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public void OnCanMoveDown(Object sender, CanExecuteRoutedEventArgs e)
        {
			WPFUtils.ListBox_OnCanMove<TypeModel>(sender, e, 1);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnMoveUp(Object sender, ExecutedRoutedEventArgs e)
        {
			WPFUtils.ListBox_OnMove<TypeModel>(sender, e, -1, Strings.POSITION);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnMoveDown(Object sender, ExecutedRoutedEventArgs e)
        {
			WPFUtils.ListBox_OnMove<TypeModel>(sender, e, 1, Strings.POSITION);
		}
    }
}