using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using System.Data.Common;

namespace LinguaSpace.Common.Data
{
	public abstract class DataTableRoot : DataTable
	{
		protected DataTableRoot(String name)
			: base(name)
		{
			this.TableNewRow += new DataTableNewRowEventHandler(OnTableNewRow);
			this.CaseSensitive = true;
		}

		protected static DataColumn CreateDataColumn(String name, Type type, bool readOnly, Object defaultValue, int maxLength)
		{
			DataColumn column = CreateDataColumn(name, type, readOnly, defaultValue);
			column.MaxLength = maxLength;
			return column;
		}

		protected static DataColumn CreateDataColumn(String name, Type type, bool readOnly, Object defaultValue, bool allowDBNull)
		{
			DataColumn column = new DataColumn(name, type);
			column.ReadOnly = readOnly;
			column.AllowDBNull = allowDBNull;
			column.DefaultValue = defaultValue;
			return column;
		}

		protected static DataColumn CreateDataColumn(String name, Type type, bool readOnly, Object defaultValue)
		{
			return CreateDataColumn(name, type, readOnly, defaultValue, false);
		}

		private void OnTableNewRow(Object sender, DataTableNewRowEventArgs e)
		{
			DataRowBase row = (DataRowBase)e.Row;
			row.OnNewRow();
		}

		protected override void OnColumnChanged(DataColumnChangeEventArgs e)
		{
			base.OnColumnChanged(e);
			DataRowBase row = (DataRowBase)e.Row;
			row.OnColumnChanged(e.Column);
		}

		protected abstract DbDataAdapter CreateAdapter(DbConnection conn);

		public void Close()
		{
			this.Clear();
		}

		public void Load(DbConnection conn)
		{
			this.CreateAdapter(conn).Fill(this);
		}



		public void Save(DbConnection conn, DataRowState state)
		{
            DataRow[] changes = GetChanges(state);
            if (changes != null)
                CreateAdapter(conn).Update(changes);

            /*

			DataTable changes = this.GetChanges(state);
			if (changes == null)
				return;

			DbDataAdapter adapter = CreateAdapter(conn);

			DataRelation relation = FindSelfRelation(changes);
			if (relation == null)
				SaveSimple(adapter, changes);
			else if (state == DataRowState.Added)
				SaveUpDown(adapter, relation, changes);
			else
				SaveDownUp(adapter, relation, changes);
             */
		}

        private new DataRow[] GetChanges(DataRowState state)
        {
            DataTable changes = base.GetChanges(state);
            if (changes == null)
                return null;

            IList<DataRow> rows = new List<DataRow>(changes.Rows.Cast<DataRow>());

            DataRelation relation = FindSelfRelation();
            if (relation == null)
                return rows.ToArray();

            String parentColumnName = relation.ParentColumns[0].ColumnName;
            String childColumnName = relation.ChildColumns[0].ColumnName;

            DataRowVersion version = (state == DataRowState.Deleted) ? DataRowVersion.Original : DataRowVersion.Current;

            foreach (DataRow row in changes.Rows)
            {
                rows.Remove(row);

                Object key = row[childColumnName, version];
                int index = rows.Count;
                for (int i = 0; i < rows.Count; ++i)
                {
                    Object parentKey = rows[i][parentColumnName, version];
                    if (parentKey.Equals(key))
                    {
                        index = (state == DataRowState.Added) ? i + 1 : i;
                        break;
                    }
                }

                rows.Insert(index, row);
            }

            return rows.ToArray<DataRow>();
        }

		private void SaveSimple(DbDataAdapter adapter, DataTable changes)
		{
			adapter.Update(changes);
		}

		protected DataRelation FindSelfRelation()
		{
			foreach (DataRelation relation in ParentRelations)
				if (relation.ParentTable == this)
					return relation;
			return null;
		}
	}

	public abstract class DataTableBase<TDataRow> : DataTableRoot where TDataRow : DataRowBase
	{
		protected DataTableBase(String name)
			: base(name)
		{
		}

		protected override Type GetRowType()
		{
			return typeof(TDataRow);
		}

		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return (DataRow)Activator.CreateInstance(typeof(TDataRow), builder);
		}
	}
}
