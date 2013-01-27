 using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;
using System.Data;
using System.IO;
using Microsoft.Win32;
using LinguaSpace.Common.Text;
using LinguaSpace.Common.Data;
using LinguaSpace.Common.UI;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Common.Resources;
using LinguaSpace.Grammar.ComponentModel;
using LinguaSpace.Grammar.IO;
using LinguaSpace.Grammar.Resources;
using LinguaSpace.Grammar.Properties;

namespace LinguaSpace.Grammar.UI
{
	public partial class MainWnd : Window
	{
		private ApplicationModel _model;

		public MainWnd()
		{
			InitializeComponent();
			GotFocus += MainWnd_GotFocus;
			Loaded += GrammarWnd_Loaded;
			Closing += GrammarWnd_Closing;
		}

		private void MainWnd_GotFocus(object sender, RoutedEventArgs e)
		{
			if (_model != null && _model.IsOpen)
				InputLanguageUtils.SetTargetInputLangage();
		}

		private void GrammarWnd_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Settings.Default.MainWindowPlacement = new WindowPlacement(this);

			if (_model.IsOpen)
				e.Cancel = !DoClose();
		}

		private void GrammarWnd_Loaded(object sender, RoutedEventArgs e)
		{
			Settings.Default.MainWindowPlacement.Apply(this);
	
			_model = new ApplicationModel();
			DataContext = _model;
			
			String path = FileUtils.GetLastUsedFile(FileUtils.GrammarsFolder, "*.lsg");
			if (path != null)
			{
				DoLoad(path);
			}
			else
			{
				DoNew();
			}
		}
		
		#region Atoms

		private MessageBoxResult DoPromptSave()
		{
			return _model.IsDirty
				? MessageBox.Show(this, String.Format("Do you want to save grammar '{0}'", _model.Grammar.Title), this.Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes)
				: MessageBoxResult.None;
		}

