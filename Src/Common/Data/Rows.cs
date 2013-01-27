using System;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace LinguaSpace.Common.Data
{
	public class DataRowBase : DataRow, INotifyPropertyChanged
	{
		public DataRowBase(DataRowBuilder builder)
			: base(builder)
		{
		}

		public virtual void OnNewRow()
		{
			// initialize primary key
			foreach (DataColumn column in this.Table.PrimaryKey)
			{
				if (column.DataType == typeof(Guid))
				{
					this[column] = Guid.NewGuid();
				}
			}
			
			// initialize position if any
			if (this.Table.Columns.Contains("Position"))
			{
				this["Position"] = AdoUtils.GetNextPosition(this.Table.DefaultView, "Position");
			}
		}

		public virtual void OnColumnChanged(DataColumn column)
		{
			RaisePropertyChanged(new PropertyChangedEventArgs(column.ColumnName));
		}
		
		protected void RaisePropertyChanged(PropertyChangedEventArgs e)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, e);
			}
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		public void SetParentRow(DataRow row, String relationName)
		{
			Debug.Assert(row != null);
			DataRelation relation = this.Table.ParentRelations[relationName];
			Debug.Assert(relation != null);
			SetParentRow(row, relation);
		}
	}
}
