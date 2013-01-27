using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Windows;
using LinguaSpace.Common.Events;
using LinguaSpace.Common.Diagnostics;

namespace LinguaSpace.Common.ComponentModel
{
	public interface IPresentationModel : INotifyPropertyChanged, IDisposable
	{
		Object Data
		{
			get;
		}
		
		void NotifyPropertyChanged();
	}

	public interface IPresentationList<TPresentationModel> : IList<TPresentationModel>, IList, IDisposable
	{
		IList List
		{
			get;
		}

		void NotifyItemChanged(ListChangedType type, int index);
	}

	public class PresentationModel : NotifyPropertyChangedImpl, IPresentationModel, IWeakEventListener
	{
		private bool disposed = false;
		private Object data;
	
		protected PresentationModel()
		{
		}
	
		public PresentationModel(Object data)
		{
			Debug.Assert(data != null);
			this.data = data;
		}

		public virtual bool ReceiveWeakEvent(Type managerType, Object sender, EventArgs args)
		{
			if (managerType == typeof(NPCWeakEventManager))
				OnPropertyChanged(sender, (PropertyChangedEventArgs)args);
			else
				return false;
				
			return true;
		}

		protected virtual void OnPropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			this.RaisePropertyChangedEvent(e.PropertyName);
		}

		public Object Data
		{
			get
			{
				return this.data;
			}
		}

		public virtual void NotifyPropertyChanged()
		{
			;
		}
		
		public void Dispose()
		{
			if (!this.disposed)
			{
				Dispose(true);
				this.disposed = true;
			}
		}
				
		protected virtual void Dispose(bool disposing)
		{
			;
		}
				
