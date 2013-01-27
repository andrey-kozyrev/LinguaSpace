using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using System.Runtime.Serialization;
using System.Data.SqlServerCe;

namespace LinguaSpace.Common.Data
{
	public class DataSetBase : DataSet
	{
		public DataSetBase()
			: base()
		{
		}

		protected void AddForeignKey(DataTable tableParent, DataTable tableChild, String columnName, String constraintName, bool cascade)
		{
			AddForeignKey(tableParent, tableChild, columnName, columnName, constraintName, true, cascade);
		}

		protected void AddForeignKey(DataTable tableParent, 
									 DataTable tableChild, 
									 String columnNameParent, 
									 String columnNameChild, 
									 String constraintName, 
									 bool constraint, 
									 bool cascade)
		{
			Debug.Assert(tableParent != null);
			Debug.Assert(tableChild != null);

			DataColumn columnParent = tableParent.Columns[columnNameParent];
			DataColumn columnChild = tableChild.Columns[columnNameChild];

			Debug.Assert(columnParent != null);
			Debug.Assert(columnChild != null);

			if (constraint)
			{
				ForeignKeyConstraint fk = new ForeignKeyConstraint(constraintName, columnParent, columnChild);
				fk.AcceptRejectRule = AcceptRejectRule.Cascade;
				fk.UpdateRule = Rule.None;
				fk.DeleteRule = cascade ? Rule.Cascade : Rule.None;
				tableChild.Constraints.Add(fk);
			}

			this.Relations.Add(new DataRelation(constraintName, columnParent, columnChild, false));
		}

		protected void CloseHelper(IEnumerable<DataTableRoot> tables)
		{
			EnforceConstraints = false;
		
			foreach (DataTableRoot table in tables.Reverse<DataTableRoot>())
				table.Close();
				
			EnforceConstraints = true;
		}

		protected void LoadHelper(IEnumerable<DataTableRoot> tables, SqlCeConnection conn)
		{
			foreach (DataTableRoot table in tables)
				table.Load(conn);
		}

		protected void SaveHelper(IEnumerable<DataTableRoot> tables, SqlCeConnection conn)
		{
			// deletes from children to parents
			foreach (DataTableRoot table in tables.Reverse<DataTableRoot>())
				table.Save(conn, DataRowState.Deleted);

			// updates from parents to children
			foreach (DataTableRoot table in tables)
				table.Save(conn, DataRowState.Modified);

			// inserts from parents to children
			foreach (DataTableRoot table in tables)
				table.Save(conn, DataRowState.Added);

			// accept changes
			foreach (DataTableRoot table in tables)
				table.AcceptChanges();
		}

		public void Serialize(DataRow row, BinaryWriter writer, String[] relationNames)
		{
			Debug.Assert(writer != null);
			Debug.Assert(row != null);
            Debug.Assert(relationNames != null);

			DataTable table = row.Table;

			writer.Write(table.Columns.Count);
			for (int i = 0; i < table.Columns.Count; ++i)
			{
				DataColumn column = table.Columns[i];

				Object data = row[column];
                Type type = data.GetType();
				TypeCode code = Type.GetTypeCode(type);
				writer.Write((Int32)code);

                #region Serialize Switch
                switch (code)
				{
					case TypeCode.Empty:
                        break;
					case TypeCode.Object:
                        writer.Write(((Guid)data).ToString());
                        break;
		  			case TypeCode.DBNull:
						break;
					case TypeCode.Boolean:
						writer.Write((Boolean)data);
						break;
					case TypeCode.Char:
						writer.Write((Char)data);
						break;
					case TypeCode.SByte:
						writer.Write((SByte)data);
						break;
					case TypeCode.Byte:
						writer.Write((Byte)data);
						break;
					case TypeCode.Int16:
						writer.Write((Int16)data);
						break;
					case TypeCode.UInt16:
						writer.Write((UInt16)data);
						break;
					case TypeCode.Int32:
						writer.Write((Int32)data);
						break;
					case TypeCode.UInt32:
						writer.Write((UInt32)data);
						break;
					case TypeCode.Int64:
						writer.Write((Int64)data);
						break;
					case TypeCode.UInt64:
						writer.Write((UInt64)data);
						break;
					case TypeCode.Single:
						writer.Write((Single)data);
						break;
					case TypeCode.Double:
						writer.Write((Double)data);
						break;
					case TypeCode.Decimal:
						writer.Write((Decimal)data);
						break;
					case TypeCode.DateTime:
						writer.Write(((DateTime)data).Ticks);
						break;
					case TypeCode.String:
						writer.Write((String)data);
						break;
                }
                #endregion
            }

            IList<DataRelation> relations = table.ChildRelations.Cast<DataRelation>().Where(r => relationNames.Contains(r.RelationName)).ToList();

            writer.Write(relations.Count);

            for (int i = 0; i < relations.Count; ++i)
            {
                DataRelation relation = relations[i];
                writer.Write(relation.RelationName);
                
                DataRow[] children = row.GetChildRows(relation);
                writer.Write(children.Length);
                for (int j = 0; j < children.Length; ++j)
                {
                    Serialize(children[j], writer, relationNames);
                }
            }
		}

