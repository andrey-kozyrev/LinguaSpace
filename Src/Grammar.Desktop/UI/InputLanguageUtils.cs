using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using LinguaSpace.Grammar.ComponentModel;

namespace LinguaSpace.Grammar.UI
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
			ApplicationModel model = (ApplicationModel)Application.Current.MainWindow.DataContext;
			InputLanguageManager.Current.CurrentInputLanguage = model.Grammar.TargetLang;
		}
		
		public static void SetNativeInputLanguage()
		{
			ApplicationModel model = (ApplicationModel)Application.Current.MainWindow.DataContext;
			InputLanguageManager.Current.CurrentInputLanguage = model.Grammar.NativeLang;
		}
	}
}