		public override bool Equals(Object obj)
		{
			PresentationModel model = obj as PresentationModel;
			if (model == null)
				return false;
				
			return model.Data.Equals(this.Data);
		}
	}

	public class PresentationList<TPresentationModel> : IPresentationList<TPresentationModel> where TPresentationModel : IPresentationModel
	{
		private bool disposed = false;
		private IList list;
	
		public PresentationList(IList list)
		{
			Debug.Assert(list != null);
			this.list = list;
		}

		#region IDisposable
		
		public void Dispose()
		{
			if (!this.disposed)
			{
				Dispose(true);
				this.disposed = true;
			}
		}
		
		protected virtual void Dispose(bool disposing)
		{
			;
		}
		
		#endregion

		#region IPresentationList<T>
		
		public IList List
		{
			get
			{
				return this.list;
			}
		}

		public virtual void NotifyItemChanged(ListChangedType type, int index)
		{
			;
		}
		
		#endregion

		#region IList<T>

		TPresentationModel IList<TPresentationModel>.this[int index]
		{
			get
			{
				return (TPresentationModel)Activator.CreateInstance(typeof(TPresentationModel), this.list[index]);
			}
			set
			{
				Debug.Assert(value != null);
				this.list[index] = value.Data;
			}
		}

		int IList<TPresentationModel>.IndexOf(TPresentationModel value)
		{
			return this.list.IndexOf(value.Data);
		}

		void IList<TPresentationModel>.Insert(int index, TPresentationModel value)
		{
			this.list.Insert(index, value.Data);
		}

		void IList<TPresentationModel>.RemoveAt(int index)
		{
			this.list.RemoveAt(index);
		}

		#endregion

		#region ICollection<T>

		int ICollection<TPresentationModel>.Count 
		{ 
			get
			{
				return this.list.Count;
			}
		}

		bool ICollection<TPresentationModel>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		void ICollection<TPresentationModel>.Add(TPresentationModel value)
		{
			this.list.Add(value.Data);
		}

		void ICollection<TPresentationModel>.Clear()
		{
			this.list.Clear();
		}

		bool ICollection<TPresentationModel>.Contains(TPresentationModel value)
		{
			return this.list.Contains(value.Data);
		}

		bool ICollection<TPresentationModel>.Remove(TPresentationModel value)
		{
			int count = this.list.Count;
			this.list.Remove(value.Data);
			return (count > this.list.Count);
		}

		void ICollection<TPresentationModel>.CopyTo(TPresentationModel[] array, int arrayIndex)
		{
			foreach (TPresentationModel model in this)
				array[arrayIndex++] = model;
		}
		
		#endregion

		#region IEnumerable<T>

		IEnumerator<TPresentationModel> IEnumerable<TPresentationModel>.GetEnumerator()
		{
			foreach (Object data in this.list)
				yield return (TPresentationModel)Activator.CreateInstance(typeof(TPresentationModel), data);
		}
		
		#endregion
		
		#region IList		
		
		bool IList.IsFixedSize 
		{ 
			get
			{
				return this.list.IsFixedSize;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return this.list.IsReadOnly;
			}
		}
		
		Object IList.this[int index] 
		{ 
			get
			{
				return (TPresentationModel)Activator.CreateInstance(typeof(TPresentationModel), this.list[index]);
			}
			set
			{
				Debug.Assert(value != null);
				TPresentationModel model = (TPresentationModel)value;
				this.list[index] = model.Data;
			}
		}

		int IList.Add(Object value)
		{
			TPresentationModel model = (TPresentationModel)value;
			return this.list.Add(model.Data);
		}

		void IList.Clear()
		{
			this.list.Clear();
		}

		bool IList.Contains(Object value)
		{
			TPresentationModel model = (TPresentationModel)value;
			return this.list.Contains(model.Data);
		}

		int IList.IndexOf(Object value)
		{
			TPresentationModel model = (TPresentationModel)value;
			return this.list.IndexOf(model.Data);
		}

		void IList.Insert(int index, Object value)
		{
			TPresentationModel model = (TPresentationModel)value;
			this.list.Insert(index, model.Data);
		}

		void IList.Remove(Object value)
		{
			TPresentationModel model = (TPresentationModel)value;
			this.list.Remove(model.Data);
		}

		void IList.RemoveAt(int index)
		{
			this.list.RemoveAt(index);
		}
		
		#endregion
		
		#region ICollection

		int ICollection.Count 
		{ 
			get
			{
				return this.list.Count;
			}
		}

		bool ICollection.IsSynchronized 
		{ 
			get
			{
				return this.list.IsSynchronized;
			}
		}

		Object ICollection.SyncRoot 
		{ 
			get
			{
				return this.list.SyncRoot;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			foreach (Object model in this)
				array.SetValue(model, index++);
		}
		
		#endregion
		
		#region IEnumerable
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (Object data in this.list)
				yield return (TPresentationModel)Activator.CreateInstance(typeof(TPresentationModel), data);
		}
		
		#endregion
	}

	public class PresentationBindingList<TPresentationModel> : PresentationList<TPresentationModel>, IWeakEventListener, IBindingList where TPresentationModel : IPresentationModel
	{
		private ListChangedEventHandler listChanged;
	
		public PresentationBindingList(IBindingList bindingList)
			: base(bindingList)
		{
			ListChangedEventManager.AddListener(bindingList, this);
		}

		public bool ReceiveWeakEvent(Type managerType, Object sender, EventArgs args)
		{
			if (managerType == typeof(ListChangedEventManager))
				bindingList_ListChanged(sender, (ListChangedEventArgs)args);
			else
				return false;

			return true;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			ListChangedEventManager.RemoveListener(this.BindingList, this);
			IDisposable disposable = this.BindingList as IDisposable;
			if (disposable != null)
				disposable.Dispose();
		}

		private IBindingList BindingList
		{
			get
			{
				return (IBindingList)this.List;
			}
		}

		public override void NotifyItemChanged(ListChangedType type, int index)
		{
			Tracer.WriteLine("PresentationBindingList<T>.NotifyItemChanged");
			using (Tracer.Indent())
			{
				RaiseListChangedEvent(new ListChangedEventArgs(type, index, index));
			}
		}

		private void bindingList_ListChanged(Object sender, ListChangedEventArgs e)
		{
			Debug.Assert(sender == this.BindingList);
			RaiseListChangedEvent(e);
		}

		protected void RaiseListChangedEvent(ListChangedEventArgs e)
		{
			if (this.listChanged != null)
			{
				Tracer.WriteLine("PresentationBindingList<T>.RaiseListChangedEvent");
				using (Tracer.Indent())
				{
					switch (e.ListChangedType)
					{
						case ListChangedType.Reset:
						case ListChangedType.ItemAdded:
						case ListChangedType.ItemDeleted:
						case ListChangedType.ItemMoved:
						case ListChangedType.ItemChanged:
							this.listChanged(this, e);
							break;
					}
				}
			}
		}

		#region IBindingList

		bool IBindingList.AllowEdit 
		{ 
			get
			{
				return this.BindingList.AllowEdit;
			}
		}

		bool IBindingList.AllowNew
		{ 
			get
			{
				return this.BindingList.AllowNew;
			}
		}

		bool IBindingList.AllowRemove 
		{ 
			get
			{
				return this.BindingList.AllowRemove;
			}
		}

		bool IBindingList.IsSorted 
		{ 
			get
			{
				return false;
			}
		}

		ListSortDirection IBindingList.SortDirection 
		{ 
			get
			{
				return this.BindingList.SortDirection;
			}
		}

		PropertyDescriptor IBindingList.SortProperty 
		{ 
			get
			{
				return null;
			}
		}

		bool IBindingList.SupportsChangeNotification 
		{ 
			get
			{
				return this.BindingList.SupportsChangeNotification;
			}
		}

		bool IBindingList.SupportsSearching 
		{
			get
			{
				return false;
			}
		}

		bool IBindingList.SupportsSorting 
		{ 
			get
			{
				return false;
			}
		}

		event ListChangedEventHandler IBindingList.ListChanged
		{
			add
			{
				this.listChanged = (ListChangedEventHandler)Delegate.Combine(this.listChanged, value);
			}
			remove
			{
				this.listChanged = (ListChangedEventHandler)Delegate.Remove(this.listChanged, value);
			}
		}

		void IBindingList.AddIndex(PropertyDescriptor property)
		{
			this.BindingList.AddIndex(property);
		}

		Object IBindingList.AddNew()
		{
			Object data = this.BindingList.AddNew();
			InitializeNew(data);
			return (TPresentationModel)Activator.CreateInstance(typeof(TPresentationModel), data);
		}

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}

		int IBindingList.Find(PropertyDescriptor property, Object key)
		{
			throw new NotSupportedException();
		}

		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
			throw new NotSupportedException();
		}

		void IBindingList.RemoveSort()
		{
			throw new NotSupportedException();
		}

		#endregion
		
		protected virtual void InitializeNew(Object data)
		{	
			IEditableObject editable = data as IEditableObject;
			if (editable != null)
				editable.EndEdit();
		}
	}
}
