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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LinguaSpace.Common.UI
{
	public partial class StatusCtrl : System.Windows.Controls.UserControl
	{
		public StatusCtrl()
		{
			InitializeComponent();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnClickOK(object sender, RoutedEventArgs e)
		{
			Window wnd = Window.GetWindow(this);
			wnd.DialogResult = true;
		}
	}
}
