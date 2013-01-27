using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinguaSpace.Sync
{
	public class SyncException : InvalidOperationException
	{
		public SyncException(String message)
			: base(message)
		{
		}

		public SyncException(String message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
