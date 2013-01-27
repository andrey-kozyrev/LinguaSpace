using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Common.Resources;
using LinguaSpace.Common.Text;

namespace LinguaSpace.Common.Licensing
{
	public class Registration : NotifyPropertyChangedImpl, IRegistration
	{
		private Hardware hardware;

		private byte[] productCode;
        private byte[] keyCode;

		public Registration()
		{
			// scan hardware
			this.hardware = new Hardware();
			this.hardware.Scan();

			// build product code
			using (MemoryStream ms = new MemoryStream())
			{
				using (BinaryWriter bw = new BinaryWriter(ms))
				{
					bw.Write(CommonStrings.PRODUCT);
					bw.Write(CommonStrings.MAJOR_VERSION);
					bw.Write(CommonStrings.MINOR_VERSION);
					this.hardware.Write(bw);
					bw.Flush();
					ms.Flush();

					byte[] blob = ms.ToArray();
					HashAlgorithm hasher = HashAlgorithm.Create(CommonStrings.SHA1);
					this.productCode = hasher.ComputeHash(blob);
				}
            }
		}

		#region IRegistration Members

		public String ProductCode
		{
			get
			{
                return this.a;
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public String a
        {
            get
            {
                Debug.Assert(this.productCode != null);
                return StringUtils.Bytes2Str(this.productCode);
            }
        }
       
		public String KeyCode
		{
			get
			{
                return this.b;
			}
			set
			{
                this.b = value;
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public String b
        {
            get
            {
                String keyCodeStr = String.Empty;
                if (this.keyCode != null)
                {
                    keyCodeStr = StringUtils.Bytes2Str(this.keyCode);
                }
                return keyCodeStr;
            }
            set
            {
                Debug.Assert(value != null);
                this.keyCode = StringUtils.Str2Bytes(value);
                RaisePropertyChangedEvent(CommonStrings.b);
                RaisePropertyChangedEvent(CommonStrings.c);
            }
        }

		public bool IsKeyCodeValid
		{
			get
			{
                return c;
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
        public bool c
        {
            get
            {
                bool valid = false;
                if (this.keyCode != null)
                {
                    try
                    {
                        DSACryptoServiceProvider dsa = new DSACryptoServiceProvider(512);
                        dsa.ImportParameters(CryptographyParams.DSAP);
                        String oid = CryptoConfig.MapNameToOID(CommonStrings.SHA1);
                        valid = dsa.VerifyHash(this.productCode, oid, this.keyCode);
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                    }
                }
                return valid;
            }
        }

        /*
		public void Save()
		{
            Debug.Assert(this.keyCode != null);
			String keyCodePath = FileUtils.KeyCodeFile;
            File.Delete(keyCodePath);
            FileUtils.WriteBytes(keyCodePath, this.keyCode);
		}
         */

		#endregion
	}
}
