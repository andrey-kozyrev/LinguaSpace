using System;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using LinguaSpace.Common.Diagnostics;
using LinguaSpace.Common.Events;

namespace LinguaSpace.Common.ComponentModel
{
	public class FilterBindingList : IBindingList, IList, ICollection, IEnumerable, IWeakEventListener, IDisposable
	{
		private bool disposed = false;
	
		private readonly IBindingList sourceList;
		private readonly PropertyDescriptor filterProperty;

		private Object filterValue;
		private int begin = 0;
		private int end = 0;
		
		public FilterBindingList(IBindingList sourceList, PropertyDescriptor filterProperty)
		{
			Tracer.WriteLine("FilterBindingList.FilterBindingList");
			Debug.Assert(sourceList != null);
			Debug.Assert(filterProperty != null);
			this.sourceList = sourceList;
			this.filterProperty = filterProperty;
			this.begin = 0;
			this.end = this.sourceList.Count;
			ListChangedEventManager.AddListener(this.sourceList, this);
		}

		public bool ReceiveWeakEvent(Type managerType, Object sender, EventArgs args)
		{
			if (managerType == typeof(ListChangedEventManager))
				sourceList_ListChanged(sender, (ListChangedEventArgs)args);
			else
				return false;

			return true;
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
			if (disposing)
				ListChangedEventManager.RemoveListener(this.sourceList, this);
		}

		private bool IsFiltered
		{
			get
			{
				return (this.filterValue != null);	
			}
		}
		
		private bool IsRangeValid
		{
			get
			{
				return (this.end > this.begin);
			}
		}

