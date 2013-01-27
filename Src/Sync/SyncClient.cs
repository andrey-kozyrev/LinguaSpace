using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace LinguaSpace.Sync
{
	public class SyncClient : SyncBase
	{
		public SyncClient(String folder, String mask)
			: base(folder, mask)
		{
		}

		#region Internal

		private IPAddress GetServerIP()
		{
			IPAddress ip = IPAddress.None;
			while (_live)
			{
				try
				{
					IPHostEntry entry = Dns.GetHostEntry("ppp_peer");
					ip = entry.AddressList[0];
					break;
				}
				catch (Exception e)
				{
					_eventShutdown.WaitOne(1000, false);
				}
			}
			return ip;
		}

		protected override void MainCore()
		{
			IPAddress ip = GetServerIP();
			using (TcpClient client = new TcpClient())
			{
				client.Connect(ip, PORT);
				using (Stream stream = client.GetStream())
				{
					using (SyncReader reader = new SyncReader(stream))
					{
						using (SyncWriter writer = new SyncWriter(stream))
						{
							SyncCode code = SyncCode.Error;

							// 1. Handshake
							writer.Write(SyncCode.Handshake);
							writer.Write(HANDSHAKE);
							code = reader.ReadCode();
							VerifyCode(code, SyncCode.Ok, reader, writer);

							// 2. Files
							foreach (String file in Directory.GetFiles(_folder, _mask))
							{
								writer.Write(SyncCode.File);
								writer.Write(file);

								code = reader.ReadCode();
								if (code == SyncCode.Cancel)
									continue;

								VerifyCode(code, SyncCode.Ok, reader, writer);
							}

							writer.Write(SyncCode.Cancel);
						}
					}
				}
			}
		}
		#endregion
	}
}
