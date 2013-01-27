using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms.Integration;
using LinguaSpace.Common.UI;
using LinguaSpace.Words.ComponentModel;

namespace LinguaSpace.Words.UI
{
	public partial class SyncWnd : Window
	{
		private SyncModel _model;

		public SyncWnd(SyncModel model)
		{
			Debug.Assert(model != null);
			_model = model;
			DataContext = model;
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private FileInfoPairModel GetFileInfoPairHelper(Object sender)
		{
			ListView list = sender as ListView;
			if (list == null)
				return null;
			return list.SelectedItem as FileInfoPairModel;
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanCopyFromDesktopToDevice(Object sender, CanExecuteRoutedEventArgs e)
		{
			FileInfoPairModel fip = GetFileInfoPairHelper(sender);
			e.CanExecute = (fip != null) ? fip.CanCopyDesktopToDevice() : false;
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCopyFromDesktopToDevice(Object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				ProgressForm progress = new ProgressForm();
				progress.SetDesktopLocation((int)(Left + (ActualWidth - progress.Width)/2), (int)(Top + (ActualHeight - progress.Height) / 2));
				progress.Text = this.Title;
				Thread t = new Thread(new ParameterizedThreadStart(ShowProgress));
				t.Start(progress);
				FileInfoPairModel fip = GetFileInfoPairHelper(sender);
				fip.CopyDesktopToDevice();
				CommandManager.InvalidateRequerySuggested();
				progress.BeginInvoke(new Action(progress.Close));
				t.Join();
			}
			catch (Exception err)
			{
				MessageBox.Show("Error during file copy", Title, MessageBoxButton.OK, MessageBoxImage.Error);
				Close();
			}

			CommandManager.InvalidateRequerySuggested();
		}
		
		private void ShowProgress(Object parameter)
		{
			ProgressForm progress = (ProgressForm)parameter;
			progress.ShowDialog();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanCopyFromDeviceToDesktop(Object sender, CanExecuteRoutedEventArgs e)
		{
			FileInfoPairModel fip = GetFileInfoPairHelper(sender);
			e.CanExecute = (fip != null) ? fip.CanCopyDeviceToDesktop() : false;
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCopyFromDeviceToDesktop(Object sender, ExecutedRoutedEventArgs e)
		{
			Cursor = Cursors.Wait;
			try
			{
				ProgressForm progress = new ProgressForm();
				progress.SetDesktopLocation((int)(Left + (ActualWidth - progress.Width) / 2), (int)(Top + (ActualHeight - progress.Height) / 2));
				progress.Text = this.Title;
				Thread t = new Thread(new ParameterizedThreadStart(ShowProgress));
				t.Start(progress);
				FileInfoPairModel fip = GetFileInfoPairHelper(sender);
				fip.CopyDeviceToDesktop();
				CommandManager.InvalidateRequerySuggested();
				progress.BeginInvoke(new Action(progress.Close));
				t.Join();
			}
			catch (Exception err)
			{
				MessageBox.Show("Error during file copy", Title, MessageBoxButton.OK, MessageBoxImage.Error);
				Close();
			}
			finally
			{
				Cursor = Cursors.Arrow;
			}

		}
	}
}
