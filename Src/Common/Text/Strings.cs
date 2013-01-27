using System;
using System.Diagnostics;
using System.Text;
using LinguaSpace.Common.Resources;

namespace LinguaSpace.Common.Text
{
	public class PrivateStringsRoot
	{
		protected PrivateStringsRoot()
		{
		}

		public static StringBox MakeStringBox(ushort[] data)
		{
			return new StringBox(data);
		}

		static public String Mask(String str)
		{
			Debug.Assert(str != null);
			StringBuilder sb = new StringBuilder(str.Length);
			for (ushort n = 0; n < str.Length; ++n)
			{
				sb.Append(MaskChar(str[n], n));
			}
			return sb.ToString();
		}

		static public ushort[] EncodeString(String str)
		{
			Debug.Assert(str != null);
			ushort[] data = new ushort[str.Length];
			for (ushort n = 0; n < str.Length; ++n)
			{
				data[n] = (ushort)MaskChar(str[n], n);
			}
			return data;
		}

		static protected char MaskChar(char c, ushort n)
		{
			ushort mask = (ushort)(MASK % (n + 0xFF));
			return (char)(c ^ mask);
		}

		private static ushort MASK = 0xF173;
	}

	public class PrivateStringsBase : PrivateStringsRoot
	{
		protected PrivateStringsBase()
		{
		}

		static public String EncodeArray(String str)
		{
			Debug.Assert(str != null);
			StringBuilder sb = new StringBuilder();
			sb.Append(BRACE_OPEN);
			for (ushort n = 0; n < str.Length; ++n)
			{
				if (n > 0)
					sb.Append(", ");
				ushort us = (ushort)MaskChar(str[n], n);
				sb.Append(String.Format(CommonStrings.FORMAT_HEX, us.ToString("X")));
			}
			sb.Append(BRACE_CLOSE);
			return sb.ToString();
		}

		static public String DecodeArray(ushort[] arr)
		{
			Debug.Assert(arr != null);
			StringBuilder sb = new StringBuilder();
			for (ushort n = 0; n < arr.Length; ++n)
			{
				sb.Append(MaskChar((char)arr[n], n));
			}
			return sb.ToString();
		}

		/// <summary>
		/// {
		/// </summary>
		public static StringBox BRACE_OPEN = MakeStringBox(new ushort[] { 0x1E });

		/// <summary>
		/// }
		/// </summary>
		public static StringBox BRACE_CLOSE = MakeStringBox(new ushort[] { 0x18 });

		/// <summary>
		/// ,
		/// </summary>
		public static StringBox COMMA = MakeStringBox(new ushort[] { 0x49 });

		/// <summary>
		/// " "
		/// </summary>
		public static StringBox SPACE = MakeStringBox(new ushort[] { 0x45 });

		/// <summary>
		/// [
		/// </summary>
		public static StringBox BRACKET_OPEN = MakeStringBox(new ushort[] { 0x3E });

		/// <summary>
		/// ]
		/// </summary>
		public static StringBox BRACKET_CLOSE = MakeStringBox(new ushort[] { 0x38 });

		/// <summary>
		/// -
		/// </summary>
		public static StringBox DASH = MakeStringBox(new ushort[] { 0x48 });

		/// <summary>
		/// =
		/// </summary>
		public static StringBox EQUAL = MakeStringBox(new ushort[] { 0x58 });

		/// <summary>
		///.
		/// </summary>
		public static StringBox DOT = MakeStringBox(new ushort[] { 0x4B });

		/// <summary>
		/// \n
		/// </summary>
		public static StringBox NEWLINE = MakeStringBox(new ushort[] { 0x6f });

		/// <summary>
		/// 0
		/// </summary>
		public static StringBox ZERO = MakeStringBox(new ushort[] { 0x55 });

		/// <summary>
		/// \
		/// </summary>
		public static StringBox SLASH = MakeStringBox(new ushort[] { 0x39 });

		/// <summary>
		/// 0x{0}
		/// </summary>
		public static StringBox FORMAT_HEX = MakeStringBox(new ushort[] { 0x55, 0xB, 0xF8, 0xA5, 0xD4 });

		/// <summary>
		/// X
		/// </summary>
		public static StringBox X = MakeStringBox(new ushort[] { 0x3D });
		
	}

	public class StringRes : PrivateStringsBase
	{
		protected StringRes()
		{
		}

		/// <summary>
		/// name
		/// </summary>
		public static StringBox NAME = MakeStringBox(new ushort[] {0xB, 0x12, 0xEE, 0xF0});


		/// <summary>
		///LinguaSpace
		/// </summary>
		public static StringBox PRODUCT = MakeStringBox(new ushort[] {0x29, 0x1A, 0xED, 0xF2, 0xDC, 0xDE, 0x84, 0x81, 0x67, 0x40, 0x27});		

        /// <summary>
        ///2
        /// </summary>
        public static StringBox MAJOR_VERSION = MakeStringBox(new ushort[] { 0x57 });

		/// <summary>
		///1
		/// </summary>
		public static StringBox MINOR_VERSION = MakeStringBox(new ushort[] {0x54});

		/// <summary>
		/// 0
		/// </summary>
		public static StringBox PATCH_VERSION = MakeStringBox(new ushort[] {0x55});

		/// <summary>
		/// 2.1.0
		/// </summary>
		public static StringBox VERSION_FULL = MAJOR_VERSION + DOT + MINOR_VERSION + DOT + PATCH_VERSION;

        /// <summary>
        /// 2.0
        /// </summary>
        public static StringBox VERSION_SHORT = MAJOR_VERSION + DOT + MINOR_VERSION;

