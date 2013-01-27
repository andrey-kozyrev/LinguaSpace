using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LinguaSpace.Sync
{
	public enum SyncCode : ushort
	{
		Error,
		Ok,
		Cancel,
		Handshake,
		File,
		Guid
	}

	public class SyncReader : BinaryReader
	{
		public SyncReader(Stream input) 
			: base(input)
		{
		}

		public SyncCode ReadCode()
		{
			return (SyncCode)ReadUInt16();
		}
	}

	public class SyncWriter : BinaryWriter
	{
		public SyncWriter(Stream output)
			: base(output)
		{
		}

		public void Write(SyncCode code)
		{
			Write((ushort)code);
		}
	}
}
