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
	public abstract class SyncBase
	{
		protected String _folder;
		protected String _mask;

		protected const int PORT = 1945;

		protected ManualResetEvent _eventShutdown;

		protected bool _live;
		protected bool _dirty;

		protected const String HANDSHAKE = "LinguaSpace Sync 1.0";

		protected Thread _threadMain;

		protected SyncBase(String folder, String mask)
		{
			Debug.Assert(folder != null);
			Debug.Assert(mask != null);
			Debug.Assert(Directory.Exists(folder));
			_folder = folder;
			_mask = mask;
			_eventShutdown = new ManualResetEvent(false);
			_live = true;
			_dirty = true;
		}

		public void StartUp()
		{
			VerifyNotStarted();
			_eventShutdown.Reset();
			_threadMain = new Thread(MainProc);
			_threadMain.Start();
		}

		public void Shutdown()
		{
			_live = false;
			_threadMain.Join();
			_eventShutdown.Set();
		}

		private void MainProc()
		{
			while (_live)
			{
				try
				{
					MainCore();
				}
				catch (Exception e)
				{
					Trace.WriteLine(e);
				}
			}
		}

		protected abstract void MainCore();

		#region Internal

		protected void VerifyNotStarted()
		{
			Debug.Assert(_threadMain == null);
			if (_threadMain != null)
			{
				throw new SyncException("Synchronization manager is started");
			}
		}

		protected void VerifyCode(SyncCode actual, SyncCode expected, SyncReader reader, SyncWriter writer)
		{
			if (actual == SyncCode.Error)
			{
				String msg = reader.ReadString();
				throw new SyncException(String.Format("Peer reported error: {0}", msg));
			}
			else if (actual != expected)
			{
				String msg = String.Format("Unexpected code {0} when looking for {1}", actual, expected);
				writer.Write(SyncCode.Error);
				writer.Write(msg);
				throw new SyncException(msg);
			}
		}

		protected void VerifyHandshake(String actual, String expected, SyncWriter writer)
		{
			if (expected != actual)
			{
				String msg = String.Format("Unexpected handshake '{0}' when looking for '{1}'", actual, expected);
				writer.Write(SyncCode.Error);
				writer.Write(msg);
				throw new SyncException(msg);
			}
		}

		protected void VerifyLive()
		{
			if (!_live)
			{
				throw new SyncException("Shutdown invoked");
			}
		}

		#endregion
	}
}
