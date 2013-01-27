using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace LinguaSpace.Common.Events
{
	public abstract class WeakEventManagerBase<T, TSource> : WeakEventManager
		where T : WeakEventManagerBase<T, TSource>, new()
		where TSource : class
	{
		public static T Current
		{
			get
			{
				Type managerType = typeof(T);
				T manager = WeakEventManager.GetCurrentManager(managerType) as T;
				if (manager == null)
				{
					manager = new T();
					WeakEventManager.SetCurrentManager(managerType, manager);
				}
				return manager;
			}
		}

		public static void AddListener(TSource source, IWeakEventListener listener)
		{
			Current.ProtectedAddListener(source, listener);
		}

		public static void RemoveListener(TSource source, IWeakEventListener listener)
		{
			Current.ProtectedRemoveListener(source, listener);
		}

		protected override sealed void StartListening(object source)
		{
			StartListeningTo(source as TSource);
		}

		protected abstract void StartListeningTo(TSource source);

		protected override sealed void StopListening(object source)
		{
			StopListeningTo(source as TSource);
		}

		protected abstract void StopListeningTo(TSource source);
	}

	public class NPCWeakEventManager : WeakEventManagerBase<NPCWeakEventManager, INotifyPropertyChanged>
	{
		protected override void StartListeningTo(INotifyPropertyChanged source)
		{
			source.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
		}

		protected override void StopListeningTo(INotifyPropertyChanged source)
		{
			source.PropertyChanged -= new PropertyChangedEventHandler(OnPropertyChanged);
		}

		private void OnPropertyChanged(Object sender, PropertyChangedEventArgs args)
		{
			base.DeliverEvent(sender, args);
		}
	}
	
	public class ListChangedEventManager : WeakEventManagerBase<ListChangedEventManager, IBindingList>
	{
		protected override void StartListeningTo(IBindingList source)
		{
			source.ListChanged += new ListChangedEventHandler(OnListChanged);
		}

		protected override void StopListeningTo(IBindingList source)
		{
			source.ListChanged -= new ListChangedEventHandler(OnListChanged);
		}

		private void OnListChanged(Object sender, ListChangedEventArgs args)
		{
			base.DeliverEvent(sender, args);
		}
	}

}
