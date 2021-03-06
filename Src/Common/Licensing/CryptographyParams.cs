using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace LinguaSpace.Common.Licensing
{
	public class CryptographyParams
	{
		protected CryptographyParams()
		{
			;
		}

		static private DSAParameters dsap;

		static CryptographyParams()
		{
			dsap = new DSAParameters();
			dsap.Counter = 380;
			dsap.G = new byte[] { 0x4E, 0x6F, 0xE7, 0x44, 0xDD, 0x60, 0x88, 0xFF, 0x2A, 0x72, 0x45, 0x70, 0xC3, 0x43, 0xE0, 0x9D, 0xCA, 0x11, 0x0F, 0x38, 0x06, 0xB2, 0xBE, 0x96, 0x72, 0xB8, 0xD2, 0xD5, 0xEB, 0xF7, 0xD3, 0xD2, 0x9B, 0xFE, 0x8A, 0x10, 0xFB, 0x93, 0xB1, 0x76, 0xBB, 0x7D, 0xAC, 0x6B, 0x0E, 0xAE, 0xD3, 0x9E, 0x73, 0xCA, 0x9C, 0x29, 0x01, 0x84, 0xA4, 0x97, 0xB4, 0x59, 0xA1, 0x65, 0x9F, 0xD6, 0x0B, 0x94 };
			dsap.J = new byte[] { 0xBB, 0x6C, 0x04, 0xD1, 0xCD, 0x7B, 0xDF, 0xB4, 0x4D, 0x70, 0xDC, 0x1E, 0xBD, 0xC8, 0xA3, 0x21, 0xB5, 0xA0, 0x80, 0xBC, 0xF1, 0xA6, 0x61, 0xBA, 0x51, 0x11, 0x15, 0xD4, 0x86, 0xEC, 0x28, 0x55, 0x7E, 0x2A, 0x21, 0xAD, 0xFF, 0x96, 0xEB, 0x8C, 0xB1, 0xF4, 0xA0, 0x5A };
			dsap.P = new byte[] { 0x81, 0x66, 0x5B, 0xF0, 0x6C, 0x13, 0x07, 0x62, 0xEC, 0x5E, 0xB8, 0x8E, 0xEE, 0x76, 0xF2, 0x32, 0x4C, 0x01, 0x7B, 0x81, 0x45, 0x08, 0x22, 0x5D, 0x79, 0x6F, 0x3B, 0x88, 0x07, 0x43, 0x75, 0x23, 0x9D, 0x62, 0xB6, 0x7F, 0xE5, 0xC4, 0x01, 0x4F, 0x63, 0xAE, 0x7B, 0x58, 0xA9, 0x2C, 0x70, 0xF3, 0x2A, 0xFD, 0x2E, 0x72, 0xF3, 0x90, 0x01, 0x6B, 0xCF, 0x68, 0xE5, 0x46, 0x54, 0xD5, 0xED, 0x8F };
			dsap.Q = new byte[] { 0xB0, 0xBF, 0x5B, 0x8F, 0xAD, 0xC5, 0x15, 0x3D, 0x52, 0x90, 0x1B, 0x7D, 0x58, 0x6A, 0x1F, 0x5E, 0x47, 0x09, 0xC7, 0x43 };
			dsap.Seed = new byte[] { 0xA0, 0x13, 0xA1, 0x17, 0x47, 0x9D, 0x0B, 0x7B, 0x2C, 0xA5, 0x70, 0x72, 0xF4, 0x88, 0x72, 0x89, 0x54, 0x12, 0x4C, 0x64 };
			dsap.Y = new byte[] { 0x20, 0xA5, 0x63, 0x06, 0xA2, 0x59, 0xA1, 0x2F, 0x6B, 0xE3, 0x49, 0x83, 0x29, 0x8D, 0x5A, 0xA7, 0x8B, 0x9E, 0xF8, 0x33, 0x32, 0xE9, 0xB5, 0xFB, 0x52, 0xF3, 0xD3, 0xB4, 0x7B, 0xEA, 0xDB, 0xC1, 0xB5, 0x8E, 0x89, 0x47, 0x29, 0x4C, 0xD9, 0x33, 0xD0, 0xA8, 0xDA, 0x1E, 0x93, 0x70, 0x80, 0x3D, 0x55, 0x60, 0x35, 0x95, 0x6F, 0xD9, 0xD4, 0xD1, 0x5E, 0x06, 0x41, 0x3E, 0x3E, 0xEC, 0xD6, 0x14 };
		}

		static public DSAParameters DSAP
		{
			get
			{
				return dsap;
			}
		}
	}
}
