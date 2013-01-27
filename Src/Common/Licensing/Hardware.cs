using System;
using System.IO;
using System.Management;
using System.Diagnostics;
using System.Collections;
using LinguaSpace.Common.Resources;

namespace LinguaSpace.Common.Licensing
{
	/// <summary>
	/// a fingerprint of the hardware
	/// intended to be different on most computers and stable enough for the specific computer
	/// </summary>
	public class Hardware : WMIFingerprintBase
	{
		public Hardware()
		{
		}

		public override void Scan()
		{
			Clear();

			ScanDevices(devices, CommonStrings.MOTHERBOARDDEVICE, new String[] {
				CommonStrings.NAME,
				CommonStrings.PRIMARYBUSTYPE,
				CommonStrings.SECONDARYBUSTYPE
			});

			ScanDevices(devices, CommonStrings.PROCESSOR, new String[] {
				CommonStrings.MANUFACTURER,
				CommonStrings.ARCHITECTURE,
				CommonStrings.FAMILY,
				CommonStrings.LEVEL,
				CommonStrings.PROCESSORTYPE,
				CommonStrings.STEPPING,
				CommonStrings.PROCESSORID});

			/*
			ScanDevices(devices, StringRes.DESKTOPMONITOR, new String[] {
				StringRes.MONITORMANUFACTURER,
				StringRes.NAME});
			 */

			/*
			ScanDevices(devices, StringRes.CDROMDRIVE, new String[] {
				StringRes.MANUFACTURER,
				StringRes.NAME
			});
			 */

			ScanDevices(devices, CommonStrings.VIDEOCONTROLLER, new String[] {
				CommonStrings.ADAPTERRAM,
				CommonStrings.NAME});

			/*
			ScanDevices(devices, StringRes.BIOS, new String[] {
				StringRes.NAME,
				StringRes.MANUFACTURER,
				StringRes.SERIALNUMBER,
				StringRes.SMBIOSBIOSVERSION,
				StringRes.SMBIOSMAJORVERSION,
				StringRes.SMBIOSMINORVERSION,
				StringRes._VERSION});
			 */

			/*
			ScanDevices(devices, StringRes.OPERATINGSYSTEM, new String[] {
				StringRes.BUILDNUMBER,
				StringRes.BUILDTYPE,
				StringRes.COUNTRYCODE,
				StringRes.MANUFACTURER,
				StringRes.REGISTEREDUSER,
				StringRes.SERIALNUMBER,
				StringRes.CAPTION,
				StringRes._VERSION});
			 */
		}
	}
}
