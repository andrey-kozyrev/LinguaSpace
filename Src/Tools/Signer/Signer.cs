using System;
using System.IO;
using System.Security.Cryptography;
using LinguaSpace.Common;
using LinguaSpace.Desktop.Licensing;

namespace LinguaSpace.Signer
{
	class Signer
	{
		static private byte[] X = new byte[] { 0x26, 0x23, 0xCF, 0xA9, 0x44, 0x18, 0x6D, 0x10, 0xCE, 0x1D, 0xFD, 0xDB, 0xC6, 0xAF, 0x20, 0xD5, 0xD7, 0xF6, 0x6E, 0xD1 };

		[STAThread]
		static void Main(string[] args)
		{
			Signer signer = new Signer();
			signer.Sign(args);
		}

		void Sign(String[] args)
		{
			if (args.Length != 1)
			{
				Usage();
				return;
			}

			try
			{
				String productCode = args[0];
				byte[] pcode = StringUtils.Str2Bytes(productCode);
				for (int i = 0; i < 1; ++i)
				{
					DSAParameters dsap = CryptographyParams.DSAP;
					dsap.X = X;
					DSACryptoServiceProvider dsa = new DSACryptoServiceProvider(512);
					dsa.ImportParameters(dsap);
					String oid = CryptoConfig.MapNameToOID(LicensingStrings.SHA1);
					byte[] acode = dsa.SignHash(pcode, oid);
					String activationCode = StringUtils.Bytes2Str(acode);
					System.Console.Out.WriteLine(activationCode);
				}
			}
			catch (Exception e)
			{
				System.Console.Out.WriteLine("Error: {0}\n", e.Message);
			}
		}

		void Usage()
		{
			System.Console.Out.WriteLine("usage: LinguaSpace.Signer.exe <code>");
		}
	}
}
