using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using LinguaSpace.Words.ComponentModel;

namespace LinguaSpace.Words.UI
{
	public static class InputLanguageUtils
	{
		public static void SetDefaultInputLanguage()
		{
			foreach (CultureInfo info in InputLanguageManager.Current.AvailableInputLanguages)
			{
				InputLanguageManager.Current.CurrentInputLanguage = info;
				break;	
			}
		}
	
		public static void SetTargetInputLangage()
		{
			LinguaSpaceModel model = (LinguaSpaceModel)Application.Current.MainWindow.DataContext;
			InputLanguageManager.Current.CurrentInputLanguage = model.Vocabulary.TargetLang;
		}
		
		public static void SetNativeInputLanguage()
		{
			LinguaSpaceModel model = (LinguaSpaceModel)Application.Current.MainWindow.DataContext;
			InputLanguageManager.Current.CurrentInputLanguage = model.Vocabulary.NativeLang;
		}
	}
}
