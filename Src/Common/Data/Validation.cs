using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Text;
using System.Diagnostics;
using System.Data;
using LinguaSpace.Common.Text;
using LinguaSpace.Common.ComponentModel;

namespace LinguaSpace.Common.Data
{
	public class DataValidator : Validator
	{
		private IList<DataTable> m_tables;
		protected DataRowView m_rv;
	
		public DataValidator(DataRowView rv)
		{
			Debug.Assert(rv != null);
			m_rv = rv;
			m_tables = new List<DataTable>();
			Hook(rv);
		}
	
		protected override INotifyPropertyChanged GetNotifyPropertyChanged(Object obj)
		{
			INotifyPropertyChanged npc = null;
		
			DataRowView drv = obj as DataRowView;
			if (drv != null)
				npc = drv.Row as INotifyPropertyChanged;
				
			if (npc != null)
				return npc;
		
			return base.GetNotifyPropertyChanged(obj);
		}

		protected DataTable GetDataTable(Object obj)
		{
			DataTable table = obj as DataTable;
			if (table != null)
				return table;
				
			DataView view = obj as DataView;
			if (view != null)
				return view.Table;
			
			return null;				
		}

		protected override void  Hook(Object obj)
		{
			DataTable table = GetDataTable(obj);
			if (table != null)
			{
				table.ColumnChanged += new DataColumnChangeEventHandler(t_ColumnChanged);
				table.RowChanged += new DataRowChangeEventHandler(t_RowChanged);
				table.RowDeleted += new DataRowChangeEventHandler(t_RowDeleted);
				m_tables.Add(table);
			}
			else
			{
				base.Hook(obj);
			}
		}

		protected override void Unhook()
		{
			base.Unhook();
			
			foreach (DataTable t in m_tables)
			{
				t.ColumnChanged -= new DataColumnChangeEventHandler(t_ColumnChanged);
				t.RowChanged -= new DataRowChangeEventHandler(t_RowChanged);
				t.RowDeleted -= new DataRowChangeEventHandler(t_RowDeleted);
			}
			m_tables.Clear();
		}

		private void t_RowDeleted(object sender, DataRowChangeEventArgs e)
		{
			IsModified = true;
			Validate();
		}

		private void t_RowChanged(object sender, DataRowChangeEventArgs e)
		{
			IsModified = true;
			Validate();
		}

		private void t_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			IsModified = true;
			Validate();
		}
	}
}
