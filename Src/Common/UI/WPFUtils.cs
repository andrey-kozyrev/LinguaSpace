using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
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
using System.Globalization;
using System.Diagnostics;
using System.Windows.Threading;
using System.IO;
using System.Data;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Common.Data;
using LinguaSpace.Common.Resources;

namespace LinguaSpace.Common.UI
{
	delegate void MoveFocusDelegate(UIElement element);
	delegate void SelectListBoxItemDelegate(ListBox listBox, Object obj);
	delegate void SelectListBoxItemIndexDelegate(ListBox listBox, int index);
    delegate void SelectTreeViewItemDelegate(TreeView tree, Object obj);
    delegate void SelectTreeViewItemIndexDelegate(TreeView tree, int index);
    delegate void ExecuteDelegate(Object parameter, IInputElement target);

    public static class WPFUtils
    {
        private static BitmapSource DrawingIcons2BitmapSource(System.Drawing.Icon icon)
        {
            BitmapImage bmpi = new BitmapImage();
            using (System.Drawing.Bitmap bmp = icon.ToBitmap())
            {
                MemoryStream mem = new MemoryStream();
                bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
                bmpi.BeginInit();
                mem.Seek(0, SeekOrigin.Begin);
                bmpi.StreamSource = mem;
                bmpi.EndInit();
            }
            return bmpi;
        }

		private delegate void Func();

		public static void SelectListBoxModelNotify(ListBox listBox, Object obj)
		{
			SelectListBoxItem(listBox, obj);
			IPresentationModel model = (IPresentationModel)listBox.SelectedItem;
			model.NotifyPropertyChanged();
		}

        public static void SelectListBoxItem(ListBox listBox, Object obj)
        {
            listBox.ScrollIntoView(obj);
            listBox.SelectedItem = obj;
            ListBoxItem item = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(obj);
            if (item != null)
				item.Focus();
        }

		public static void SelectListBoxItem(ListBox listBox, int index)
		{
			Debug.Assert(0 <= index && index < listBox.Items.Count);
			SelectListBoxItem(listBox, listBox.Items[index]);
		}

        public static void SelectListBoxItemAsync(ListBox listBox, Object obj)
        {
            listBox.Dispatcher.BeginInvoke(DispatcherPriority.Background, new SelectListBoxItemDelegate(SelectListBoxItem), listBox, new Object[] { obj });
        }

		public static void SelectListBoxItemAsync(ListBox listBox, Object obj, DispatcherPriority priority)
		{
			listBox.Dispatcher.BeginInvoke(priority, new SelectListBoxItemDelegate(SelectListBoxItem), listBox, new Object[] { obj });
		}

		public static void SelectListBoxModelNotifyAsync(ListBox listBox, Object obj)
		{
			listBox.Dispatcher.BeginInvoke(DispatcherPriority.Background, new SelectListBoxItemDelegate(SelectListBoxModelNotify), listBox, new Object[] { obj });
		}

		public static void SelectListBoxItemAsync(ListBox listBox, int index)
		{
            listBox.Dispatcher.BeginInvoke(DispatcherPriority.Background, new SelectListBoxItemIndexDelegate(SelectListBoxItem), listBox, new Object[] { index });
		}

        public static void SelectItemAfterDelete(ListBox listBox, int indexDeleted)
        {
            int selectedIndex = indexDeleted;
            int count = listBox.Items.Count;
            if (count > 0)
            {
                if (selectedIndex >= count)
                {
                    selectedIndex = count - 1;
                }
                Object item = listBox.Items[selectedIndex];
                SelectListBoxItemAsync(listBox, item);
            }
        }

		public static ImageSource LoadImageSourceFromIcon(String assembly, String uri, int size)
		{
			return FindImageSource(CreateIconDecoder(assembly, uri), size);
		}

