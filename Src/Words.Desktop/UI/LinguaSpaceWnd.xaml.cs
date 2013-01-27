using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.Data;

using Microsoft.Win32;

using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Common.Data;
using LinguaSpace.Common.Text;
using LinguaSpace.Common.UI;
using LinguaSpace.Common.Licensing;
using LinguaSpace.Words.IO;
using LinguaSpace.Words.Data;
using LinguaSpace.Words.Resources;
using LinguaSpace.Words.Practice;
using LinguaSpace.Words.ComponentModel;

namespace LinguaSpace.Words.UI
{
    public partial class LinguaSpaceWnd : System.Windows.Window 
    {
		private LinguaSpaceModel model;
		private bool meaningsReady = true;
		private Guid meaningGuid = Guid.Empty;
        private ListBoxHistory history;

        public LinguaSpaceWnd()
        {
            /*
			_rdm = new RemoteDeviceManager();
			_sync = new SyncServer(FileUtils.VocabulariesFolder, "*.lsv");
             */
            
            RegistrationService.Service.Initialize();

            String keyCodePath = FileUtils.KeyCodeFile;
            if (System.IO.File.Exists(keyCodePath))
            {
                RegistrationService.Service.Registration.KeyCode = StringUtils.Bytes2Str(FileUtils.ReadBytes(keyCodePath));
            }

            this.Loaded += new RoutedEventHandler(OnLoad);
            this.Closing += new CancelEventHandler(OnClosing);

            InitializeComponent();

            this.history = new ListBoxHistory(this.listWords);

			this.GotFocus += new RoutedEventHandler(LinguaSpaceWnd_GotFocus);
		}

		#region Event Handlers

		private void RemoteDeviceManager_DeviceDisconnected(Object sender, EventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();
		}

		private void RemoteDeviceManager_OnDeviceConnected(Object sender, EventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();
		}

		private void LinguaSpaceWnd_GotFocus(Object sender, RoutedEventArgs e)
		{
			InputLanguageUtils.SetDefaultInputLanguage();
		}

		private void OnLoad(Object sender, RoutedEventArgs e)
		{
			this.model = new LinguaSpaceModel(new LinguaSpaceDataSet());

			String profilePath = FileUtils.GetLastUsedFile(FileUtils.ProfilesFolder, "*.lsp");
			if (profilePath != null)
			{
				DoLoadProfile(profilePath);
			}
			else
			{
                DoFirstProifle();
			}

			this.DataContext = this.model;

            this.listWords.MouseDoubleClick += new MouseButtonEventHandler(listWords_MouseDoubleClick);
			this.listWords.SelectionChanged += new SelectionChangedEventHandler(listWords_SelectionChanged);
			
			this.listMeanings.ItemsSource = this.model.Meanings;
			this.listWords.ItemsSource = this.model.Words;
			
			if (this.listWords.HasItems)
				WPFUtils.SelectListBoxItemAsync(this.listWords, 0);

            /*
			_sync.StartUp();
			_rdm.DeviceConnected += new EventHandler(RemoteDeviceManager_OnDeviceConnected);
			_rdm.DeviceDisconnected += new EventHandler(RemoteDeviceManager_DeviceDisconnected);
             */ 
        }

		private void listWords_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if (this.meaningsReady)
			{
				this.meaningsReady = false;
				Dispatcher.CurrentDispatcher.BeginInvoke(new Func<bool>(FilterMeanings), DispatcherPriority.DataBind);
			}
		}
				
		private bool FilterMeanings()
		{
			this.meaningsReady = true;
			WordItemModel word = (WordItemModel)this.listWords.SelectedItem;
			if (word != null)
			{
				DataRowView wordRow = (DataRowView)word.Data;
				IPresentationList<MeaningItemModel> meaningsModel = (IPresentationList<MeaningItemModel>)this.listMeanings.ItemsSource;
				FilterBindingList filteredMeanings = (FilterBindingList)meaningsModel.List;
				filteredMeanings.FilterValue = wordRow["WordId"];
				
				if (this.meaningGuid != Guid.Empty)
				{
					MeaningItemModel meaning = meaningsModel.FirstOrDefault<MeaningItemModel>( m => m.MeaningId == this.meaningGuid );
					if (meaning != null)
					{
						WPFUtils.SelectListBoxItemAsync(this.listMeanings, meaning);
					}
					this.meaningGuid = Guid.Empty;
				}
			}
			return this.meaningsReady;
		}

