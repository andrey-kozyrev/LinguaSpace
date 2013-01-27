using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using LinguaSpace.Common.ComponentModel;

namespace LinguaSpace.Common.UI
{
	public class WindowClosingGuard
	{
		private IValidationStatusProvider _validator;

		public WindowClosingGuard(Window window, IValidationStatusProvider validator)
		{
			Debug.Assert(window != null);
			Debug.Assert(validator != null);
			_validator = validator;
			window.Closing += new CancelEventHandler(window_Closing);
		}

		private void window_Closing(Object sender, CancelEventArgs e)
		{
			if (_validator.IsModified)
			{
				Window window = (Window)sender;

				if (! window.DialogResult ?? false)
				{
					if (MessageBox.Show(window, "Discard changes?", window.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
					{
						e.Cancel = true;
					}
				}
			}
		}
	}
}
