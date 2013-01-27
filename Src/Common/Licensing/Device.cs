using System;
using System.Collections;
using System.IO;
using System.Text;

namespace LinguaSpace.Common.Licensing
{
	public class Device
	{
		protected String name;
		protected IDictionary properties = new SortedList();

		public Device()
		{
		}

		public String Name
		{
			get
			{
				return name;
			}

			set
			{
				name = value;
			}
		}

		public IDictionary Properties
		{
			get
			{
				return properties;
			}
		}

		internal void Read(BinaryReader br)
		{
			name = br.ReadString();
			int count = br.ReadInt32();
			for (int i = 0; i < count; ++i)
			{
				String key = br.ReadString();
				String value = br.ReadString();
				properties.Add(key, value);
			}
		}

		internal void Write(BinaryWriter bw)
		{
			bw.Write(name);
			Int32 count = properties.Count;
			bw.Write(count);
			foreach (DictionaryEntry de in properties)
			{
				String key = (String)de.Key;
				bw.Write(key);
				String value = (String)de.Value;
				bw.Write(value);
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(this.name);
			sb.Append(" {");
			foreach (DictionaryEntry de in this.properties)
			{
				sb.Append(" ");
				sb.Append(de.Key.ToString());
				sb.Append("=");
				sb.Append(de.Value.ToString());
			}
			sb.Append(" }");
			return sb.ToString();
		}
	}
}
