using System;
using System.IO;
using LinguaSpace.Common.Text;

namespace LinguaSpace.Common.IO
{
	public class XBinaryWriter : BinaryWriter
	{
		public XBinaryWriter(Stream stream)
			: base(stream)
		{
		}

		public override void Write(String value)
		{
			base.Write(StringRes.Mask(value));
		}
	}
}
