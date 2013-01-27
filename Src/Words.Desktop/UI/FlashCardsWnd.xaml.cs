using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Diagnostics;

using LinguaSpace.Common.Events;
using LinguaSpace.Common.Data;
using LinguaSpace.Common.UI;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Words.Practice;

namespace LinguaSpace.Words.UI
{
	public class FlashCardItemModel : PresentationModel
	{
		public FlashCardItemModel(IFlashCardItem item)
			: base(item)
		{
			NPCWeakEventManager.AddListener((INotifyPropertyChanged)item, this);
		}
		
		public new IFlashCardItem Data
		{
			get
			{
				return (IFlashCardItem)base.Data;
			}
		}
		
		public String Caption
		{
			get
			{
				return Data.Caption;
			}
		}
		
		public String Text
		{
			get
			{
				return Data.Text;
			}
		}
		
		public int FontSize
		{
			get
			{
				return 16;
			}
		}
		
		public Brush Color
		{
			get
			{
				Brush brush = SystemColors.WindowTextBrush;
				switch (Data.Status)
				{
					case FlashCardItemStatus.Right:
						brush = Brushes.Green;
						break;
					case FlashCardItemStatus.Wrong:
						brush = Brushes.Red;
						break;
					case FlashCardItemStatus.Other:
						brush = SystemColors.ControlBrush;
						break;
				}
				return brush;
			}
		}

