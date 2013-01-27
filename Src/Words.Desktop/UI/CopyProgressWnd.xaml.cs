using System;
using System.IO;
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

namespace LinguaSpace.Words.UI
{
	/// <summary>
	/// Interaction logic for CopyProgressWnd.xaml
	/// </summary>
	public partial class CopyProgressWnd : Window
	{
		private bool _done;
		private Action _action;
		private Stream _stream;

		public CopyProgressWnd(Action action)
		{
			_action = action;
			InitializeComponent();
			Closing += new CancelEventHandler(CopyProgressWnd_Closing);
		}

		private void CopyProgressWnd_Closing(Object sender, CancelEventArgs e)
		{
			e.Cancel = !_done;
		}

		private void Window_Loaded(Object sender, RoutedEventArgs e)
		{
			Uri imageLocation = new Uri("../Resources/Icons/FileCopy.gif", UriKind.Relative);
			_stream = Application.GetResourceStream(imageLocation).Stream;
			_picture.Image = new System.Drawing.Bitmap(_stream);
			_picture.HandleDestroyed += new EventHandler(Picture_HandleDestroyed);

			Dispatcher.BeginInvoke(_action);

			// _action.Invoke();
			// _action.BeginInvoke(new AsyncCallback(OnActionCompleted), _action);
		}

		private void Picture_HandleDestroyed(object sender, EventArgs e)
		{
			_stream.Close();
			_stream = null;
		}

		private void OnActionCompleted(IAsyncResult result)
		{
			_done = true;
			Close();			
			_action.EndInvoke(result);
		}
	}
}
