using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Threading;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Threading;
using System.Diagnostics;
using LinguaSpace.Common.Text;
using LinguaSpace.Common.Resources;
using LinguaSpace.Common.UI;
using LinguaSpace.Words.UI;

namespace LinguaSpace.Words
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private EventWaitHandle nextInstanceStarted;

        private String GetFileNameFromStartup(StartupEventArgs e)
        {
            String fileName = String.Empty;
            if (e.Args.Length > 0)
            {
                fileName = e.Args[0];
            }
            return fileName;
        }
        
        private String GetFileNameFromClipboard()
        {
            String fileName = String.Empty;
			if (Clipboard.ContainsData(CommonStrings.PRODUCT_VERSION_SHORT))
            {
				fileName = (String)Clipboard.GetData(CommonStrings.PRODUCT_VERSION_SHORT);
            }
            return fileName;
        }

        private void PutFileNameToClipboard(String fileName)
        {
			Clipboard.SetData(CommonStrings.PRODUCT_VERSION_SHORT, fileName);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            String fileName = GetFileNameFromStartup(e);

            bool bCreated = false;
            EventWaitHandleSecurity security = new EventWaitHandleSecurity();
            String identity = WindowsIdentity.GetCurrent().Name;
            security.AddAccessRule(new EventWaitHandleAccessRule(identity, EventWaitHandleRights.FullControl, AccessControlType.Allow));
			this.nextInstanceStarted = new EventWaitHandle(false, EventResetMode.AutoReset, CommonStrings.PRODUCT_VERSION_SHORT, out bCreated, security);
            
            if (bCreated)
            {
                ThreadPool.RegisterWaitForSingleObject(this.nextInstanceStarted, new WaitOrTimerCallback(DispatchNextInstanceStarted), this, -1, false);
                
                if (!StringUtils.IsEmpty(fileName) && System.IO.File.Exists(fileName))
                {
                    DispatchShellOpen(fileName);
                }
            }
            else
            {
                if (!StringUtils.IsEmpty(fileName) && System.IO.File.Exists(fileName))
                {
                    PutFileNameToClipboard(fileName);
                }
                
                this.nextInstanceStarted.Set();
                Dispatcher.InvokeShutdown();
            }
        }

        private delegate void NextInstanceStartedDelegate();

        private void OnNextInstanceStarted()
        {
            if (this.MainWindow.WindowState == WindowState.Minimized)
            {
                this.MainWindow.WindowState = WindowState.Normal;
            }
            this.MainWindow.Activate();
            this.MainWindow.BringIntoView();

            String fileName = GetFileNameFromClipboard();

            if (!StringUtils.IsEmpty(fileName) && System.IO.File.Exists(fileName))
            {
                DispatchShellOpen(fileName);
            }
        }

        private void DispatchNextInstanceStarted(Object state, bool timedOut)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new NextInstanceStartedDelegate(OnNextInstanceStarted));
        }

        #region ShellOpen
        
        private delegate void ShellOpenDelegate(String fileName);

        private void ShellOpen(String fileName)
        {
            Debug.Assert(this.MainWindow != null);
            ShellCommands.Open.Execute(fileName, this.MainWindow);
        }

        private void DispatchShellOpen(String fileName)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new ShellOpenDelegate(ShellOpen), fileName);
        }

        private void list_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox != null)
            {
                if (listBox.Items.Count == 0)
                {
                    listBox.Focus();
                }
            }
        }

		private void item_GotFocus(Object sender, RoutedEventArgs e)
		{
			ListBoxItem item = sender as ListBoxItem;
			if (item != null)
			{
				item.IsSelected = true;
			}
		}

        #endregion
    }
}