		private static BitmapDecoder CreateIconDecoder(String assembly, String uri)
		{
			String path = String.Format("pack://application:,,,/{0};component/{1}", assembly, uri);
			return new IconBitmapDecoder(new Uri(path), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
		}

		private static BitmapDecoder CreatePngDecoder(String assembly, String uri)
		{
			String path = String.Format("pack://application:,,,/{0};component/{1}", assembly, uri);
			return new PngBitmapDecoder(new Uri(path), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
		}

		public static ImageSource FindImageSource(BitmapDecoder decoder, int size)
		{
			ImageSource src = null;
			foreach (BitmapFrame frame in decoder.Frames)
			{
				if (frame.PixelHeight == size)
				{
					src = frame;
					break;
				}
			}
			return src;
		}
        
		public static ImageSource GetIconImageSource(String assembly, String category, String name, int size)
		{
			String path = String.Format("Resources/Icons/{0}/{1}/{2}.ico", size, category, name);
			return FindImageSource(CreateIconDecoder(assembly, path), size);
		}

		public static ImageSource GetPngImageSource(String assembly, String category, String name, int size)
		{
			String path = String.Format("Resources/Icons/{0}/{1}/{2}.png", size, category, name);
			return FindImageSource(CreatePngDecoder(assembly, path), size);
		}

		public static void MoveFocus(UIElement element)
		{
			element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
		}

		public static void MoveFocusAsync(UIElement element)
		{
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new MoveFocusDelegate(MoveFocus), element);
		}

        public static void Move<T>(ListBox listBox, int offset)
        {
            Debug.Assert(listBox != null);
            int index = listBox.SelectedIndex;
            (listBox.ItemsSource as ObservableCollection<T>).Move(index, index + offset);
            SelectListBoxItemAsync(listBox, index + offset);
        }

        public static bool CanMove(ListBox listBox, int offset)
        {
            Debug.Assert(listBox != null);
            return ((listBox.SelectedIndex >= 0) && 
                    (listBox.SelectedIndex + offset >= 0) && 
                    (listBox.SelectedIndex + offset < listBox.Items.Count));
        }

        public static void ExecuteAsync(RoutedCommand cmd, Object parameter, IInputElement target)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new ExecuteDelegate(cmd.Execute), parameter, target); 
        }
        
        private static bool ListBox_OnCanDeleteHelper(Object source)
        {
			ListBox listBox = source as ListBox;
			if (listBox == null)
				return false;

			IBindingList bindingList = listBox.ItemsSource as IBindingList;
			if (bindingList == null)
				return false;
				
			if (listBox.SelectedIndex < 0 || listBox.SelectedIndex >= bindingList.Count)
				return false;

			return true;
        }
        
        public static void ListBox_OnCanDelete(Object sender, CanExecuteRoutedEventArgs e)
        {
			e.CanExecute = ListBox_OnCanDeleteHelper(e.Source);
        }
        
