using System;
using System.IO;
using System.Management;
using System.Diagnostics;
using System.Collections;
using LinguaSpace.Common.Resources;

namespace LinguaSpace.Common.Licensing
{
	public abstract class WMIFingerprintBase
	{
		protected ArrayList devices = new ArrayList();

		public WMIFingerprintBase()
		{
		}

		// public i-face
		public void Clear()
		{
			devices.Clear();
		}

		/// <summary>
		/// scan the appropriate devices
		/// </summary>
		public abstract void Scan();

		// returns number of devices in this hardware which are absent in the other hardware
		public uint Compare(WMIFingerprintBase hardware)
		{
			return CompareDevices(devices, hardware.devices) + CompareDevices(hardware.devices, devices);
		}

		// scanning =======================================================
		protected void ScanDevices(ArrayList devices, String name, String[] properties)
		{
			String className = CommonStrings.WIN32_ + name;
			ManagementClass mc = new ManagementClass(className);
			ManagementObjectCollection objects = mc.GetInstances();
			foreach (ManagementObject mo in objects)
			{
				Device device = new Device();
				device.Name = name;
				ScanDevice(device, mo, properties);
				devices.Add(device);
			}
		}

		protected void ScanDevice(Device device, ManagementObject mo, String[] properties)
		{
			for (int i = 0; i < properties.Length; ++i)
			{
				String name = properties[i];
				String value = String.Empty;
				Object obj = mo[name];
				if (obj != null)
				{
					value = obj.ToString();
				}
				device.Properties[name] = value;
			}
		}


		// comparing ==========================================================
		protected uint CompareDevices(ArrayList devsA, ArrayList devsB)
		{
			uint diff = 0;

			ArrayList devicesA = (ArrayList)devsA.Clone();
			ArrayList devicesB = (ArrayList)devsB.Clone();

			foreach (Device deviceA in devicesA)
			{
				Device match = FindMatchingDevice(devicesB, deviceA);
				if (match != null)
				{
					devicesB.Remove(match);
				}
				else
				{
					++diff;
				}
			}
			return diff;
		}

		protected Device FindMatchingDevice(ArrayList devices, Device device)
		{
			Device same = null;
			foreach (Device d in devices)
			{
				if ( IsDeviceEqual(device, d) )
				{
					same = d;
					break;
				}
			}
			return same;
		}

		protected bool IsDeviceEqual(Device d1, Device d2)
		{
			bool result = false;
			if (d1.Name == d2.Name)
			{
				result = true;
				foreach (DictionaryEntry de in d1.Properties)
				{
					Object name = de.Key;
					Object value = de.Value;
					if (!value.Equals(d2.Properties[name]))
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}


		// stream read/write ======================================================
		internal void Read(BinaryReader br)
		{
			devices.Clear();
			Int32 count = br.ReadInt32();
			for (Int32 i = 0; i < count; ++i)
			{
				Device device = new Device();
				device.Read(br);
				devices.Add(device);
			}
		}

		internal void Write(BinaryWriter bw)
		{
			Int32 count = devices.Count;
			bw.Write(count);
			foreach (Device device in devices)
			{
				device.Write(bw);
			}
		}

		public void dump()
		{
			IEnumerator ed = devices.GetEnumerator();
			foreach (Device d in devices)
			{
				System.Console.Out.WriteLine(d.Name);
				foreach (DictionaryEntry p in d.Properties)
				{
					System.Console.Out.WriteLine(p.Key + "=" + p.Value);
				}
			}	
		}
	}
}
