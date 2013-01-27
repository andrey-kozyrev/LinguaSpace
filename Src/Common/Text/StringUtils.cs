using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using LinguaSpace.Common.Resources;

namespace LinguaSpace.Common.Text
{
	public class StringUtils
	{
		private StringUtils()
		{
		}

		static public string Trim(string str)
		{
			if (str != null)
			{
				return str.Trim();
			}
			else
			{
				return String.Empty;
			}
		}

		static public bool Contains(string[] arr, string str)
		{
			Debug.Assert(str != null);
			Debug.Assert(arr != null);
			int count = arr.Length;
			for (int i = 0; i < count; ++i)
			{
				if (arr[i] == str)
				{
					return true;
				}
			}
			return false;
		}

		static public bool IsEmpty(String str)
		{
			return (str == null || str.Length == 0);
		}

		static public String WrapNull(String str)
		{
			return str != null ? str : String.Empty;
		}

		public static int FindToken(String data, String token, int pos)
		{
			pos = data.IndexOf(token, pos);
			if (pos < 0)
			{
				pos = data.Length;
			}
			return pos;
		}

		public static String Bytes2Str(byte[] bytes)
		{
			StringBuilder sb = new StringBuilder(bytes.Length * 2);
			for (int i = 0; i < bytes.Length; ++i)
			{
				sb.Append(bytes[i].ToString("X2"));
			}
			return sb.ToString();
		}

		public static byte[] Str2Bytes(String value)
		{
			Debug.Assert(value.Length % 2 == 0);
			int length = value.Length / 2;
			byte[] bytes = new byte[length];
			for (int i = 0; i < length; ++i)
			{
				String s = value.Substring(2*i, 2);
				byte b = Byte.Parse(s, System.Globalization.NumberStyles.HexNumber);
				bytes[i] = b;
			}
			return bytes;
		}

		private static String FormatByteStringHelper(byte[] bytes)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < bytes.Length; ++i)
			{
				if (i > 0)
				{
					sb.Append(", ");
				}
				sb.Append("0x");
				sb.Append(bytes[i].ToString("X2"));
			}
			return sb.ToString();
		}

        public static void StringToList(String text, List<String> list)
        {
            text.Trim();
            String[] items = text.Split(null);
            list.Clear();
            foreach (String item in items)
            {
                if (!StringUtils.IsEmpty(item))
                {
                    list.Add(item);
                }
            }
        }

        public static String ListToString(List<String> list)
        {
            String text = String.Empty;
            if (list.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (String item in list)
                {
                    sb.Append(item);
                    sb.Append(" ");
                }
                text = sb.ToString(0, sb.Length - 1);
            }
            return text;
        }

		public static void AppendToken(StringBuilder builder, String text)
		{
			if (text.Length > 0)
			{
				if (builder.Length > 0)
					builder.Append(" ");
				builder.Append(text);
			}
		}

		public static String ListToString(params Object[] parameters)
		{
			StringBuilder builder = new StringBuilder();
			foreach (Object obj in parameters)
			{
				AppendToken(builder, obj.ToString());
			}
			return builder.ToString();
		}
	}
}
