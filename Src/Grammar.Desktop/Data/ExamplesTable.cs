using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using LinguaSpace.Common.Data;
using LinguaSpace.Grammar.Resources;

namespace LinguaSpace.Grammar.Data
{
	public class ExamplesTable : DataTableBase<DataRowBase>
	{
		protected DataColumn _columnExampleId;
		protected DataColumn _columnRuleId;
		protected DataColumn _columnPosition;
		protected DataColumn _columnTargetText;
		protected DataColumn _columnNativeText;
		protected DataColumn _columnActive;
		protected DataColumn _columnException;

		protected UniqueConstraint _constraintPkExamples;

		public ExamplesTable()
			: base(Strings.TABLE_EXAMPLES)
		{
			_columnExampleId = CreateDataColumn(Strings.COL_EXAMPLE_ID, typeof(Guid), true, Guid.Empty);
			_columnRuleId = CreateDataColumn(Strings.COL_RULE_ID, typeof(Guid), true, Guid.Empty);
			_columnPosition = CreateDataColumn(Strings.COL_POSITION, typeof(Int16), false, 0);
			_columnTargetText = CreateDataColumn(Strings.COL_TARGET_TEXT, typeof(String), false, String.Empty, 2000);
			_columnNativeText = CreateDataColumn(Strings.COL_NATIVE_TEXT, typeof(String), false, String.Empty, 2000);
			_columnActive = CreateDataColumn(Strings.COL_ACTIVE, typeof(bool), false, true);
			_columnException = CreateDataColumn(Strings.COL_EXCEPTION, typeof(bool), false, false);

			Columns.AddRange(new DataColumn[] { _columnExampleId, _columnRuleId, _columnPosition, _columnTargetText, _columnNativeText, _columnActive, _columnException });

			_constraintPkExamples = new UniqueConstraint(Strings.PK_EXAMPLES, _columnExampleId, true);
			Constraints.Add(_constraintPkExamples);
		}

		protected override DbDataAdapter CreateAdapter(DbConnection c)
		{
			SqlCeConnection conn = (SqlCeConnection)c;
			SqlCeDataAdapter adapter = new SqlCeDataAdapter();
			adapter.FillLoadOption = LoadOption.PreserveChanges;

			SqlCeCommand commandSelect = new SqlCeCommand();
			commandSelect.Connection = conn;
			commandSelect.CommandText = Strings.SELECT_EXAMPLES;
			adapter.SelectCommand = commandSelect;
 
			SqlCeCommand commandInsert = new SqlCeCommand();
			commandInsert.Connection = conn;
			commandInsert.CommandText = Strings.INSERT_EXAMPLES;
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_EXAMPLE_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_EXAMPLE_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_RULE_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_RULE_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_POSITION, SqlDbType.SmallInt, 0, Strings.COL_POSITION));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_TARGET_TEXT, SqlDbType.NVarChar, 2000, Strings.COL_TARGET_TEXT));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_NATIVE_TEXT, SqlDbType.NVarChar, 2000, Strings.COL_NATIVE_TEXT));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_ACTIVE, SqlDbType.Bit, 0, Strings.COL_ACTIVE));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_EXCEPTION, SqlDbType.Bit, 0, Strings.COL_EXCEPTION));
			adapter.InsertCommand = commandInsert;

			SqlCeCommand commandUpdate = new SqlCeCommand();
			commandUpdate.Connection = conn;
			commandUpdate.CommandText = Strings.UPDATE_EXAMPLES;
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_RULE_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_RULE_ID));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_POSITION, SqlDbType.SmallInt, 0, Strings.COL_POSITION));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_TARGET_TEXT, SqlDbType.NVarChar, 2000, Strings.COL_TARGET_TEXT));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_NATIVE_TEXT, SqlDbType.NVarChar, 2000, Strings.COL_NATIVE_TEXT));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_ACTIVE, SqlDbType.Bit, 0, Strings.COL_ACTIVE));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_EXCEPTION, SqlDbType.Bit, 0, Strings.COL_EXCEPTION));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_EXAMPLE_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_EXAMPLE_ID)).SourceVersion = DataRowVersion.Original;
			adapter.UpdateCommand = commandUpdate;

			SqlCeCommand commandDelete = new SqlCeCommand();
			commandDelete.Connection = conn;
			commandDelete.CommandText = Strings.DELETE_EXAMPLES;
			commandDelete.Parameters.Add(new SqlCeParameter(Strings.COL_EXAMPLE_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_EXAMPLE_ID)).SourceVersion = DataRowVersion.Original;
			adapter.DeleteCommand = commandDelete;

			return adapter;
		}
	}
}
