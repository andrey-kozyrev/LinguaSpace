using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Diagnostics;

namespace LinguaSpace.Common.ComponentModel
{
	// basic INotifyPropertyChanged implementation
	// + a wrapper class that can translate collection changed even into property change event
	public class NotifyPropertyChangedImpl : INotifyPropertyChanged
	{
		private bool raiseEvents = true;

		protected bool RaiseEvents
		{
			get
			{
				return this.raiseEvents;
			}
			set
			{
				this.raiseEvents = value;
			}
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChangedEvent(String propertyName)
		{
			if (this.PropertyChanged != null && this.raiseEvents)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

#if PLATFORM_DESKTOP
		protected class NotifyCollectionPropertyChangedWrapper
		{
			private NotifyPropertyChangedImpl obj;
			private String property;

			public NotifyCollectionPropertyChangedWrapper(NotifyPropertyChangedImpl obj, String property)
			{
				Debug.Assert(obj != null);
				Debug.Assert(property != null);
				this.obj = obj;
				this.property = property;
			}

			public void CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
			{
				obj.RaisePropertyChangedEvent(property);
			}
		}
#endif
	}
}
