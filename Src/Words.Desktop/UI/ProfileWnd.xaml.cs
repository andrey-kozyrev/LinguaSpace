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
using LinguaSpace.Words.ComponentModel;

using LinguaSpace.Common.ComponentModel;

namespace LinguaSpace.Words.UI
{
    [System.Reflection.Obfuscation(Exclude = true)]
	public partial class ProfileWnd : System.Windows.Window
	{
		public ProfileWnd(ProfileModel model, IValidationStatusProvider vsp)
		{
			Debug.Assert(model != null);
			Debug.Assert(vsp != null);

			InitializeComponent();

			this.DataContext = model;
			this.status.DataContext = vsp;

			this.GotFocus += new RoutedEventHandler(ProfileWnd_GotFocus);
		}

		private void ProfileWnd_GotFocus(object sender, RoutedEventArgs e)
		{
			InputLanguageUtils.SetDefaultInputLanguage();
		}
	}
}