using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSpace.Common.Events
{
	public delegate void UnregisterCallback<E>(EventHandler<E> eventHandler) where E : EventArgs;
	
	public interface IWeakEventHandler<E> where E : EventArgs
	{
		EventHandler<E> Handler
		{
			get;
		}
	}

	public class WeakEventHandler<T, E> : IWeakEventHandler<E>
		where T : class 
		where E : EventArgs
	{
		private delegate void OpenEventHandler(T @this, Object sender, E e);
		
		private WeakReference targetRef;
		private OpenEventHandler openHandler;
		private EventHandler<E> handler;
		private UnregisterCallback<E> unregister;
		
		public WeakEventHandler(EventHandler<E> handler, UnregisterCallback<E> unregister)
		{
			this.targetRef = new WeakReference(handler.Target);
			this.openHandler = (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler), null, handler.Method);
			this.handler = this.Invoke;
			this.unregister = unregister;
		}
		
		public void Invoke(Object sender, E e)
		{
			T target = (T)this.targetRef.Target;
			
			if (target != null)
			{
				this.openHandler.Invoke(target, sender, e);
			}
			else if (this.unregister != null)
			{
				this.unregister(this.handler);
				this.unregister = null;
			}
		}
		
		public EventHandler<E> Handler
		{
			get
			{
				return this.handler;
			}
		}
		
		public static implicit operator EventHandler<E>(WeakEventHandler<T, E> weh)
		{
			return weh.Handler;
		}
	}

}
