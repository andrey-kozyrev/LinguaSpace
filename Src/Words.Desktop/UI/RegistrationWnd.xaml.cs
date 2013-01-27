using System;
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

using LinguaSpace.Common;
using LinguaSpace.Common.Licensing;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Common.Resources;
using LinguaSpace.Words.Resources;

namespace LinguaSpace.Words.UI
{
	/// <summary>
	/// Interaction logic for RegistrationWnd.xaml
	/// </summary>
	public partial class RegistrationWnd : System.Windows.Window
	{
		public RegistrationWnd(RegistrationService service, IValidationStatusProvider vsp)
		{
			InitializeComponent();

			this.DataContext = service.Registration;
			this.status.DataContext = vsp;

			this.GotFocus += new RoutedEventHandler(RegistrationWnd_GotFocus);
		}

		private void RegistrationWnd_GotFocus(object sender, RoutedEventArgs e)
		{
			InputLanguageUtils.SetDefaultInputLanguage();
		}

        private void Image_MouseUp(Object sender, MouseButtonEventArgs e)
        {
            IRegistration r = (IRegistration)this.DataContext;
			String link = String.Format(Strings.PAYPAL, CommonStrings.PRODUCT_VERSION_FULL, r.ProductCode);

            try
			{
				Process proc = new Process();
				proc.StartInfo.FileName = link; 
				proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
				proc.Start();
			}
			catch
			{
                ;
			}
        }
	}
}