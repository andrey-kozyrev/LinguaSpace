using System;
using System.IO;
using LinguaSpace.Common.Text;


namespace LinguaSpace.Common.IO
{
	public class XBinaryReader : BinaryReader
	{
		public XBinaryReader(Stream stream)
			: base(stream)
		{
		}

		public override String ReadString()
		{
			return StringRes.Mask(base.ReadString());
		}
	}
}