        public static bool ListBox_OnDelete(Object sender, ExecutedRoutedEventArgs e)
        {
			bool success = false;
        
			ListBox listBox = e.Source as ListBox;
			IBindingList bindingList = (IBindingList)listBox.ItemsSource;
			int selectedIndex = listBox.SelectedIndex;

            if (MessageBox.Show("Do you want to delete selected item?", CommonStrings.PRODUCT_VERSION_FULL, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
			{
				try
				{
					bindingList.RemoveAt(selectedIndex);
					WPFUtils.SelectItemAfterDelete(listBox, selectedIndex);
					success = true;
				}
				catch (InvalidConstraintException)
				{
					MessageBox.Show("This item cannot be deleted because it is already refered to.", CommonStrings.PRODUCT_VERSION_FULL, MessageBoxButton.OK, MessageBoxImage.Error);
				}
				catch (Exception err)
				{
					MessageBox.Show(err.Message, CommonStrings.PRODUCT_VERSION_FULL, MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			
			return success;
        }
        
        private static bool ItemsControl_OnCanEditHelper(Object source)
        {
			ListBox listBox = source as ListBox;
			if (listBox == null)
				return false;

			IBindingList bindingList = listBox.ItemsSource as IBindingList;
			if (bindingList == null)
				return false;

			if (bindingList.Count == 0)
				return false;

			IPresentationModel item = listBox.SelectedItem as IPresentationModel;
			if (item == null)
				return false;
			
			return true;
        }
        
        public static void ListBox_OnCanEdit(Object sender, CanExecuteRoutedEventArgs e)
        {
			e.CanExecute = ItemsControl_OnCanEditHelper(e.Source);
        }

		public static void ListBox_OnEdit<TDisplayModel, TEditModel, TEditWnd, TValidator>(Object sender, ExecutedRoutedEventArgs e)
			where TDisplayModel : IPresentationModel, ITransactionContextAware
			where TEditModel : IPresentationModel, ITransactionContextAware
			where TEditWnd : Window
			where TValidator : IValidationStatusProvider, IDisposable
        {
			ListBox listBox = e.Source as ListBox;
			IPresentationList<TDisplayModel> presentationList = (IPresentationList<TDisplayModel>)listBox.ItemsSource;
			IBindingList bindingList = (IBindingList)listBox.ItemsSource;
			TDisplayModel displayModel = (TDisplayModel)listBox.SelectedItem;

			ITransactionContext context = displayModel.TransactionContext;
			using (ITransaction transaction = context.BeginTransaction())
			{
				using (TEditModel editModel = (TEditModel)Activator.CreateInstance(typeof(TEditModel), displayModel.Data))
				{
					using (TValidator validator = (TValidator)Activator.CreateInstance(typeof(TValidator), editModel.Data))
					{
						TEditWnd wnd = (TEditWnd)Activator.CreateInstance(typeof(TEditWnd), editModel, validator);
						wnd.Owner = Window.GetWindow(listBox);
						if (wnd.ShowDialog() ?? false)
						{
							transaction.Success = true;
							int index = presentationList.IndexOf(displayModel);
							presentationList.NotifyItemChanged(ListChangedType.ItemMoved, index);
							WPFUtils.SelectListBoxModelNotifyAsync(listBox, displayModel);
						}
					}
				}
			}
        }
        
		public static void TreeView_OnCanEdit(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ItemsControl_OnCanEditHelper(e.Source);
		}
		
		
        
        private static bool ListBox_OnCanNewHelper(Object source)
        {
			ListBox listBox = source as ListBox;
			if (listBox == null)
				return false;

			IBindingList bindingList = listBox.ItemsSource as IBindingList;
			if (bindingList == null)
				return false;

			return true;
        }
        
		public static void ListBox_OnCanNew(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ListBox_OnCanNewHelper(e.Source);
		}
        
        public static void ListBox_OnNew<TDisplayModel, TEditModel, TEditWnd, TValidator>(Object sender, ExecutedRoutedEventArgs e)
			where TDisplayModel : IPresentationModel, ITransactionContextAware
			where TEditModel : IPresentationModel, ITransactionContextAware
			where TEditWnd : Window
			where TValidator : IValidationStatusProvider, IDisposable
		{
			ListBox listBox = e.Source as ListBox;
			IPresentationList<TDisplayModel> presentationList = (IPresentationList<TDisplayModel>)listBox.ItemsSource;
			IBindingList bindingList = (IBindingList)listBox.ItemsSource;
			ITransactionContextAware contextAware = (ITransactionContextAware)listBox.DataContext;
			ITransactionContext context = contextAware.TransactionContext;
			using (ITransaction transaction = context.BeginTransaction())
			{
				TDisplayModel displayModel = (TDisplayModel)bindingList.AddNew();
				using (TEditModel editModel = (TEditModel)Activator.CreateInstance(typeof(TEditModel), displayModel.Data))
				{
					using (TValidator validator = (TValidator)Activator.CreateInstance(typeof(TValidator), editModel.Data))
					{
						TEditWnd wnd = (TEditWnd)Activator.CreateInstance(typeof(TEditWnd), editModel, validator);
						wnd.Owner = Window.GetWindow(listBox);
						if (wnd.ShowDialog() ?? false)
						{
							transaction.Success = true;
							int index = presentationList.IndexOf(displayModel);
							presentationList.NotifyItemChanged(ListChangedType.ItemMoved, index);
							WPFUtils.SelectListBoxModelNotifyAsync(listBox, displayModel);
						}
					}
				}
			}
		}

		private static bool ListBox_OnCanMoveHelper<TItem>(Object source, int offset)
		{
			ListBox listBox = source as ListBox;
			if (listBox == null)
				return false;
				
			IPresentationList<TItem> presentationList = listBox.ItemsSource as IPresentationList<TItem>;
			if (presentationList == null)
				return false;
			
			IBindingList list = presentationList.List as IBindingList;
			if (list == null)
				return false;
			
			return CanMove(listBox, offset);
		}

		public static void ListBox_OnCanMove<TItem>(Object sender, CanExecuteRoutedEventArgs e, int offset)
		{
			e.CanExecute = ListBox_OnCanMoveHelper<TItem>(e.Source, offset);
		}
		
		public static void ListBox_OnMove<TItem>(Object sender, ExecutedRoutedEventArgs e, int offset, String columnName)
		{
			ListBox listBox = e.Source as ListBox;
			IPresentationList<TItem> presentationList = listBox.ItemsSource as IPresentationList<TItem>;
			IBindingList list = (IBindingList)presentationList.List;
			int selectedIndex = listBox.SelectedIndex;
			AdoUtils.Swap(list, columnName, selectedIndex, selectedIndex + offset);
			SelectListBoxItemAsync(listBox, selectedIndex + offset);
		}

        public static void ListBox_OnCanCut(Object sender, CanExecuteRoutedEventArgs e)
        {
            ListBox_OnCanCopy(sender, e);
        }

        public static void ListBox_OnCut(Object sender, ExecutedRoutedEventArgs e, String format)
        {
            ListBox listBox = (ListBox)sender;
            IBindingList bindingList = (IBindingList)listBox.ItemsSource;
            int selectedIndex = listBox.SelectedIndex;
            if (MessageBox.Show("Do you want to delete selected item?", CommonStrings.PRODUCT_VERSION_FULL, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                try
                {
                    ListBox_OnCopy(sender, e, format);
                    bindingList.RemoveAt(selectedIndex);
                    WPFUtils.SelectItemAfterDelete(listBox, selectedIndex);
                }
                catch (InvalidConstraintException ice)
                {
                    MessageBox.Show("This item cannot be deleted because it is already refered to.", CommonStrings.PRODUCT_VERSION_FULL, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, CommonStrings.PRODUCT_VERSION_FULL, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public static void ListBox_OnCanCopy(Object sender, CanExecuteRoutedEventArgs e)
        {
            ListBox list = sender as ListBox;
            if (list != null)
            {
                IBinarySerializable bs = list.SelectedItem as IBinarySerializable;
                if (bs != null)
                {
                    e.CanExecute = true;
                }
            }
        }

        public static void ListBox_OnCopy(Object sender, ExecutedRoutedEventArgs e, String format)
        {
            ListBox list = (ListBox)sender;
            IBinarySerializable serializable = (IBinarySerializable)list.SelectedItem;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    serializable.Serialize(writer);
                    Clipboard.SetData(format, stream.ToArray());
                }
            }
        }

        public static void ListBox_OnCanPaste<TDisplayModel>(Object sender, CanExecuteRoutedEventArgs e, String format)
        {
            if (Clipboard.ContainsData(format))
            {
                ListBox list = sender as ListBox;
                if (list != null)
                {
                    IPresentationList<TDisplayModel> rules = list.ItemsSource as IPresentationList<TDisplayModel>;
                    if (rules != null)
                    {
                        IBindingList bindingList = rules as IBindingList;
                        if (bindingList != null)
                        {
                            ITransactionContextAware tca = list.DataContext as ITransactionContextAware;
                            if (tca != null)
                            {
                                e.CanExecute = true;
                            }
                        }
                    }
                }
            }
        }

        public static void ListBox_OnPaste<TDisplayModel>(Object sender, ExecutedRoutedEventArgs e, String format)
        {
            ListBox list = (ListBox)sender;
            IPresentationList<TDisplayModel> pl = (IPresentationList<TDisplayModel>)list.ItemsSource;
            IBindingList bl = (IBindingList)pl;
            ITransactionContextAware tca = (ITransactionContextAware)list.DataContext;
            ITransactionContext tc = tca.TransactionContext;
            using (ITransaction t = tc.BeginTransaction())
            {
                using (MemoryStream stream = new MemoryStream((byte[])Clipboard.GetData(format)))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        TDisplayModel model = (TDisplayModel)bl.AddNew();
                        IBinarySerializable bs = (IBinarySerializable)model;
                        bs.Deserialize(reader);
                        t.Success = true;
                        int index = pl.IndexOf(model);
                        pl.NotifyItemChanged(ListChangedType.ItemMoved, index);
                        WPFUtils.SelectListBoxModelNotifyAsync(list, model);
                    }
                }
            }            
        }

		private static bool IsExpanded(ItemsControl control)
		{
			Debug.Assert(control != null);
			TreeViewItem tvi = control as TreeViewItem;
			return (tvi == null || tvi.IsExpanded);
		}

        public static ItemsControl FindHierarchicalItem(ItemsControl control, Object model)
        {
            Debug.Assert(control != null);
            Debug.Assert(model != null);

            ItemsControl result = null;
            
            if (IsExpanded(control))
            {
				foreach (Object obj in control.Items)
				{
					ItemsControl item = (ItemsControl)control.ItemContainerGenerator.ContainerFromItem(obj);
					result = model.Equals(obj) ? item : FindHierarchicalItem(item, model);
					if (result != null)
						break;
				}
			}

            return result;
        }

        public static void SelectTreeViewItem(TreeView tree, Object model)
        {
            TreeViewItem item = (TreeViewItem)FindHierarchicalItem(tree, model);
            if (item != null)
            {
                item.IsSelected = true;
                item.Focus();
            }
            else
            {
                tree.Focus();
            }
        }

        public static void SelectTreeViewItemAsync(TreeView tree, Object model)
        {
            tree.Dispatcher.BeginInvoke(DispatcherPriority.Background, new SelectTreeViewItemDelegate(SelectTreeViewItem), tree, model);
        }
    }
}
