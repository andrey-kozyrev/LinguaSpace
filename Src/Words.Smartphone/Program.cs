using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using LinguaSpace.Words.UI;

namespace LinguaSpace.Words
{
	static class Program
	{
		[MTAThread]
		static void Main()
		{
			Application.Run(new MainForm());
		}
	}
}