        public static StringBox PRODUCT_VERSION_FULL = PRODUCT + SPACE + VERSION_FULL;

        public static StringBox PRODUCT_VERSION_SHORT = PRODUCT + SPACE + VERSION_SHORT;


		/// <summary>
		/// Help
		/// </summary>
		public static StringBox HELP = MakeStringBox(new ushort[] {0x2D, 0x16, 0xEF, 0xE5});

		/// <summary>
		///help-quickstart.html
		/// </summary>
		public static StringBox HELP_QUICKSTART_HTML = MakeStringBox(new ushort[] {0xD, 0x16, 0xEF, 0xE5, 0x84, 0xCE, 0xA2, 0x98, 0x65, 0x48, 0x31, 0x17, 0xE7, 0xD9, 0xA6, 0xD5, 0x7F, 0x37, 0x1C, 0xCD});

        /// <summary>
        ///Key.dat
        /// </summary>
        public static StringBox KEY_DAT = MakeStringBox(new ushort[] {0x2E, 0x16, 0xFA, 0xBB, 0xCD, 0xDE, 0xA3});

		/// <summary>
		/// dat
		/// </summary>
		public static StringBox DAT = MakeStringBox(new ushort[] {0x1, 0x12, 0xF7});

		/// <summary>
		///sales@linguaspace.com
		/// </summary>
		public static StringBox EMAIL = MakeStringBox(new ushort[] {0x16, 0x12, 0xEF, 0xF0, 0xDA, 0xFF, 0xBB, 0x98, 0x68, 0x44, 0x37, 0x2, 0xF5, 0xDB, 0xB3, 0x98, 0x72, 0x6D, 0x12, 0xCE, 0xBE});

		// X2
		public static StringBox X2 = MakeStringBox(new ushort[] {0x3D, 0x41});

		/// <summary>
		///X8
		/// </summary>
		public static StringBox X8 = MakeStringBox(new ushort[] {0x3D, 0x4B});

		// 0x
		public static StringBox OX = MakeStringBox(new ushort[] {0x55, 0xB});

		/// <summary>
		///LinguaSpace.ini 
		/// </summary>
		public static StringBox LINGUASPACE_INI  = MakeStringBox(new ushort[] {0x29, 0x1A, 0xED, 0xF2, 0xDC, 0xDE, 0x84, 0x81, 0x67, 0x40, 0x27, 0x4D, 0xEF, 0xC5, 0xBB, 0xDB});

		/// <summary>
		///lsp
		/// </summary>
		public static StringBox LSP = MakeStringBox(new ushort[] { 0x9, 0x0, 0xF3 });

		/// <summary>
		///*.lsp
		/// </summary>
		public static StringBox LSP_MASK = MakeStringBox(new ushort[] {0x4F, 0x5D, 0xEF, 0xE6, 0xD9});

		/// <summary>
		///lsv
		/// </summary>
		public static StringBox LSV = MakeStringBox(new ushort[] { 0x9, 0x0, 0xF5 });

        /// <summary>
        ///*.lsv
        /// </summary>
        public static StringBox LSV_MASK = MakeStringBox(new ushort[] {0x4F, 0x5D, 0xEF, 0xE6, 0xDF});

        /// <summary>
        ///.lsp
        /// </summary>
        public static StringBox DOT_LSP = DOT + LSP;
        
        /// <summary>
        ///.lsv
        /// </summary>
        public static StringBox DOT_LSV = DOT + LSV;

		/// <summary>
		///Profiles
		/// </summary>
		public static StringBox PROFILES = MakeStringBox(new ushort[] { 0x35, 0x1, 0xEC, 0xF3, 0xC0, 0xD3, 0xB2, 0x82 });

		/// <summary>
		///Vocabularies
		/// </summary>
		public static StringBox VOCABULARIES = MakeStringBox(new ushort[] { 0x33, 0x1C, 0xE0, 0xF4, 0xCB, 0xCA, 0xBB, 0x90, 0x74, 0x4A, 0x27, 0x10 });

        /// <summary>
        ///License
        /// </summary>
        public static StringBox LICENSE = MakeStringBox(new ushort[] { 0x29, 0x1A, 0xE0, 0xF0, 0xC7, 0xCC, 0xB2 });

		/// <summary>
		///IsValid
		/// </summary>
		public static StringBox ISVALID = MakeStringBox(new ushort[] { 0x2C, 0x0, 0xD5, 0xF4, 0xC5, 0xD6, 0xB3 });

		/// <summary>
		///ValidationMessage
		/// </summary>
		public static StringBox VALIDATIONMESSAGE = MakeStringBox(new ushort[] { 0x33, 0x12, 0xEF, 0xFC, 0xCD, 0xDE, 0xA3, 0x98, 0x69, 0x4D, 0xF, 0x6, 0xF5, 0xD8, 0xB3, 0x9C, 0x72 });

		/// <summary>
		///ValidationMessageType
		/// </summary>
		public static StringBox VALIDATIONMESSAGETYPE = MakeStringBox(new ushort[] { 0x33, 0x12, 0xEF, 0xFC, 0xCD, 0xDE, 0xA3, 0x98, 0x69, 0x4D, 0xF, 0x6, 0xF5, 0xD8, 0xB3, 0x9C, 0x72, 0x17, 0x8, 0xD1, 0xB6 });

		/// <summary>
		///Data is valid
		/// </summary>
		public static StringBox DATA_IS_VALID = MakeStringBox(new ushort[] {0x21, 0x12, 0xF7, 0xF4, 0x89, 0xD6, 0xA4, 0xD1, 0x70, 0x42, 0x2E, 0xA, 0xE2});
	}
}

