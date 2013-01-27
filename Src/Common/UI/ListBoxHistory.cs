using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace LinguaSpace.Common.UI
{
	public class ListBoxHistory
	{
		private ListBox listBox;
		private bool isBacking = false;
		private Stack back;
		private Stack forward;

		public ListBoxHistory(ListBox listBox)
		{
			Debug.Assert(listBox != null);
			this.back = new Stack();
			this.forward = new Stack();
			this.listBox = listBox;
			this.listBox.SelectionChanged += new SelectionChangedEventHandler(listBox_SelectionChanged);
			
			IBindingList bl = this.listBox.ItemsSource as IBindingList;
			if (bl != null)
				bl.ListChanged += new ListChangedEventHandler(OnListChanged);
		}

		private void OnListChanged(Object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.Reset)
			{
				this.back.Clear();
				this.forward.Clear();
			}
		}

		private void listBox_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			Stack stack = this.isBacking ? this.forward : this.back;
			foreach (Object obj in e.RemovedItems)
				if (stack.Count == 0 || !stack.Peek().Equals(obj))
					stack.Push(obj);
		}

		private bool IsAlive(Object obj)
		{
			IList list = this.listBox.ItemsSource as IList;
			if (list != null)
				return list.Contains(obj);
				
			foreach (Object o in this.listBox.ItemsSource)
				if (o.Equals(obj))
					return true;
					
			return false;
		}

		private Object Unwind(Stack stack, Object selection)
		{
			Object obj = selection;
			while (stack.Count > 0 && (obj == selection || !IsAlive(obj)))
			{
				obj = stack.Pop();
			}
			return obj;
		}

		private void Step(Stack stack, bool backing)
		{
			this.isBacking = backing;
			try
			{
				Object selection = this.listBox.SelectedItem;
				Object obj = Unwind(stack, selection);
				if (obj != selection)
				{
					WPFUtils.SelectListBoxItem(this.listBox, obj);
				}
			}
			finally
			{
				this.isBacking = false;
			}
		}

		public void Back()
		{
			Step(this.back, true);
		}

		public bool CanBack()
		{
			return (this.back.Count > 0);
		}

		public void Forward()
		{
			Step(this.forward, false);
		}

		public bool CanForward()
		{
			return (this.forward.Count > 0);
		}
	}
}
