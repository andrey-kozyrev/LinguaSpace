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
using LinguaSpace.Common.UI;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Grammar.ComponentModel;

namespace LinguaSpace.Grammar.UI
{
	public partial class GrammarWnd : Window
	{
		public GrammarWnd(GrammarEditModel model, Validator validator)
		{
			InitializeComponent();
			this.status.DataContext = validator;
			this.DataContext = model;
			new WindowClosingGuard(this, validator);
		}
	}
}
