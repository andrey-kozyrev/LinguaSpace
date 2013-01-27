using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace LinguaSpace.Common.Diagnostics
{
	public static class Tracer
	{
		public const bool ON = true;
		
		private class IndentGuard : IDisposable
		{
			private bool live = true;
		
			public IndentGuard()
			{
				#if PLATFORM_DESKTOP
				Trace.Indent();
				#endif
			}
			
			public void Dispose()
			{
				if (this.live)
				{
					#if PLATFORM_DESKTOP
					Trace.Unindent();
					#endif
					this.live = false;
				}
			}
		}
		
		public static void WriteLine(String text)
		{
			Trace.WriteLineIf(ON, text);
		}
		
		
		
		public static void WriteLine(String format, params Object[] args)
		{
			Trace.WriteLineIf(ON, String.Format(format, args));
		}
		
		public static void WriteLineIf(bool condition, String format, params Object[] args)
		{
			if (condition)
				WriteLine(format, args);
		}
		
		public static IDisposable Indent()
		{
			return new IndentGuard();
		}
	}
}