		private String DoAskSaveFileName()
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.InitialDirectory = FileUtils.GrammarsFolder;
			sfd.Filter = "LinguaSpace Grammar Files|*.lsg|All Files|*.*";
			if (sfd.ShowDialog(this) ?? false)
				return sfd.FileName;
			return String.Empty;
		}

		private bool DoSaveAs()
		{
			Debug.Assert(_model.IsOpen);
		
			String path = DoAskSaveFileName();
			if (StringUtils.IsEmpty(path))
				return false;

			if (System.IO.File.Exists(path))
				System.IO.File.Delete(path);

			_model.SaveAs(path);
			
			return true;
		}
		
		private bool DoSave()
		{
			Debug.Assert(_model.IsOpen);
			
			if (StringUtils.IsEmpty(_model.Path))
				return DoSaveAs();
				
			_model.Save();
			return true;
		}
		
		private bool DoClose()
		{
			Debug.Assert(_model.IsOpen);
			
			MessageBoxResult mgr = DoPromptSave();
			if (mgr == MessageBoxResult.Cancel)
				return false;
				
			if (mgr == MessageBoxResult.Yes)
				if (!DoSave())
					return false;
					
			_model.Close();
			return true;
		}
		
		private bool DoEdit()
		{
			Debug.Assert(_model.IsOpen);
			
			bool success = false;
			
			using (ITransaction t = _model.TransactionContext.BeginTransaction())
			{
				GrammarEditModel grammar = _model.Grammar;
				using (Validator v = new GrammarValidator(grammar.RowView))
				{
					GrammarWnd wnd = new GrammarWnd(grammar, v);
					wnd.Owner = this;
					if (wnd.ShowDialog() ?? false)
					{
						success = true;
						t.Success = true;
						_model.NotifyPropertyChanged();
					}
				}
			}
			
			return success;
		}
		
		private bool DoNew()
		{
			bool success = false;
		
			if (!_model.IsOpen || DoClose())
			{
				using (ITransaction t = _model.TransactionContext.BeginTransaction())
				{
					_model.CreateGrammar();
					_topics.DataContext = _model.Grammar.RootTopic;
					t.Success = DoEdit();
					success = t.Success;
					_model.NotifyPropertyChanged();
				}
			}
			
			return success;
		}
		
		private bool DoOpen()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.InitialDirectory = FileUtils.GrammarsFolder;
			ofd.Filter = "LinguaSpace Grammars (*.lsg)|*.lsg|All Files|*.*";
			if (!(ofd.ShowDialog(this) ?? false))
				return false;

			if (_model.IsOpen && !DoClose())
				return false;

			return DoLoad(ofd.FileName);
		}
		
		private bool DoLoad(String path)
		{
			if (StringUtils.IsEmpty(path))
				return false;
				
			if (!System.IO.File.Exists(path))
				return false;
				
			_model.Load(path);
			    _topics.DataContext = _model.Grammar.RootTopic;
			
			if (_topics.Items.Count > 0)
			{
				TreeViewItem item = (TreeViewItem)_topics.ItemContainerGenerator.ContainerFromIndex(0);
				item.Focus();
			}
			else
			{
				_topics.Focus();
			}
			
			return true;
		}
		
		#endregion
		
		#region Commands
		
        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanFileNew(Object sender, CanExecuteRoutedEventArgs e)
		{
            e.CanExecute = (_model != null);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnFileNew(Object sender, ExecutedRoutedEventArgs e)
		{
			DoNew();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanFileClose(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (_model != null && _model.IsOpen);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnFileClose(Object sender, ExecutedRoutedEventArgs e)
		{
			DoClose();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanFileOpen(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (_model != null);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnFileOpen(Object sender, ExecutedRoutedEventArgs e)
		{
            try
            {
                DoOpen();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, Strings.WINDOW_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanFileProperties(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (_model != null && _model.IsOpen);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnFileProperties(Object sender, ExecutedRoutedEventArgs e)
		{
			DoEdit();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanFileSave(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (_model != null && _model.IsOpen && _model.IsDirty);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnFileSave(Object sender, ExecutedRoutedEventArgs e)
		{
            TreeViewItem topicItem = (TreeViewItem)_topics.Tag;
            TopicModel topic = topicItem != null ? (TopicModel)topicItem.DataContext : null;
            RuleItemModel rule = (RuleItemModel)_rules.SelectedItem;
            bool selectRule = _rules.IsKeyboardFocusWithin;
			
			DoSave();

            if (topic != null)
            {
                WPFUtils.SelectTreeViewItemAsync(_topics, topic);
                if (selectRule && rule != null)
                    WPFUtils.SelectListBoxItemAsync(_rules, rule, DispatcherPriority.ApplicationIdle);
            }
        }

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanFileSaveAs(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (_model != null && _model.IsOpen);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnFileSaveAs(Object sender, ExecutedRoutedEventArgs e)
		{
            TreeViewItem topicItem = (TreeViewItem)_topics.Tag;
            TopicModel topic = topicItem != null ? (TopicModel)topicItem.DataContext : null;
            RuleItemModel rule = (RuleItemModel)_rules.SelectedItem;
            bool selectRule = _rules.IsKeyboardFocusWithin;

            DoSaveAs();

            if (topic != null)
            {
                WPFUtils.SelectTreeViewItemAsync(_topics, topic);
				if (selectRule && rule != null)
                    WPFUtils.SelectListBoxItemAsync(_rules, rule, DispatcherPriority.ApplicationIdle);
            }
        }

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanFileExit(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnFileExit(Object sender, ExecutedRoutedEventArgs e)
		{
			Close();

			// TODO: update focus
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanTopicEdit(Object sender, CanExecuteRoutedEventArgs e)
		{
			TreeView tree = e.Source as TreeView;
			e.CanExecute = (_model.IsOpen && tree != null && tree.SelectedItem != null);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnTopicEdit(Object sender, ExecutedRoutedEventArgs e)
		{
			TreeView tree = (TreeView)e.Source;
			TreeViewItem item = (TreeViewItem)tree.Tag;
			TopicModel model = (TopicModel)item.DataContext;
			using (ITransaction t = _model.TransactionContext.BeginTransaction())
			{
				using (Validator v = new TopicValidator(model.RowView))
				{
					TopicWnd wnd = new TopicWnd(model, v);
					wnd.Owner = this;
					if (wnd.ShowDialog() ?? false)
					{
						t.Success = true;
						item.Focus();
						model.NotifyPropertyChanged();
					}
				}
			}
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanTopicNew(Object sender, CanExecuteRoutedEventArgs e)
		{
			TreeView tree = e.Source as TreeView;
			e.CanExecute = (_model.IsOpen && tree != null);
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnTopicNew(Object sender, ExecutedRoutedEventArgs e)
		{
			TreeView tree = (TreeView)e.Source;
			ItemsControl parent = (tree.SelectedItem == null) ? tree : ItemsControl.ItemsControlFromItemContainer((TreeViewItem)tree.Tag);
			OnTopicNewHelper(parent);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanTopicNewChild(Object sender, CanExecuteRoutedEventArgs e)
		{
			TreeView tree = e.Source as TreeView;
			e.CanExecute = (_model.IsOpen && tree != null && tree.SelectedItem != null);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnTopicNewChild(Object sender, ExecutedRoutedEventArgs e)
		{
			TreeView tree = (TreeView)e.Source;
			TreeViewItem item = tree.Tag as TreeViewItem;
			item.IsExpanded = true;
			OnTopicNewHelper(item);
		}
		
		private void OnTopicNewHelper(ItemsControl parentItem)
		{
			TopicModel parentTopic = (TopicModel)parentItem.DataContext;
			IPresentationList<TopicModel> presentationList = (IPresentationList<TopicModel>)parentTopic.Topics;
			IBindingList list = (IBindingList)presentationList;
			using (ITransaction t = _model.TransactionContext.BeginTransaction())
			{
				TopicModel topic = (TopicModel)list.AddNew();
				topic.Title = "New topic";
				using (Validator v = new TopicValidator(topic.RowView))
				{
					TopicWnd wnd = new TopicWnd(topic, v);
					wnd.Owner = this;
					if (wnd.ShowDialog() ?? false)
					{
						t.Success = true;
						TreeViewItem item = (TreeViewItem)parentItem.ItemContainerGenerator.ContainerFromItem(topic);
						topic = (TopicModel)item.DataContext;
						topic.NotifyPropertyChanged();
						item.Focus();
					}
				}
			}
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanTopicDelete(Object sender, CanExecuteRoutedEventArgs e)
		{
			TreeView tree = e.Source as TreeView;
			e.CanExecute = (_model.IsOpen && tree != null && tree.SelectedItem != null);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnTopicDelete(Object sender, ExecutedRoutedEventArgs e)
		{
			TreeView tree = (TreeView)e.Source;
			TreeViewItem item = tree.Tag as TreeViewItem;
			TopicModel topic = (TopicModel)item.DataContext;
			ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(item);
			TopicModel parentTopic = parent.DataContext as TopicModel;
			IPresentationList<TopicModel> presentationList = (IPresentationList<TopicModel>)parentTopic.Topics;
			IBindingList list = (IBindingList)presentationList;

			if (MessageBox.Show("Are you sure want to delete selected item?", Strings.WINDOW_TITLE, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
			{
				try
				{
					int index = list.IndexOf(topic);
					list.Remove(topic);
					
					Control focus = null;
					if (list.Count == 0)
						focus = parent;
					else if (index < list.Count)
						focus = (Control)parent.ItemContainerGenerator.ContainerFromIndex(index);
					else
						focus = (Control)parent.ItemContainerGenerator.ContainerFromIndex(list.Count - 1);
						
					focus.Focus();
				}
				catch (InvalidConstraintException)
				{
					MessageBox.Show("This item cannot be deleted because it is already refered to.", Strings.WINDOW_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
				}
				catch (Exception err)
				{
					MessageBox.Show(err.Message, Strings.WINDOW_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanTopicUp(Object sender, CanExecuteRoutedEventArgs e)
		{
			TreeView tree = e.Source as TreeView;
			if (_model.IsOpen && tree != null && tree.SelectedItem != null)
			{
				TreeViewItem item = (TreeViewItem)tree.Tag;
				ItemsControl parent = (ItemsControl)ItemsControl.ItemsControlFromItemContainer(item);
				int index = parent.ItemContainerGenerator.IndexFromContainer(item);
				e.CanExecute = (index > 0);
			}
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnTopicUp(Object sender, ExecutedRoutedEventArgs e)
		{
			TreeView tree = e.Source as TreeView;
			TreeViewItem item = (TreeViewItem)tree.Tag;
			ItemsControl parent = (ItemsControl)ItemsControl.ItemsControlFromItemContainer(item);
			int index = parent.ItemContainerGenerator.IndexFromContainer(item);
			TopicModel topic = (TopicModel)parent.DataContext;
			IBindingList list = (IBindingList)topic.Topics.List;
			AdoUtils.Swap(list, Strings.COL_POSITION, index, index - 1);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanTopicDown(Object sender, CanExecuteRoutedEventArgs e)
		{
			TreeView tree = e.Source as TreeView;
			if (_model.IsOpen && tree != null && tree.SelectedItem != null)
			{
				TreeViewItem item = (TreeViewItem)tree.Tag;
				ItemsControl parent = (ItemsControl)ItemsControl.ItemsControlFromItemContainer(item);
				int index = parent.ItemContainerGenerator.IndexFromContainer(item);
				e.CanExecute = (index < parent.Items.Count - 1);
			}
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnTopicDown(Object sender, ExecutedRoutedEventArgs e)
		{
			TreeView tree = e.Source as TreeView;
			TreeViewItem item = (TreeViewItem)tree.Tag;
			ItemsControl parent = (ItemsControl)ItemsControl.ItemsControlFromItemContainer(item);
			int index = parent.ItemContainerGenerator.IndexFromContainer(item);
			TopicModel topic = (TopicModel)parent.DataContext;
			IBindingList list = (IBindingList)topic.Topics.List;
			AdoUtils.Swap(list, Strings.COL_POSITION, index, index + 1);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanRuleNew(Object sender, CanExecuteRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnCanNew(sender, e);			
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnRuleNew(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnNew<RuleItemModel, RuleEditModel, RuleWnd, RuleValidator>(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanRuleEdit(Object sender, CanExecuteRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnCanEdit(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnRuleEdit(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnEdit<RuleItemModel, RuleEditModel, RuleWnd, RuleValidator>(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanRuleDelete(Object sender, CanExecuteRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnCanDelete(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnRuleDelete(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnDelete(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanRuleUp(Object sender, CanExecuteRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnCanMove<RuleItemModel>(sender, e, -1);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnRuleUp(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnMove<RuleItemModel>(sender, e, -1, Strings.COL_POSITION);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanRuleDown(Object sender, CanExecuteRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnCanMove<RuleItemModel>(sender, e, 1);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnRuleDown(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnMove<RuleItemModel>(sender, e, 1, Strings.COL_POSITION);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanCutTopic(Object sender, CanExecuteRoutedEventArgs e)
		{
            TreeView tree = (TreeView)sender;
            TreeViewItem item = (TreeViewItem)tree.Tag;
            e.CanExecute = (item != null);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
        private void OnCutTopic(Object sender, ExecutedRoutedEventArgs e)
		{
			;
		}

		[System.Reflection.Obfuscation(Exclude = true)]
        private void OnCanCopyTopic(Object sender, CanExecuteRoutedEventArgs e)
		{
            TreeView tree = sender as TreeView;
            if (tree != null)
            {
                TreeViewItem item = tree.Tag as TreeViewItem;
                if (item != null)
                {
                    IBinarySerializable bs = item.DataContext as IBinarySerializable;
                    if (bs != null)
                    {
                        e.CanExecute = true;
                    }
                }
            }
        }

		[System.Reflection.Obfuscation(Exclude = true)]
        private void OnCopyTopic(Object sender, ExecutedRoutedEventArgs e)
		{
            TreeView tree = (TreeView)sender;
            TreeViewItem item = (TreeViewItem)tree.Tag;
            IBinarySerializable bs = (IBinarySerializable)item.DataContext;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    bs.Serialize(writer);
                    Clipboard.SetData("Grammar:Topic", stream.ToArray());
                }
            }
		}

		[System.Reflection.Obfuscation(Exclude = true)]
        private void OnCanPasteTopic(Object sender, CanExecuteRoutedEventArgs e)
		{
            if (Clipboard.ContainsData("Grammar:Topic"))
            {
                TreeView tree = sender as TreeView;
                if (tree != null)
                {
                    ItemsControl parent = (tree.SelectedItem == null) ? tree : ItemsControl.ItemsControlFromItemContainer((TreeViewItem)tree.Tag);
                    if (parent != null)
                    {
                        IPresentationList<TopicModel> ptopics = parent.ItemsSource as IPresentationList<TopicModel>;
                        if (ptopics != null)
                        {
                            IBindingList btopics = ptopics as IBindingList;
                            if (btopics != null)
                            {
                                ITransactionContextAware tca = tree.DataContext as ITransactionContextAware;
                                if (tca != null)
                                {
                                    e.CanExecute = true;
                                }
                            }
                        }
                    }
                }
            }
        }

		[System.Reflection.Obfuscation(Exclude = true)]
        private void OnPasteTopic(Object sender, ExecutedRoutedEventArgs e)
		{
            TreeView tree = (TreeView)sender;
            ItemsControl parent = (tree.SelectedItem == null) ? tree : ItemsControl.ItemsControlFromItemContainer((TreeViewItem)tree.Tag);
            IPresentationList<TopicModel> ptopics = (IPresentationList<TopicModel>)parent.ItemsSource;
            IBindingList btopics = (IBindingList)ptopics;
            ITransactionContextAware tca = (ITransactionContextAware)tree.DataContext;
            ITransactionContext context = tca.TransactionContext;
            using (ITransaction t = context.BeginTransaction())
            {
                using (MemoryStream stream = new MemoryStream((byte[])Clipboard.GetData("Grammar:Topic")))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        TopicModel topic = (TopicModel)btopics.AddNew();
                        IBinarySerializable bserializable = (IBinarySerializable)topic;
                        bserializable.Deserialize(reader);
                        t.Success = true;
                        TreeViewItem item = (TreeViewItem)parent.ItemContainerGenerator.ContainerFromItem(topic);
                        topic = (TopicModel)item.DataContext;
                        topic.NotifyPropertyChanged();
                        item.Focus();
                    }
                }
            }
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanCutRule(Object sender, CanExecuteRoutedEventArgs e)
		{
            WPFUtils.ListBox_OnCanCut(sender, e);
        }

		[System.Reflection.Obfuscation(Exclude = true)]
        private void OnCutRule(Object sender, ExecutedRoutedEventArgs e)
		{
            WPFUtils.ListBox_OnCut(sender, e, "Grammar:Rule");
		}

		[System.Reflection.Obfuscation(Exclude = true)]
        private void OnCanCopyRule(Object sender, CanExecuteRoutedEventArgs e)
		{
            WPFUtils.ListBox_OnCanCopy(sender, e);
        }

		[System.Reflection.Obfuscation(Exclude = true)]
        private void OnCopyRule(Object sender, ExecutedRoutedEventArgs e)
		{
            WPFUtils.ListBox_OnCopy(sender, e, "Grammar:Rule");
		}

		[System.Reflection.Obfuscation(Exclude = true)]
        private void OnCanPasteRule(Object sender, CanExecuteRoutedEventArgs e)
		{
            WPFUtils.ListBox_OnCanPaste<RuleItemModel>(sender, e, "Grammar:Rule");
        }

		[System.Reflection.Obfuscation(Exclude = true)]
        private void OnPasteRule(Object sender, ExecutedRoutedEventArgs e)
		{
            WPFUtils.ListBox_OnPaste<RuleItemModel>(sender, e, "Grammar:Rule");
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanPractice(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (_model != null && _model.IsOpen && _model.ExamplesCount > 0 && _topics.SelectedItem != null);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnPractice(Object sender, ExecutedRoutedEventArgs e)
		{
			using (PracticeModel model = new PracticeModel((TopicModel)_topics.SelectedItem))
			{
				if (model.IsEmpty)
				{
					MessageBox.Show("Selected topic does not contain active examples to exercise", Strings.WINDOW_TITLE, MessageBoxButton.OK, MessageBoxImage.Warning); 
				}
				else
				{
					PracticeWnd wnd = new PracticeWnd(model);
					wnd.Owner = this;
					wnd.ShowDialog();
				}
			}
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanToggleActive(Object sender, CanExecuteRoutedEventArgs e)
		{
			ListBox listBox = e.Source as ListBox;
			e.CanExecute = (listBox != null && listBox.SelectedItem != null);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnToggleActive(Object sender, ExecutedRoutedEventArgs e)
		{
			ListBox listBox = (ListBox)e.Source;
			RuleItemModel model = (RuleItemModel)listBox.SelectedItem;
			model.Active = !model.Active;
			model.NotifyPropertyChanged();
		}
		
		#endregion

        private void OnRulesDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _rules.Dispatcher.BeginInvoke(DispatcherPriority.Background, 
                (Func<bool>) delegate() 
                             {
                                 if (_rules.HasItems)
                                     _rules.ScrollIntoView(_rules.Items[0]);
                                 return true;  
                             });
        }
	}
}
