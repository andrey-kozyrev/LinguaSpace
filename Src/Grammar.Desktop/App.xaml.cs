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
using LinguaSpace.Grammar.Properties;

namespace Grammar
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			Exit += App_Exit;
		}

		private void App_Exit(Object sender, ExitEventArgs e)
		{
			Settings.Default.Save();
		}

		private void list_MouseDown(Object sender, MouseButtonEventArgs e)
		{
			ItemsControl control = sender as ItemsControl;
			if (control != null)
			{
				if (control.Items.Count == 0)
				{
					control.Focus();
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

		private void TreeViewItem_Selected(Object sender, RoutedEventArgs e)
		{
			TreeView tree = e.Source as TreeView;
			Debug.Assert(tree != null);
			TreeViewItem item = e.OriginalSource as TreeViewItem;
			Debug.Assert(item != null);
			tree.Tag = item.IsSelected ? item : null;
		}
	}
	
	
}