		private void OnClosing(Object sender, CancelEventArgs e)
		{
			if (this.model.IsProfileOpen)
				e.Cancel = !DoCloseProfile();
		}

		void OnFlashCardsWndLoaded(Object sender, RoutedEventArgs e)
		{
			this.Hide();
       	}

		void OnFlashCardsWndClosed(object sender, EventArgs e)
		{
			this.Show();
			this.Activate();
		}

		#endregion

		#region Command Atoms

		private MessageBoxResult DoPromptSaveVocabulary()
		{
			return this.model.IsVocabularyDirty 
				? MessageBox.Show(this, String.Format("Do you want to save vocabulary '{0}'", this.model.Vocabulary.Name), this.Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes)
				: MessageBoxResult.None;
		}

		private String DoAskSaveFileName()
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.InitialDirectory = FileUtils.VocabulariesFolder;
			sfd.Filter = "LinguaSpace Vocabulary Files|*.lsv|All Files|*.*";
			if (sfd.ShowDialog(this) ?? false)
				return sfd.FileName;
			return String.Empty;
		}

		private bool DoSaveVocabularyAs()
		{
			Debug.Assert(this.model.IsVocabularyOpen);

			String path = DoAskSaveFileName();
			if (StringUtils.IsEmpty(path))
				return false;
			
			if (System.IO.File.Exists(path))
				System.IO.File.Delete(path);

			this.model.SaveVocabularyAs(path);
			return true;
		}

		private bool DoSaveVocabulary()
		{
			Debug.Assert(this.model.IsVocabularyOpen);
            
			if (StringUtils.IsEmpty(this.model.VocabularyFileName))
				return DoSaveVocabularyAs();
			
			this.model.SaveVocabulary();
			
			return true;
		}

		private bool DoCloseVocabulary()
		{			
			Debug.Assert(this.model.IsVocabularyOpen);
		
			MessageBoxResult mbr = DoPromptSaveVocabulary();
			if (mbr == MessageBoxResult.Cancel)
				return false;
			
			if (mbr == MessageBoxResult.Yes)
				DoSaveVocabulary();

			this.model.CloseVocabulary();

			return true;;
		}

		private bool DoEditVocabulary()
		{
			Debug.Assert(this.model.IsVocabularyOpen);

			bool done = false;

			using (ITransaction t = this.model.TransactionContext.BeginTransaction())
			{
				using (Validator validator = new VocabularyValidator((DataRowView)model.Vocabulary.Data))
				{
					VocabularyWnd vpw = new VocabularyWnd(model.Vocabulary, validator);
					vpw.Owner = this;
					if (vpw.ShowDialog() ?? false)
					{
						done = true;
						t.Success = true;
						model.NotifyPropertyChanged();
					}
				}
			}

			return done;
		}

		private bool DoNewVocabulary()
		{
			bool done = false;

			if (!this.model.IsVocabularyOpen || DoCloseVocabulary())
			{
				using (ITransaction t = this.model.TransactionContext.BeginTransaction())
				{
					IBindingList bindingList = (IBindingList)this.model.Vocabularies;
					bindingList.AddNew();
					t.Success = DoEditVocabulary();
					done = t.Success;
					model.NotifyPropertyChanged();
				}
			}

			return done;
		}

		private bool DoOpenVocabulary()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.InitialDirectory = FileUtils.VocabulariesFolder;
			ofd.Filter = "LinguaSpace Vocabularies (*.lsv)|*.lsv|All Files|*.*";
			if (!(ofd.ShowDialog(this) ?? false))
				return false;

			if (this.model.IsVocabularyOpen && !DoCloseVocabulary())
				return false;
			
			return DoLoadVocabulary(ofd.FileName);				
		}

		private bool DoLoadVocabulary(String path)
		{
			if (StringUtils.IsEmpty(path))
				return false;
				
			if (!System.IO.File.Exists(path))
				return false;
				
			this.model.LoadVocabulary(path);

			if (this.listWords.HasItems)
				WPFUtils.SelectListBoxItemAsync(this.listWords, 0);

			return true;
		}

		private bool DoSaveProfileAs()
		{
			Debug.Assert(this.model != null);
			Debug.Assert(this.model.IsProfileOpen);
			
			String path = FileUtils.ProfilesFolder;
			path = System.IO.Path.Combine(path, this.model.Profile.Name);
			path = System.IO.Path.ChangeExtension(path, "lsp");
			
			if (System.IO.File.Exists(path))
				System.IO.File.Delete(path);
				
			this.model.SaveProfileAs(path);
			
			return true;
		}

		private bool DoSaveProfile()
		{
			Debug.Assert(this.model.IsProfileOpen);
			
			if (StringUtils.IsEmpty(this.model.ProfileFileName))
				return DoSaveProfileAs();

			this.model.SaveProfile();
			return true;
		}

		private bool DoCloseProfile()
		{
			Debug.Assert(this.model.IsProfileOpen);

			if (!DoSaveProfile())
				return false;
			
			if (this.model.IsVocabularyOpen)
				if (!DoCloseVocabulary())
					return false;
			
			this.model.CloseProfile();
			
			return true;
		}

		private bool DoEditProfile()
		{
			Debug.Assert(this.model != null);
			Debug.Assert(this.model.IsProfileOpen);
			
			bool done = false;
			
			using (ITransaction t = this.model.TransactionContext.BeginTransaction())
			{
				using (Validator validator = new ProfileValidator((DataRowView)model.Profile.Data))
				{
					ProfileWnd wnd = new ProfileWnd(this.model.Profile, validator);
					wnd.Owner = this;
					if (wnd.ShowDialog() ?? false)
					{
						done = true;
						t.Success = true;
						model.NotifyPropertyChanged();
					}
				}
			}

			return done;
		}

		private bool DoNewProfile()
		{
			bool done = false;
			
			if (!this.model.IsProfileOpen || DoCloseProfile())
			{
				using (ITransaction t = this.model.TransactionContext.BeginTransaction())
				{
					IBindingList bindingList = (IBindingList)this.model.Profiles;
					bindingList.AddNew();
					t.Success = DoEditProfile();
					done = t.Success;
					model.NotifyPropertyChanged();
				}
			}

			return done;
		}

		private bool DoOpenProfile()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.InitialDirectory = FileUtils.ProfilesFolder;
			ofd.Filter = "LinguaSpace User Profiles (*.lsp)|*.lsp|All Files|*.*";
			if (!(ofd.ShowDialog(this) ?? false))
				return false;

			if (this.model.IsProfileOpen && !DoCloseProfile())
				return false;
				
			return DoLoadProfile(ofd.FileName);
		}

		private bool DoLoadProfile(String path)
		{
			if (StringUtils.IsEmpty(path))
				return false;
				
			if (!System.IO.File.Exists(path))
				return false;

			this.model.LoadProfile(path);

			DoLoadVocabulary(this.model.Profile.DefaultVocabularyFileName);
			
			return true;
		}
		
        private bool DoFirstProifle()
        {
			Debug.Assert(!this.model.IsProfileOpen);

			IBindingList bindingList = (IBindingList)this.model.Profiles;
			bindingList.AddNew();
			this.model.Profile.Name = Environment.UserName;
			this.model.Profile.DefaultVocabularyFileName = System.IO.Path.Combine(FileUtils.VocabulariesFolder, "German-Russian.lsv");
			return DoLoadVocabulary(this.model.Profile.DefaultVocabularyFileName);
        }

        private delegate void Arg0Delegate();

		/*
        public bool IsProfileExpired(IUserProfile profile)
        {
            DateTime now = DateTime.Now;
            return (profile.Saved > now || (now - profile.Created).Days > 30);
        }

        private void CheckProfileExpired()
        {
            Debug.Assert(this.profile != null);
            if (IsProfileExpired(this.profile))
            {
                MessageBox.Show(this, String.Format(StringRes.PROFILE_EXPIRED, profile.Name), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        */

        private bool DoShellOpen(String fileName)
        {
            Debug.Assert(!StringUtils.IsEmpty(fileName));
            Debug.Assert(System.IO.File.Exists(fileName));

            bool done = false;
            
            String fileExt = System.IO.Path.GetExtension(fileName);
            
            if (fileExt == ".lsv")
            {
                if (!this.model.IsVocabularyOpen || DoCloseVocabulary())
                {
                    done = DoLoadVocabulary(fileName);
                }
            }
			else if (fileExt == ".lsp")
            {
                if (DoSaveProfile())
				{
					done = DoLoadProfile(fileName);
				}
            }

            return done;
        }

		#endregion

		#region Commands

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanBack(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = this.model != null && this.history.CanBack();
		} 

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnBack(Object sender, ExecutedRoutedEventArgs e)
		{
			this.history.Back();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanForward(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = this.model != null && this.history.CanForward();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnForward(Object sender, ExecutedRoutedEventArgs e)
		{
			this.history.Forward();
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnCanGoToWord(Object sender, CanExecuteRoutedEventArgs e)
        {
			Debug.Assert(this.model != null);
            e.CanExecute = (this.model.IsVocabularyOpen && e.Parameter != null);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnGoToWord(Object sender, ExecutedRoutedEventArgs e)
        {
            /*
            IWord word = (IWord)e.Parameter;
            WPFUtils.SelectListBoxItemAsync(this.ctrlWords.listWords, word);
             */
        }

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanVocabularyNew(Object sender, CanExecuteRoutedEventArgs e)
		{
			Debug.Assert(this.model != null);
            e.CanExecute = (this.model != null && this.model.IsProfileOpen);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnVocabularyNew(Object sender, ExecutedRoutedEventArgs e)
		{
			DoNewVocabulary();
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanVocabularyClose(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (this.model != null && this.model.IsVocabularyOpen);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnVocabularyClose(Object sender, ExecutedRoutedEventArgs e)
		{
			DoCloseVocabulary();
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanVocabularyOpen(Object sender, CanExecuteRoutedEventArgs e)
		{
            e.CanExecute = (this.model != null && this.model.IsProfileOpen);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnVocabularyOpen(Object sender, ExecutedRoutedEventArgs e)
		{
			DoOpenVocabulary();
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanVocabularyProperties(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (this.model != null && this.model.IsVocabularyOpen);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnVocabularyProperties(Object sender, ExecutedRoutedEventArgs e)
		{
			DoEditVocabulary();
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanVocabularySave(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (model != null && model.IsVocabularyOpen && this.model.IsVocabularyDirty);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnVocabularySave(Object sender, ExecutedRoutedEventArgs e)
		{
			DoSaveVocabulary();

            if (this.listWords.SelectedItem != null) 
            {
                this.listWords.ScrollIntoView(this.listWords.SelectedItem);
                WPFUtils.SelectListBoxItemAsync(this.listWords, this.listWords.SelectedItem, DispatcherPriority.DataBind);
            }

            if (this.listMeanings.IsKeyboardFocused && this.listMeanings.SelectedItem != null)
            {
                WPFUtils.SelectListBoxItemAsync(this.listMeanings, this.listMeanings.SelectedItem, DispatcherPriority.DataBind);
            }
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanExit(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnExit(Object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnCanFindWord(Object sender, CanExecuteRoutedEventArgs e)
        {
			e.CanExecute = (model != null && model.IsVocabularyOpen && this.listWords.HasItems);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnFindWord(Object sender, ExecutedRoutedEventArgs e)
        {
			WordFinderModel finder = this.model.CreateWordFinder();
			WordFindWnd wnd = new WordFindWnd(finder, new WordFinderModelValidator(finder));
			wnd.Owner = this;
			if (wnd.ShowDialog() ?? false)
			{
				WordItemModel word = this.model.Words.FirstOrDefault<WordItemModel>( m => m.WordId == finder.WordId );
				if (word != null)
				{
					WPFUtils.SelectListBoxItemAsync(this.listWords, word);
				}
			}
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnCanFindMeaning(Object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (model != null && model.IsVocabularyOpen && this.listWords.HasItems);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnFindMeaning(Object sender, ExecutedRoutedEventArgs e)
        {
			MeaningFinderModel finder = this.model.CreateMeaningFinder();
			MeaningFindWnd wnd = new MeaningFindWnd(finder, new MeaningFinderModelValidator(finder));
			wnd.Owner = this;
			if (wnd.ShowDialog() ?? false)
			{
				WordItemModel word = this.model.Words.FirstOrDefault<WordItemModel>( w => w.WordId == finder.WordId );
				if (word != null)
				{
					this.meaningGuid = finder.MeaningId;
					WPFUtils.SelectListBoxItemAsync(this.listWords, word);
				}
			}
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnCanTouchMeaning(Object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.model != null && this.model.IsVocabularyOpen)
            {
                WPFUtils.ListBox_OnCanEdit(sender, e);
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnTouchMeaning(Object sender, ExecutedRoutedEventArgs e)
        {
            ListBox listBox = e.Source as ListBox;
            if (listBox != null) 
            {
                MeaningItemModel meaning = listBox.SelectedItem as MeaningItemModel;
                if (meaning != null)
                {
                    meaning.UpdateHistory();
                }
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanFlashCards(Object sender, CanExecuteRoutedEventArgs e)
		{
            e.CanExecute = (model != null && model.IsVocabularyOpen && this.model.IsProfileOpen);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnFlashCards(Object sender, ExecutedRoutedEventArgs e)
		{
			if (this.model.IsVocabularyDirty)
				OnVocabularySave(sender, e);
		
			DoSaveProfile();
				
			Dispatcher.BeginInvoke( (System.Windows.Forms.MethodInvoker)delegate()
			{
				IFlashCardsGenerator generator = new FlashCardsGenerator(this.model.VocabularyFileName, this.model.ProfileFileName);
				FlashCardsWnd fcw = new FlashCardsWnd(generator, 10);
				fcw.Loaded += new RoutedEventHandler(OnFlashCardsWndLoaded);
				fcw.Closed += new EventHandler(OnFlashCardsWndClosed);
				fcw.Show();
			}, DispatcherPriority.ApplicationIdle );				
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanUserNew(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (this.model != null);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnUserNew(Object sender, ExecutedRoutedEventArgs e)
		{
			DoNewProfile();
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanUserSettings(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (this.model != null && this.model.IsProfileOpen);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnUserSettings(Object sender, ExecutedRoutedEventArgs e)
		{
			DoEditProfile();
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanUserSwitch(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (this.model != null);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnUserSwitch(Object sender, ExecutedRoutedEventArgs e)
		{
			DoOpenProfile();
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanRegister(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnRegister(Object sender, ExecutedRoutedEventArgs e)
		{
			RegistrationService service = RegistrationService.Service;
			service.Initialize();
			IValidationStatusProvider vsp = new RegistrationServiceValidator(RegistrationService.Service);
			RegistrationWnd wnd = new RegistrationWnd(RegistrationService.Service, vsp);
			wnd.Owner = this;
			if ((bool)wnd.ShowDialog())
			{
                Debug.Assert(service.Registration.KeyCode != null);
                if (System.IO.File.Exists(FileUtils.KeyCodeFile))
                {
                    System.IO.File.Delete(FileUtils.KeyCodeFile);
                }
                
                FileUtils.WriteBytes(FileUtils.KeyCodeFile, StringUtils.Str2Bytes(service.Registration.KeyCode));
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnCanQuickStart(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnQuickStart(Object sender, ExecutedRoutedEventArgs e)
		{
            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = FileUtils.HelpFile;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                proc.Start();
            }
            catch (Exception err)
            {
                MessageBox.Show(this, err.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnCanAbout(Object sender, CanExecuteRoutedEventArgs e)
        {
            String[] formats = Clipboard.GetDataObject().GetFormats();
            e.CanExecute = true;
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnAbout(Object sender, ExecutedRoutedEventArgs e)
        {
            AboutWnd aboutWnd = new AboutWnd();
            aboutWnd.Owner = this;
            aboutWnd.ShowDialog();
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnCanShellOpen(Object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.IsVisible;
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnShellOpen(Object sender, ExecutedRoutedEventArgs e)
        {
            String fileName = (String)e.Parameter;
            Debug.Assert(!StringUtils.IsEmpty(fileName));
            Debug.Assert(System.IO.File.Exists(fileName));
            DoShellOpen(fileName);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnDrag(Object sender, DragEventArgs e)
        {
            String fileName = DragDropUtils.GetFileName(e.Data);
            String fileExt = System.IO.Path.GetExtension(fileName);
			e.Effects = (fileExt == ".lsp" || fileExt == ".lsv") ? DragDropEffects.Move : DragDropEffects.None;
            e.Handled = true;
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnDrop(Object sender, DragEventArgs e)
        {
            String fileName = DragDropUtils.GetFileName(e.Data);
            e.Handled = DoShellOpen(fileName);
        }

        /*
        private void Connect()
        {
            LSRAPI.IRAPIUtils rapi = new LSRAPI.RAPIUtilsClass();
            LSRAPI.ConnectionInfo connInfo = rapi.GetConnectionInfo();

            IPAddress ipDesktop = new IPAddress(connInfo.desktop.ip);
            IPAddress ipDevice = new IPAddress(connInfo.device.ip);

            TcpClient client = new TcpClient();
            client.Connect(ipDevice, 1945);

            NetworkStream stream = client.GetStream();
            String msg = "hi, baby!";
            byte[] bufferMsg = Encoding.UTF8.GetBytes(msg);
            stream.Write(bufferMsg, 0, bufferMsg.Length);
            byte[] bufferRsp = new byte[256];
            int length = stream.Read(bufferRsp, 0, 256);
            String rsp = Encoding.UTF8.GetString(bufferRsp, 0, length);
            stream.Close();
        }
         */

		// Edit
		private void listWords_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
		{
			EditCommands.Edit.Execute(e, (IInputElement)sender);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanDelete(Object sender, CanExecuteRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnCanDelete(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnDelete(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnDelete(sender, e);
 		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanNewWord(Object sender, CanExecuteRoutedEventArgs e)
		{
			if (this.model != null && this.model.IsVocabularyOpen)
				WPFUtils.ListBox_OnCanNew(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnNewWord(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnNew<WordItemModel, WordEditModel, WordWnd, WordValidator>(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanEditWord(Object sender, CanExecuteRoutedEventArgs e)
		{
			if (this.model != null && this.model.IsVocabularyOpen)
				WPFUtils.ListBox_OnCanEdit(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnEditWord(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnEdit<WordItemModel, WordEditModel, WordWnd, WordValidator>(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanNewMeaning(Object sender, CanExecuteRoutedEventArgs e)
		{
			if (this.model != null && this.model.IsVocabularyOpen)
				WPFUtils.ListBox_OnCanNew(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnNewMeaning(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnNew<MeaningItemModel, MeaningEditModel, MeaningWnd, MeaningValidator>(sender, e);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanEditMeaning(Object sender, CanExecuteRoutedEventArgs e)
		{
			if (this.model != null && this.model.IsVocabularyOpen)
				WPFUtils.ListBox_OnCanEdit(sender, e);
		}
        
		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnEditMeaning(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnEdit<MeaningItemModel, MeaningEditModel, MeaningWnd, MeaningValidator>(sender, e);
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanMoveUpMeaning(Object sender, CanExecuteRoutedEventArgs e)
		{
			if (this.model != null && this.model.IsVocabularyOpen)
				WPFUtils.ListBox_OnCanMove<MeaningItemModel>(sender, e, -1);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnMoveUpMeaning(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnMove<MeaningItemModel>(sender, e, -1, Strings.POSITION);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanMoveDownMeaning(Object sender, CanExecuteRoutedEventArgs e)
		{
			if (this.model != null && this.model.IsVocabularyOpen) 
				WPFUtils.ListBox_OnCanMove<MeaningItemModel>(sender, e, 1);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnMoveDownMeaning(Object sender, ExecutedRoutedEventArgs e)
		{
			WPFUtils.ListBox_OnMove<MeaningItemModel>(sender, e, 1, Strings.POSITION);
		}

		#endregion
	}
}
