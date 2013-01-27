using System;
using LinguaSpace.Common.Resources;

namespace LinguaSpace.Common.Licensing
{
	public class OS : WMIFingerprintBase
	{
		public OS()
		{
		}

		public override void Scan()
		{
			Clear();
			ScanDevices(devices, CommonStrings.OPERATINGSYSTEM, new String[] {
				CommonStrings.CAPTION,
				CommonStrings._VERSION,
				CommonStrings.BUILDTYPE,
				CommonStrings.INSTALLDATE,
				CommonStrings.SERIALNUMBER,
				CommonStrings.WINDOWSDIRECTORY
			});
		}
	}
}
