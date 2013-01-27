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
	public class SyncServer : SyncBase
	{
		public SyncServer(String folder, String mask)
			: base(folder, mask)
		{
		}

		protected override void MainCore()
		{
			TcpListener listener = new TcpListener(IPAddress.Any, PORT);
			listener.Start();
			using (TcpClient client = listener.AcceptTcpClient())
			{
				using (Stream stream = client.GetStream())
				{
					using (SyncReader reader = new SyncReader(stream))
					{
						using (SyncWriter writer = new SyncWriter(stream))
						{
							SyncCode code = SyncCode.Error;
							
							code = reader.ReadCode();
							VerifyCode(code, SyncCode.Handshake, reader, writer);
							String handshake = reader.ReadString();
							VerifyHandshake(handshake, HANDSHAKE, writer);

							writer.Write(SyncCode.Ok);

							while (true)
							{
								code = reader.ReadCode();
								if (code == SyncCode.Cancel)
									break;

								VerifyCode(code, SyncCode.File, reader, writer);
								String file = reader.ReadString();
								writer.Write(SyncCode.Cancel);
							}
						}
					}
				}
			}
		}
	}
}
