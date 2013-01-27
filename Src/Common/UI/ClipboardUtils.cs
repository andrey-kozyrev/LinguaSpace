using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace LinguaSpace.Common.UI
{
	/*
	public static class ClipboardUtils
	{
		public static void Clear()
		{
			Clipboard.Clear();
		}

		public static bool Contains<TType>()
		{
			bool contains = false;
			IDataObject data = Clipboard.GetDataObject();
			if (data != null)
			{
				contains = data.GetDataPresent(typeof(TType));
			}
			return contains;
		}

		public static TType Get<TType>(TType obj) where TType : class
		{
			Debug.Assert(obj != null);
			IDataObject data = Clipboard.GetDataObject();
			if (data != null)
			{
				byte[] bytes = (byte[])data.GetData(typeof(TType));
				Debug.Assert(bytes != null);
				using (MemoryStream stream = new MemoryStream(bytes))
				{
					using (BinaryReader reader = new BinaryReader(stream))
					{
						IBinarySerializable bs = (IBinarySerializable)obj;
						bs.PrepareLoad();
						bs.Load(reader);
						bs.FinalizeLoad();
					}
				}
			}
			return obj;			
		}

		public static void Set<TType>(TType obj) where TType : class
		{
			Debug.Assert(obj != null);
			using (MemoryStream stream = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(stream))
				{
					IBinarySerializable bs = (IBinarySerializable)obj;
					bs.PrepareSave();
					bs.Save(writer);
					bs.FinalizeSave();
					byte[] bytes = stream.ToArray();
					DataObject data = new DataObject();
					data.SetData(typeof(TType), bytes);
					Clipboard.SetDataObject(data);
				}
			}
		}
	}
	 */
}