		private void sourceList_ListChanged(Object sender, ListChangedEventArgs e)
		{
			Tracer.WriteLine("FilterBindingList.sourceList_ListChanged {0} : {1}", e.ListChangedType, e.NewIndex);
			using (Tracer.Indent())
			{
				if (!this.IsFiltered)
				{
					RaiseListChanged(e);
					return;
				}

				switch (e.ListChangedType)
				{
					case ListChangedType.Reset:
						{
							ResetRange();
							RebuildRange();
							RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
						}
						break;
					case ListChangedType.ItemAdded:
						{
							ResetRange();
							RebuildRange();
							if (this.begin <= e.NewIndex && e.NewIndex < this.end)
								RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, e.NewIndex - this.begin));
						}
						break;
					case ListChangedType.ItemDeleted:
						{
							int begin = this.begin;
							int end = this.end;

							ResetRange();
							RebuildRange();

							if (begin <= e.NewIndex && e.NewIndex < end)
								RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, e.NewIndex - begin));
						}
						break;
					case ListChangedType.ItemMoved:
						{
							if ((this.begin <= e.OldIndex && e.OldIndex < this.end) &&
								(this.begin <= e.NewIndex && e.NewIndex < this.end))
								RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemMoved, e.NewIndex - this.begin, e.OldIndex - this.begin));
						}
						break;
					case ListChangedType.ItemChanged:
						{
							if (this.begin <= e.NewIndex && e.NewIndex < this.end)
								RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, e.NewIndex - this.begin));
						}
						break;
				}
			}
		}

		private void RaiseListChanged(ListChangedEventArgs e)
		{
			if (this.ListChanged != null)
			{
				Tracer.WriteLine("FilterBindingList.RaiseListChanged");
				this.ListChanged(this, e);
			}
		}
		
		public Object FilterValue
		{
			get
			{
				return this.filterValue;
			}
			set
			{
				Tracer.WriteLine("FilterBindingList.FilterValue.set");
				using (Tracer.Indent())
				{
					this.filterValue = value;
					ResetRange();
					if (this.IsFiltered)
						RebuildRange();
					RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
				}
			}
		}

		protected void ResetRange()
		{
			this.begin = 0;
			this.end = 0;
		}

		protected void RebuildRange()
		{
			Debug.Assert(this.IsFiltered);

			Tracer.WriteLine("FilterBindingList.RebuildRange");

			/*
			foreach (Object obj in this.sourceList)
				Tracer.WriteLine(this.filterProperty.GetValue(obj).ToString());
			 */
		
			int pos = this.sourceList.Find(this.filterProperty, filterValue);
			
			if (pos >= 0)
			{
				int pos1 = pos;
				while (pos1 >= 0 && this.filterProperty.GetValue(this.sourceList[pos1]).Equals(this.filterValue))
					this.begin = pos1--;

				int pos2 = pos;
				while (pos2 < this.sourceList.Count && this.filterProperty.GetValue(this.sourceList[pos2]).Equals(this.filterValue))
					this.end = ++pos2;
			}
		}

		#region IBindingList
		
		public bool AllowEdit 
		{ 
			get
			{
				return this.sourceList.AllowEdit;
			}
		}
		
		public bool AllowNew 
		{ 
			get
			{
				return this.sourceList.AllowNew;
			}
		}
		
		public bool AllowRemove 
		{ 
			get
			{
				return this.sourceList.AllowRemove;
			}
		}
		
		public bool IsSorted 
		{ 
			get
			{
				return this.sourceList.IsSorted;
			}
		}
		
		public ListSortDirection SortDirection 
		{
			get
			{
				return this.sourceList.SortDirection;
			}
		}
		
		public PropertyDescriptor SortProperty 
		{	
			get
			{
				return this.filterProperty;
			}
		}
		
		public bool SupportsChangeNotification 
		{ 
			get
			{
				return this.sourceList.SupportsChangeNotification;
			}
		}
		
		public bool SupportsSearching 
		{ 
			get
			{
				return true;
			}
		}
		
		public bool SupportsSorting 
		{ 
			get
			{
				return false;
			}
		}
		
		public event ListChangedEventHandler ListChanged;
		
		public void AddIndex(PropertyDescriptor property)
		{
			throw new NotImplementedException();
		}
		
		public Object AddNew()
		{
			Tracer.WriteLine("FilterBindingList.AddNew");
			using (Tracer.Indent())
			{
				Object item = this.sourceList.AddNew();
				this.filterProperty.SetValue(item, this.filterValue);
				return item;
			}
		}
		
		public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			throw new NotImplementedException();
		}
		
		public int Find(PropertyDescriptor property, Object key)
		{
			Tracer.WriteLine("FilterBindingList.Find");
			if (!this.IsFiltered)
				return this.sourceList.Find(property, key);
		
			for (int i = 0; i < this.Count; ++i)
				if (property.GetValue(this[i]).Equals(key))
					return i;
			return -1;
		}
		
		public void RemoveIndex(PropertyDescriptor property)
		{
			throw new NotImplementedException();
		}
		
		public void RemoveSort()
		{
			throw new NotImplementedException();
		}
		
		#endregion

		#region IList

		public bool IsFixedSize 
		{ 
			get
			{
				return this.sourceList.IsFixedSize;
			}
		}

		public bool IsReadOnly 
		{ 
			get
			{
				return this.sourceList.IsReadOnly;
			}
		}

		public Object this[int index] 
		{ 
			get
			{
				Debug.Assert(!IsFiltered || IsRangeValid);
				return this.sourceList[this.begin + index];
			} 
			
			set
			{
				Debug.Assert(!IsFiltered || IsRangeValid);
				this.sourceList[this.begin + index] = value;
			}
		}

		public int Add(Object item)
		{
			Tracer.WriteLine("FilterBindingList.Add");
			using (Tracer.Indent())
			{
				if (!IsFiltered)
					return this.sourceList.Add(item);

				if (!this.filterProperty.GetValue(item).Equals(this.filterValue))
					this.filterProperty.SetValue(item, this.filterValue);
				return this.sourceList.Add(item) - this.begin;
			}
		}

		public void Clear()
		{
			Tracer.WriteLine("FilterBindingList.Clear");
			using (Tracer.Indent())
			{
				if (!this.IsFiltered)
				{
					this.sourceList.Clear();
					return;
				}

				if (!this.IsRangeValid)
					return;

				this.sourceList.ListChanged -= new ListChangedEventHandler(sourceList_ListChanged);
				try
				{
					for (int i = this.end - 1; i >= this.begin; --i)
						this.sourceList.RemoveAt(i);
					ResetRange();
				}
				finally
				{
					this.sourceList.ListChanged += new ListChangedEventHandler(sourceList_ListChanged);
				}

				RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
			}
		}

		public bool Contains(Object value)
		{
			if (!this.IsFiltered)
				return this.sourceList.Contains(value);

			if (!this.IsRangeValid)
				return false;
		
			foreach (Object item in this)
				if (item.Equals(value))
					return true;
			
			return false;
		}

		public int IndexOf(Object item)
		{
			if (!this.IsFiltered)
				return this.sourceList.IndexOf(item);
		
			if (!this.IsRangeValid)
				return -1;

			for (int i = 0; i < this.Count; ++i)
				if (this[i].Equals(item))
					return i;
			return -1;
		}

		public void Insert(int index, Object value)
		{
			Tracer.WriteLine("FilterBindingList.Insert");
			using (Tracer.Indent())
			{
				Add(value);
			}
		}

		public void Remove(Object value)
		{
			Tracer.WriteLine("FilterBindingList.Remove");
			using (Tracer.Indent())
			{
				if (!this.IsFiltered)
				{
					this.sourceList.Remove(value);
					return;
				}

				if (!this.IsRangeValid)
					return;

				for (int i = 0; i < this.Count; ++i)
					if (this[i].Equals(value))
						this.sourceList.RemoveAt(this.begin + i);
			}
		}

		public void RemoveAt(int index)
		{
			Tracer.WriteLine("FilterBindingList.RemoveAt");
			using (Tracer.Indent())
			{
				if (!this.IsFiltered)
				{
					this.sourceList.RemoveAt(index);
					return;
				}

				if (!this.IsRangeValid)
					throw new InvalidOperationException();

				this.sourceList.RemoveAt(this.begin + index);
			}		
		}

		#endregion

		#region ICollection

		public int Count 
		{ 
			get
			{
				return this.end - this.begin;
			}
		}

		public bool IsSynchronized 
		{ 
			get
			{
				return this.sourceList.IsSynchronized;
			}
		}

		public Object SyncRoot 
		{ 
			get
			{
				return this.sourceList.SyncRoot;
			}
		}

		public void CopyTo(Array array, int index)
		{
			foreach (Object item in this)
				array.SetValue(item, index++);
		}
		
		#endregion

		#region IEnumerable
		
		public IEnumerator GetEnumerator()
		{
			for (int i = this.begin; i < this.end; ++i)
				yield return this.sourceList[i];
		}
		
		#endregion
	}
}
