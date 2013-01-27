using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using LSRAPI;

namespace RAPITest
{
	class Program
	{
		static void Main(string[] args)
		{
			IRAPIUtils u = new RAPIUtilsClass();
			ConnectionInfo info = u.GetConnectionInfo();
			Console.WriteLine("Device: {0}:{1}", new IPAddress(info.device.ip), info.device.port);
			Console.WriteLine("Desktop: {0}:{1}", new IPAddress(info.desktop.ip), info.desktop.port);
			Console.WriteLine("");
		}
	}
}
