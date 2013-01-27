using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace LinguaSpace.Common.UI
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct WINDOWPLACEMENT
	{
		public int length;
		public int flags;
		public int showCmd;
		public POINT minPosition;
		public POINT maxPosition;
		public RECT normalPosition;
	}

	public static class Win32
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		extern public static bool DestroyIcon(IntPtr handle);

		public const uint WM_HOTKEY = 0x312;
		public const uint WM_SETHOTKEY = 0x0032;
		public const uint WM_GETHOTKEY = 0x33;

		[DllImport("User32")]
		public extern static int SendMessage(IntPtr hwnd, uint msg, uint wParam, uint lParam);

		[DllImport("User32")]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeys fsModifiers, int uVirtKey);
		[DllImport("User32")]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		[DllImport("user32.dll")]
		public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

		[DllImport("user32.dll")]
		public static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);
	}
}
