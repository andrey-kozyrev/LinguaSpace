using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Diagnostics;

namespace LinguaSpace.Common.Data
{
	public interface ITransaction : IDisposable
	{
		bool Success
		{
			get;
			set;
		}
	}
	
	public interface ITransactionContext
	{
		ITransaction BeginTransaction();
	}
	
	public interface ITransactionContextAware
	{
		ITransactionContext TransactionContext
		{
			get;
		}
	}

	abstract internal class DataChangeBase
	{
		internal abstract void Rollback();
		internal abstract bool IsIdentical(DataChangeBase other);
	}

	internal class NoneDataChange : DataChangeBase
	{
		internal override void Rollback()
		{
			;
		}

		internal override bool IsIdentical(DataChangeBase other)
		{
			return false;
		}
	}

	abstract internal class DataChange : DataChangeBase
	{
		protected DataTable table;
		protected Object[] originalData;

		protected DataChange(DataRow row)
		{
			Debug.Assert(row != null);

			this.table = row.Table;
			Debug.Assert(this.table.PrimaryKey.Length > 0);

			this.originalData = row.ItemArray;
		}

		internal override bool IsIdentical(DataChangeBase other)
		{
			if (this.GetType() != other.GetType())
				return false;

			DataChange otherTableChange = (DataChange)other;

			if (this.table != otherTableChange.table)
				return false;

			foreach (DataColumn column in this.table.PrimaryKey)
				if (!this.originalData[column.Ordinal].Equals(otherTableChange.originalData[column.Ordinal]))
					return false;

			return true;
		}
	}

	internal class AddDataChange : DataChange
	{
		internal AddDataChange(DataRow row)
			: base(row)
		{
		}

		internal override void Rollback()
		{
			DataRow row = table.LoadDataRow(originalData, LoadOption.Upsert);

			Debug.Assert(row != null);
			Debug.Assert(row.RowState == DataRowState.Added);

			Trace.WriteLine(String.Format("~INSERT\t{0}({1})", row.Table.TableName, row.ItemArray[0]));

			table.Rows.Remove(row);
		}
	}

	internal class ModifyDataChange : DataChange
	{
		internal ModifyDataChange(DataRow row)
			: base(row)
		{
		}

		internal override void Rollback()
		{
			DataRow row = table.LoadDataRow(originalData, LoadOption.Upsert);
			Trace.WriteLine(String.Format("~UPDATE\t{0}({1})", row.Table.TableName, row.ItemArray[0]));
			Debug.Assert(row != null);
			for (int i = 0; i < this.originalData.Length; ++i)
				if (!table.Columns[i].ReadOnly)
					row[i] = originalData[i];
		}
	}

	internal class DeleteDataChange : DataChange
	{
		private DataRow originalRow;
		protected DataRowState originalState;

		internal DeleteDataChange(DataRow row)
			: base(row)
		{
			this.originalRow = row;
			this.originalState = row.RowState;
		}

		internal override void Rollback()
		{
			if (this.originalRow.RowState == DataRowState.Deleted)
				this.originalRow.RejectChanges();

			DataRow row = table.LoadDataRow(originalData, LoadOption.Upsert);
			Debug.Assert(row != null);

			Trace.WriteLine(String.Format("~DELETE\t{0}({1})", row.Table.TableName, row.ItemArray[0]));

			if (row.RowState == DataRowState.Unchanged)
			{
				if (this.originalState == DataRowState.Added)
					row.SetAdded();
				else if (this.originalState == DataRowState.Modified)
					row.SetModified();
			}
		}
	}

	public class DataChangeLogger
	{
		private DataSet ds;
		private Stack<DataChangeBase> changes;
		private List<Object> stakeholders;

		public DataChangeLogger(DataSet ds)
		{
			Debug.Assert(ds != null);
			this.ds = ds;
			this.changes = new Stack<DataChangeBase>();
			this.stakeholders = new List<Object>();
		}

		private void Hook()
		{
			Debug.Assert(ds != null);

			foreach (DataTable table in ds.Tables)
			{
				table.ColumnChanging += new DataColumnChangeEventHandler(table_ColumnChanging);
				table.RowChanging += new DataRowChangeEventHandler(table_RowChanging);
				table.RowDeleting += new DataRowChangeEventHandler(table_RowDeleting);
			}
		}

		private void Unhook()
		{
			Debug.Assert(this.ds != null);

			foreach (DataTable table in ds.Tables)
			{
				table.ColumnChanging -= new DataColumnChangeEventHandler(table_ColumnChanging);
				table.RowChanging -= new DataRowChangeEventHandler(table_RowChanging);
				table.RowDeleting -= new DataRowChangeEventHandler(table_RowDeleting);
			}
		}

		private void Log(DataChangeBase change)
		{
			Debug.Assert(change != null);
			if (changes.Count == 0 || !changes.Peek().IsIdentical(change))
			{
				Trace.WriteLine("*** logged");
				changes.Push(change);
			}
		}

		private void table_ColumnChanging(Object sender, DataColumnChangeEventArgs e)
		{
			if (e.Row.RowState == DataRowState.Added || e.Row.RowState == DataRowState.Modified || e.Row.RowState == DataRowState.Unchanged)
			{
				Trace.WriteLine(String.Format("UPDATE\t{0}({1}) : {2} -> {3}", e.Row.Table.TableName, e.Row.ItemArray[0], e.Row[e.Column], e.ProposedValue));
				Log(new ModifyDataChange(e.Row));
			}
		}

		private void table_RowDeleting(Object sender, DataRowChangeEventArgs e)
		{
			Trace.WriteLine(String.Format("DELETE\t{0}({1})", e.Row.Table.TableName, e.Row.ItemArray[0]));
			Log(new DeleteDataChange(e.Row));
		}

		private void table_RowChanging(Object sender, DataRowChangeEventArgs e)
		{
			if (e.Row.RowState == DataRowState.Detached && e.Action == DataRowAction.Add)
			{
				Trace.WriteLine(String.Format("INSERT\t{0}({1})", e.Row.Table.TableName, e.Row.ItemArray[0]));
				Log(new AddDataChange(e.Row));
			}
		}

		internal int Enlist(Object stakeholder)
		{
			Debug.Assert(stakeholder != null);

			if (this.stakeholders.Count == 0)
			{
				Hook();
			}

			this.stakeholders.Add(stakeholder);

			Log(new NoneDataChange());

			return this.changes.Count - 1;
		}

		internal void Retire(Object stakeholder)
		{
			Debug.Assert(stakeholder != null);

			this.stakeholders.Remove(stakeholder);

			if (this.stakeholders.Count == 0)
			{
				Unhook();
				this.changes.Clear();
			}
		}

		internal void Undo(int scope)
		{
			ds.EnforceConstraints = false;

			Unhook();

			while (changes.Count > scope)
				changes.Pop().Rollback();

			ds.EnforceConstraints = true;

			Hook();
		}
	}

	public class Transaction : ITransaction, IDisposable
	{
		private DataChangeLogger logger;
		private bool success;
		private int scope;

		internal Transaction(DataChangeLogger logger)
		{
			Debug.Assert(logger != null);
			this.logger = logger;
			this.scope = logger.Enlist(this);
		}

		public bool Success
		{
			get
			{
				return this.success;
			}
			set
			{
				this.success = value;
			}
		}

		public void Dispose()
		{
			if (this.logger != null)
			{
				if (!this.success)
					this.logger.Undo(this.scope);
				this.logger.Retire(this);
				this.logger = null;
			}
		}
	}
}
