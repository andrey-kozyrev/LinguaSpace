using System;
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
using System.Windows.Threading;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Grammar.ComponentModel;

namespace LinguaSpace.Grammar.UI
{
	public partial class TopicWnd : Window
	{
		public TopicWnd(TopicModel model, Validator validator)
		{
			InitializeComponent();
			this.status.DataContext = validator;
			this.DataContext = model;
			this.Loaded += new RoutedEventHandler(TopicWnd_Loaded);
			InputLanguageUtils.SetTargetInputLangage();
			Dispatcher.CurrentDispatcher.BeginInvoke( (Func<bool>)(delegate() { this.title.Select(0, model.Title.Length); return false; }), DispatcherPriority.Background);
		}

		private void TopicWnd_Loaded(object sender, RoutedEventArgs e)
		{
			InputLanguageUtils.SetNativeInputLanguage();
		}
	}
}
