using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Win32;
using LinguaSpace.Common.Text;
using LinguaSpace.Common.IO;
using LinguaSpace.Common.Resources;

namespace LinguaSpace.Common.Licensing
{
	public class TrialCounter : ITrial
	{
		static private int TRIAL_DAYS = 30;

		private OS os = null;
        private int days = 123;

		public TrialCounter()
		{
			this.os = new OS();
			this.os.Scan();
			
			String path = GetPath();

            /*
			using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(path))
			{
                Debug.Assert(key != null);

                this.days = 0;

                DateTime now = DateTime.Now;
                DateTime start = now;
                DateTime last = now;
                Read(path, ref start, ref last);

                if (last < start)
                {
                    last = start;
                }

                if (last <= now)
                {
                    this.days = (TRIAL_DAYS - (now - start).Days);
                    if (this.days < 0)
                    {
                        days = 0;
                    }
                    last = now;
                }

                Write(path, start, last);
			}
             */
		}

		#region ITrialCounter Members

		public int Days
		{
			get
			{
				return days;
			}	
		}

		#endregion

		protected String GetPath()
		{
			Debug.Assert(os != null);
			byte[] data = null;
			using (MemoryStream ms = new MemoryStream())
			{
				using (BinaryWriter bw = new BinaryWriter(ms))
				{
					os.Write(bw);
					bw.Flush();
					ms.Flush();
					data = ms.ToArray();
				}
			}
			HashAlgorithm hasher = HashAlgorithm.Create(CommonStrings.SHA1);
			byte[] hash = hasher.ComputeHash(data);
			byte[] hash16 = new byte[16];
			for (int i = 0; i < 16; ++i)
			{
				hash16[i] = hash[i];
			}
			Guid guid = new Guid(hash16);
			String path = String.Format(CommonStrings.FORMAT_TRIAL_COUNTER_PATH, guid.ToString());
			return path;
		}

        protected void Read(String path, ref DateTime start, ref DateTime last)
        {
            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(path))
            {
                if (key != null)
                {
                    String sData = (String)key.GetValue(String.Empty);
                    if (sData != null)
                    {
                        byte[] data = StringUtils.Str2Bytes(sData);
                        using (MemoryStream s = new MemoryStream(data))
                        {
                            using (BinaryReader br = new XBinaryReader(s))
                            {
                                start = new DateTime(br.ReadInt64());
                                last = new DateTime(br.ReadInt64());
                            }
                        }
                    }
                }
            }
        }

        protected void Write(String path, DateTime start, DateTime last)
        {
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(path))
            {
                Debug.Assert(key != null);
                using (MemoryStream s = new MemoryStream())
                {
                    using (XBinaryWriter bw = new XBinaryWriter(s))
                    {
                        bw.Write(start.Ticks);
                        bw.Write(last.Ticks);
                    }
                    byte[] data = s.ToArray();
                    String sData = StringUtils.Bytes2Str(data);
                    key.SetValue(String.Empty, sData);
                }
            }
        }
	}
}
