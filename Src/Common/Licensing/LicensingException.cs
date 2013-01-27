using System;

namespace LinguaSpace.Common.Licensing
{
	public class LicensingException : ApplicationException
	{
		public LicensingException(String msg)
			: base(msg)
		{
		}
	}
}
