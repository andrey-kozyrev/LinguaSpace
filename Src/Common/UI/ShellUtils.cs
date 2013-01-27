using System;
using System.Runtime.InteropServices;

namespace LinguaSpace.Common.UI
{
	public class ShellUtils 
    {
		#region "API Declaration"

        private enum FO_Func : uint
        {
            FO_MOVE = 0x0001,
            FO_COPY = 0x0002,
            FO_DELETE = 0x0003,
            FO_RENAME = 0x0004,
            FOF_ALLOWUNDO = 0x0040
        }

        private struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            public FO_Func wFunc;

            [MarshalAs(UnmanagedType.LPWStr)]
            public String pFrom;

            [MarshalAs(UnmanagedType.LPWStr)]
            public String pTo;

            public ushort fFlags;

            public bool fAnyOperationsAborted;

            public IntPtr hNameMappings;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszProgressTitle;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static extern int SHFileOperation(/*[In]*/ ref SHFILEOPSTRUCT lpFileOp);

		#endregion

        public static bool CopyFiles(String from, String to)
        {
			SHFILEOPSTRUCT op = new SHFILEOPSTRUCT();
			op.wFunc = FO_Func.FO_COPY;
			op.fFlags = (ushort)FO_Func.FOF_ALLOWUNDO;
			op.pFrom = from;
			op.pTo = to;
			return (SHFileOperation(ref op) == 0 && !op.fAnyOperationsAborted);
        }
	}
}

