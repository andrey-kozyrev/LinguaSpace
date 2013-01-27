using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LinguaSpace.Words.UI
{
	public partial class ProgressForm : Form
	{
		public ProgressForm()
		{
			InitializeComponent();
		}

		private void ProgressForm_FormClosing(Object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
		}

		public new void Close()
		{
			FormClosing -= new FormClosingEventHandler(ProgressForm_FormClosing);
			base.Close();
		}
	}
}