        public void Deserialize(DataRow row, BinaryReader reader, String relationName)
		{
            Debug.Assert(row != null);
			Debug.Assert(reader != null);

            DataTable table = row.Table;

            DataRelation relation = relationName != null ? Relations[relationName] : null;

			Int32 columnCount = reader.ReadInt32();
			
			for (int i = 0; i < columnCount; ++i)
			{
				DataColumn column = table.Columns[i];
				
				TypeCode code = (TypeCode)reader.ReadInt32();

				Object data = null;

                #region Deserialize Switch

                switch (code)
				{
					case TypeCode.Empty:
                        break;
					case TypeCode.Object:
                        data = new Guid(reader.ReadString());
                        break;
					case TypeCode.DBNull:
						break;
					case TypeCode.Boolean:
						data = reader.ReadBoolean();
						break;
					case TypeCode.Char:
						data = reader.ReadChar();
						break;
					case TypeCode.SByte:
						data = reader.ReadSByte();
						break;
					case TypeCode.Byte:
						data = reader.ReadByte();
						break;
					case TypeCode.Int16:
						data = reader.ReadInt16();
						break;
					case TypeCode.UInt16:
						data = reader.ReadUInt16();
						break;
					case TypeCode.Int32:
						data = reader.ReadInt32();
						break;
					case TypeCode.UInt32:
						data = reader.ReadUInt32();
						break;
					case TypeCode.Int64:
						data = reader.ReadInt64();
						break;
					case TypeCode.UInt64:
						data = reader.ReadUInt64();
						break;
					case TypeCode.Single:
						data = reader.ReadSingle();
						break;
					case TypeCode.Double:
						data = reader.ReadDouble();
						break;
					case TypeCode.Decimal:
						data = reader.ReadDecimal();
						break;
					case TypeCode.DateTime:
						data = new DateTime(reader.ReadInt64());
						break;
					case TypeCode.String:
						data = reader.ReadString();
						break;
                }

                #endregion

                if (column.ReadOnly)
                    continue;

                if (relation != null && relation.ChildColumns.Contains(column))
                    continue;

                if (column.ColumnName == "Position")
                    continue;

                row[column] = data;
			}
			
            Int32 childRelationCount = reader.ReadInt32();
            for (int i = 0; i < childRelationCount; ++i)
            {
                String childRelationName = reader.ReadString();
                DataRelation childRelation = Relations[childRelationName];

                Int32 childrenCount = reader.ReadInt32();
                for (int j = 0; j < childrenCount; ++j)
                {
                    DataTable childTable = childRelation.ChildTable;
                    DataRow childRow = childTable.NewRow();

                    for (int k = 0; k < childRelation.ParentColumns.Length; ++k)
                        childRow[childRelation.ChildColumns[k]] = row[childRelation.ParentColumns[k]];

                    childTable.Rows.Add(childRow);

                    Deserialize(childRow, reader, childRelationName);
                }
            }
		}
	}

	public class TransactedDataSet : DataSetBase, ITransactionContext
	{
		private DataChangeLogger logger;
		
		public TransactedDataSet()
			: base()
		{
			this.logger = new DataChangeLogger(this);
		}
		
		public ITransaction BeginTransaction()
		{
			return new Transaction(this.logger);
		}
	}
}
