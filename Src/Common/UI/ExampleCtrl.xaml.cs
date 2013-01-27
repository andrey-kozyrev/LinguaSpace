using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LinguaSpace.Common.UI
{
    public partial class ExampleCtrl : System.Windows.Controls.UserControl
	{
		public static readonly DependencyProperty TextProperty;

		static ExampleCtrl()
		{
			FrameworkPropertyMetadata textMetadata = new FrameworkPropertyMetadata(String.Empty,
				FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				new PropertyChangedCallback(OnTextChanged),
				new CoerceValueCallback(OnCoerceText),
				true);

			ExampleCtrl.TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(ExampleCtrl), textMetadata);
		}

		private static Object OnCoerceText(DependencyObject d, Object baseValue)
		{
			return baseValue;
		}

		private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ExampleCtrl exampleCtrl = (ExampleCtrl)d;
			exampleCtrl.OnTextChanged(e);
		}

		private bool internalTextChanging = false;

		public ExampleCtrl()
		{
			InitializeComponent();
			this.richTextBox.TextChanged += new TextChangedEventHandler(OnRichTextBoxTextChanged);
			this.Loaded += new RoutedEventHandler(ExampleCtrl_Loaded);
			this.GotFocus += new RoutedEventHandler(ExampleCtrl_GotFocus);
		}

		private void ExampleCtrl_GotFocus(object sender, RoutedEventArgs e)
		{
			Dispatcher.CurrentDispatcher.BeginInvoke((Func<bool>)this.richTextBox.Focus, DispatcherPriority.Background);
		}

		void ExampleCtrl_Loaded(Object sender, RoutedEventArgs e)
		{
			Style styleBase = (Style)this.FindResource(typeof(RichTextBox));
			Style styleExample = (Style)this.richTextBox.FindResource("Example");
			styleExample.BasedOn = styleBase;
			this.richTextBox.Style = styleExample;
		}

		public String Text
		{
			get
			{
				return (String)GetValue(TextProperty);
			}
			set
			{
				SetValue(TextProperty, value);
			}
		}
		
		public event DependencyPropertyChangedEventHandler TextChanged;

		protected virtual void OnTextChanged(DependencyPropertyChangedEventArgs e)
		{
			if (!this.internalTextChanging)
			{
				String text = (String)e.NewValue;
				FlowDocument doc = FlowDocumentUtils.Text2FlowDocument(text, true, SystemColors.WindowTextBrush);
				SetFlowDocumentAsync(doc);
				RaiseTextChanged(e);
			}
		}

		private void RaiseTextChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.TextChanged != null)
			{
				this.TextChanged(this, e);
			}
		}

		private delegate void SetFlowDocumentDelegate(FlowDocument doc);

		private void SetFlowDocument(FlowDocument doc)
		{
			this.internalTextChanging = true;
			this.richTextBox.TextChanged -= new TextChangedEventHandler(OnRichTextBoxTextChanged);

			try
			{
				TextRange rangeBeforeCaret1 = new TextRange(this.richTextBox.Document.ContentStart, this.richTextBox.CaretPosition);
				String textBeforeCaret1 = rangeBeforeCaret1.Text;

				this.richTextBox.Document = doc;

				using (this.richTextBox.DeclareChangeBlock())
				{
					TextPointer caretPosition2 = this.richTextBox.Document.ContentStart;
					TextRange rangeBeforeCaret2 = new TextRange(this.richTextBox.Document.ContentStart, caretPosition2);
					String textBeforeCaret2 = rangeBeforeCaret2.Text;

					while (textBeforeCaret1 != textBeforeCaret2)
					{
						caretPosition2 = caretPosition2.GetNextInsertionPosition(LogicalDirection.Forward);
						if (caretPosition2 == null)
						{
							caretPosition2 = this.richTextBox.Document.ContentEnd;
							break;
						}
						rangeBeforeCaret2 = new TextRange(this.richTextBox.Document.ContentStart, caretPosition2);
						textBeforeCaret2 = rangeBeforeCaret2.Text;
					}
					this.richTextBox.CaretPosition = caretPosition2;
				}
			}
			finally
			{
				this.internalTextChanging = false;
				this.richTextBox.TextChanged += new TextChangedEventHandler(OnRichTextBoxTextChanged);
			}
		}

		private void SetFlowDocumentAsync(FlowDocument doc)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Send, new SetFlowDocumentDelegate(SetFlowDocument), doc);
		}

        private void OnRichTextBoxTextChanged(Object sender, TextChangedEventArgs e)
		{
			String text = FlowDocumentUtils.FlowDocument2Text(this.richTextBox.Document);
            String sign1 = FlowDocumentUtils.Text2BracketsSignature(text);
            String sign2 = FlowDocumentUtils.Text2BracketsSignature(this.Text);
            bool bSame = (sign1 == sign2);
            
            bool internalTextChangingOld = this.internalTextChanging;
            
            if (bSame)
                this.internalTextChanging = true;
            
			this.Text = text;

            this.internalTextChanging = internalTextChangingOld;
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        private void OnCanHighlight(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !this.richTextBox.Selection.IsEmpty;
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		private void OnHighlight(Object sender, ExecutedRoutedEventArgs e)
		{
			this.richTextBox.Selection.Text = "[" + this.richTextBox.Selection.Text + "]";
		}
	}
}