using System;
using System.Diagnostics;

namespace LinguaSpace.Common.Text
{
	public class StringBox
	{
		private ushort[] data = null;

		public StringBox(ushort[] data)
		{
			Debug.Assert(data != null);
			this.data = data;
		}

        //[System.Reflection.Obfuscation(Exclude = true)]
        public String Text
        {
            get
            {
                return this.ToString();
            }
        }

		public override String ToString()
		{
			return StringRes.DecodeArray(this.data);
		}

		public static bool operator == (StringBox box1, StringBox box2)
		{
			bool equal = false;
			if ((Object)box1 != null && (Object)box2 != null)
			{
				if (box1.data == box2.data)
				{
					equal = true;
				}
				else if (box1.data.Length == box2.data.Length)
				{
					equal = true;
					for (int i = 0; i < box1.data.Length; ++i)
					{
						if (box1.data[i] != box2.data[i])
						{
							equal = false;
							break;
						}
					}
				}
			}
			return equal;
		}

		public static bool operator != (StringBox box1, StringBox box2)
		{
			return ! (box1 == box2);
		}

		public static bool operator == (StringBox box, String str)
		{
			bool equal = false;
			if ((Object)box != null && (Object)str != null)
			{
				if (box.data.Length == str.Length)
				{
					equal = (box.ToString() == str);
				}
			}
			return equal;
		}

		public static bool operator != (StringBox box, String str)
		{
			return !(box == str);
		}

		public static bool operator == (String str, StringBox box)
		{
			return (box == str);
		}

		public static bool operator != (String str, StringBox box)
		{
			return !(str == box);
		}

		public override bool Equals(Object obj)
		{
			bool equal = false;
			if (obj is StringBox)
			{
				StringBox box = (StringBox)obj;
				equal = (this == box);
			}
			else if (obj is String)
			{
				String str = (String)obj;
				equal = (this == str);
			}
			else
			{
				equal = base.Equals(obj);
			}
			return equal;
		}

		public override int GetHashCode()
		{
			return this.data.GetHashCode();
		}

		public static implicit operator String(StringBox box)
		{
			return box.ToString();
		}

		public static StringBox operator + (StringBox sb1, StringBox sb2)
		{
			String str1 = sb1.ToString();
			String str2 = sb2.ToString();
			String str = str1 + str2;
			ushort[] data = StringRes.EncodeString(str);
			return new StringBox(data);
		}

		public static String operator + (StringBox box, String str)
		{
			return box.ToString() + str;
		}

		public static String operator + (String str, StringBox box)
		{
			return str + box.ToString();
		}
	}
}
