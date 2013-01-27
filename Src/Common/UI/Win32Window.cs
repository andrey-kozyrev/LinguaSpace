using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LinguaSpace.Common.UI
{
	public class Win32Window : IWin32Window
	{
		private IntPtr _handle;

		public Win32Window(IntPtr handle)
		{
			Debug.Assert(handle != IntPtr.Zero);
			_handle = handle;
		}

		public IntPtr Handle 
		{
			get
			{
				return _handle;
			}
		}
	}
}
