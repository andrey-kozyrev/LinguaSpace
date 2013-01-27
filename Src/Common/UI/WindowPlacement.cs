using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace LinguaSpace.Common.UI
{
	[Serializable]
	public class WindowPlacement
	{
		public WINDOWPLACEMENT _data;

		public WindowPlacement()
		{
		}

		public WindowPlacement(Window window)
		{
			Debug.Assert(window != null);
			Win32.GetWindowPlacement(new WindowInteropHelper(window).Handle, out _data);
		}
	}

	public static class WindowPlacementExtension
	{
		public static void Apply(this WindowPlacement placement, Window window)
		{
			if (placement != null)
			{
				Win32.SetWindowPlacement(new WindowInteropHelper(window).Handle, ref placement._data);
			}
		}
	}
}