		protected override void OnPropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Status")
			{
				RaisePropertyChangedEvent("Color");
			}
			else
			{
				base.OnPropertyChanged(sender, e);
			}
		}
	}

	public class FlashCardModel : PresentationModel
	{
		private static readonly ImageSource BmpsQuestion32;
		private static readonly ImageSource BmpsWarning32;
		private static readonly ImageSource BmpsError32;
		private static readonly ImageSource BmpsOK32;

		static FlashCardModel()
		{
			BmpsError32 = WPFUtils.GetPngImageSource("Words", "Status", "Error", 32);
			BmpsQuestion32 = WPFUtils.GetPngImageSource("Words", "Status", "Question", 32);
			BmpsWarning32 = WPFUtils.GetPngImageSource("Words", "Status", "Warning", 32);
			BmpsOK32 = WPFUtils.GetPngImageSource("Words", "Status", "OK", 32);
		}
	
		private PresentationList<FlashCardItemModel> _answers;
	
		public FlashCardModel(IFlashCard card)
			: base(card)
		{
			IList list = new ArrayList();
			foreach (IFlashCardItem item in card.Answers)
				list.Add(item);
			_answers = new PresentationList<FlashCardItemModel>(list);
			card.StatusChanged += new StatusChangedHandler(card_StatusChanged);
		}

		private void card_StatusChanged(object sender, StatusEventArgs args)
		{
			RaisePropertyChangedEvent("Example");
			RaisePropertyChangedEvent("StatusText");
			RaisePropertyChangedEvent("StatusImage");
		}
		
		public new IFlashCard Data
		{
			get
			{
				return (IFlashCard)base.Data;
			}
		}

		public String QuestionCaption
		{
			get
			{
				return this.Data.Question.Caption;
			}
		}

		public FlashCardItemModel Question
		{
			get
			{
				return new FlashCardItemModel(Data.Question);
			}
		}

		public String AnswersCaption
		{
			get
			{
				return this.Data.Answers.First<IFlashCardItem>( x => x.Equals(this.Data.Question) ).Caption;
			}
		}

		public Brush CaptionColor
		{
			get
			{
				Brush brush = Brushes.Gray;
				IFlashCardItem item = this.Data.Answers.First<IFlashCardItem>( x => x.Equals(this.Data.Question) );
				if (item != null)
				{
					switch (item.Type)
					{
						case FlashCardItemType.Translations:
						case FlashCardItemType.Word:
						case FlashCardItemType.Synonyms:
							brush = Brushes.Green;
							break;
						case FlashCardItemType.Antonyms:
							brush = Brushes.Red;
							break;
						case FlashCardItemType.Definition:
							brush = Brushes.Navy;
							break;
					}
				}
				return brush;
			}
		}

		public IPresentationList<FlashCardItemModel> Answers
		{
			get
			{
				return _answers;
			}
		}

		public FlowDocument Example
		{
			get
			{
				if (Data.Status == FlashCardStatus.Question)
					return null;

				FlowDocument doc = FlowDocumentUtils.Text2FlowDocument(Data.Example, false, Brushes.Green);
				doc.FontFamily = new FontFamily("Verdana");
				doc.FontSize = 13;
				doc.PagePadding = new Thickness(5);
				return doc;
			}
		}

		public String StatusText
		{
			get
			{
				String text = String.Empty;
				switch (Data.Status)
				{
					case FlashCardStatus.Question:
						text = "Please choose the answer";
						break;
					case FlashCardStatus.Prompt:
						text = "Here is the right answer";
						break;
					case FlashCardStatus.Wrong:
						text = "Your answer is wrong";
						break;
					case FlashCardStatus.Right:
						text = "Your answer is right";
						break;
				}
				return text;
			}		
		}

		public ImageSource StatusImage
		{
			get
			{
				ImageSource imsrc = null;
				switch (Data.Status)
				{
					case FlashCardStatus.Question:
						imsrc = BmpsQuestion32;
						break;
					case FlashCardStatus.Prompt:
						imsrc = BmpsWarning32;
						break;
					case FlashCardStatus.Wrong:
						imsrc = BmpsError32;
						break;
					case FlashCardStatus.Right:
						imsrc = BmpsOK32;
						break;
				}
				return imsrc;
			}
		}

		public void Answer(FlashCardItemModel item)
		{
			Data.Answer(item.Data);
		}

		public void Prompt()
		{
			Data.Prompt();
		}
	}

	public partial class FlashCardsWnd : System.Windows.Window
	{
		private const int HOT_KEY_ID = 777;
		
		private System.Windows.Forms.NotifyIcon _notifyIcon;

		private DispatcherTimer _timerSleep;
		private DispatcherTimer _timerSafeFreeze;

		private IFlashCardsGenerator _generator;
		private IFlashCard _card;

		private bool _freezed = false;

		private HwndSource _hwndSrc;

		public FlashCardsWnd(IFlashCardsGenerator generator, int sleep)
		{
			Debug.Assert(generator != null);
			
			_generator = generator;

			InitializeComponent();

			listAnswers.MouseDoubleClick += new MouseButtonEventHandler(listAnswers_MouseDoubleClick);

			_timerSleep = new DispatcherTimer();
			_timerSleep.IsEnabled = false;
			_timerSleep.Interval = new TimeSpan(0, sleep, 0);
			_timerSleep.Tick += new EventHandler(OnTimerSleepTick);

			_timerSafeFreeze = new DispatcherTimer();
			_timerSafeFreeze.IsEnabled = false;
			_timerSafeFreeze.Interval = new TimeSpan(0, 0, 0, 0, 500);
			_timerSafeFreeze.Tick += new EventHandler(OnTimerSafeFreezeTick);

			_notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.Icon = LinguaSpace.Words.Properties.Resources.LinguaSpace;
			_notifyIcon.Click += new EventHandler(OnNotifyIconClick);

            Closed += new EventHandler(OnClosed);
            Loaded += new RoutedEventHandler(OnLoaded);
            IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);
		}

		private void listAnswers_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
		{
			FlashCardsCommands.Answer.Execute(null, (IInputElement)sender);
		}

		private void OnNotifyIconClick(Object sender, EventArgs e)
		{
			_timerSleep.Stop();
			Visibility = Visibility.Visible;
			Activate();
			FlashCardsCommands.Next.Execute(null, this);
		}

		private void OnIsVisibleChanged(Object sender, DependencyPropertyChangedEventArgs e)
		{
			_notifyIcon.Visible = (Visibility == Visibility.Hidden);
		}

		private void OnClosed(Object sender, EventArgs e)
		{
			_generator.Dispose();
			_notifyIcon.Dispose();
			Win32.UnregisterHotKey(_hwndSrc.Handle, HOT_KEY_ID);
		}

		private void OnLoaded(Object sender, EventArgs e)
		{
			WindowInteropHelper wndHelper = new WindowInteropHelper(this);
			_hwndSrc = HwndSource.FromHwnd(wndHelper.Handle);
			_hwndSrc.AddHook(new HwndSourceHook(WndProc));
			Win32.RegisterHotKey(_hwndSrc.Handle, HOT_KEY_ID, ModifierKeys.Control | ModifierKeys.Alt, 76);
            WPFUtils.ExecuteAsync(FlashCardsCommands.Next, null, this);
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (Win32.WM_HOTKEY == msg && (int)wParam == HOT_KEY_ID)
			{
				if (_timerSleep.IsEnabled)
				{
					OnTimerSleepTick(this, new EventArgs());
				}
				handled = true;
			}
			return IntPtr.Zero;
		}

		private void OnTimerSafeFreezeTick(Object sender, EventArgs e)
		{
			_timerSafeFreeze.Stop();
			_freezed = false;
			CommandManager.InvalidateRequerySuggested();
		}

		private void OnTimerSleepTick(Object sender, EventArgs e)
		{
			_timerSleep.Stop();
			Visibility = Visibility.Visible;
			Activate();
			FlashCardsCommands.Next.Execute(null, this);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnCanNext(Object sender, CanExecuteRoutedEventArgs e)
        {
			e.CanExecute = (!_freezed && (_card == null || _card.Question.Status != FlashCardItemStatus.Unknown));
        }

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnNext(Object sender, ExecutedRoutedEventArgs e)
		{
            _card = _generator.Generate();
            if (_card != null)
            {
                _freezed = true;
                _timerSafeFreeze.Start();
                DataContext = new FlashCardModel(_card);
                WPFUtils.SelectListBoxItemAsync(listAnswers, 0);
            }
            else
            {
                Closed += new EventHandler(OnClosedWhenNoCards);
                Close();
            }
		}

        private void OnClosedWhenNoCards(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Vocabulary does not have enough words to generate flash cards.\nPlease add at least a dozen of words of the same type before running flash cards exam.",
                Title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanAnswer(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (!_freezed && _card != null && _card.Question.Status == FlashCardItemStatus.Unknown);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnAnswer(Object sender, ExecutedRoutedEventArgs e)
		{
			FlashCardItemModel answer = (FlashCardItemModel)listAnswers.SelectedItem;
			Debug.Assert(answer != null);
			_card.Answer(answer.Data);
			WPFUtils.SelectListBoxItemAsync(listAnswers, new FlashCardItemModel(_card.Question));
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanPrompt(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (!_freezed && _card != null && _card.Question.Status == FlashCardItemStatus.Unknown);
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnPrompt(Object sender, ExecutedRoutedEventArgs e)
		{
			_card.Prompt();
			WPFUtils.SelectListBoxItemAsync(listAnswers, new FlashCardItemModel(_card.Question));
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanSleep(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnSleep(Object sender, ExecutedRoutedEventArgs e)
		{
			Visibility = Visibility.Hidden;
			_timerSleep.Start();
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnCanStop(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnStop(Object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnMoveDown(Object sender, ExecutedRoutedEventArgs e)
        {
            if (listAnswers.SelectedIndex == listAnswers.Items.Count - 1)
            {
                WPFUtils.SelectListBoxItemAsync(listAnswers, 0);
                e.Handled = true;
            }
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnMoveUp(Object sender, ExecutedRoutedEventArgs e)
        {
            if (listAnswers.SelectedIndex == 0)
            {
                WPFUtils.SelectListBoxItemAsync(listAnswers, listAnswers.Items.Count - 1);
                e.Handled = true;
            }
        }
	}
}