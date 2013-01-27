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
	public class RulesTable : DataTableBase<DataRowBase>
	{
		protected DataColumn _columnRuleId;
		protected DataColumn _columnTopicId;
		protected DataColumn _columnPosition;
		protected DataColumn _columnComment;
		protected DataColumn _columnActive;

		protected UniqueConstraint _constraintPkRules;

		public RulesTable()
			: base(Strings.TABLE_RULES)
		{
			_columnRuleId = CreateDataColumn(Strings.COL_RULE_ID, typeof(Guid), true, Guid.Empty);
			_columnTopicId = CreateDataColumn(Strings.COL_TOPIC_ID, typeof(Guid), true, Guid.Empty);
			_columnPosition = CreateDataColumn(Strings.COL_POSITION, typeof(Int16), false, 0);
			_columnComment = CreateDataColumn(Strings.COL_COMMENT, typeof(String), false, String.Empty, 2000);
			_columnActive = CreateDataColumn(Strings.COL_ACTIVE, typeof(bool), false, true);

			Columns.AddRange(new DataColumn[] { _columnRuleId, _columnTopicId, _columnPosition, _columnComment, _columnActive });

			_constraintPkRules = new UniqueConstraint(Strings.PK_RULES, _columnRuleId, true);
			Constraints.Add(_constraintPkRules);
		}

		protected override DbDataAdapter CreateAdapter(DbConnection c)
		{
			SqlCeConnection conn = (SqlCeConnection)c;
			SqlCeDataAdapter adapter = new SqlCeDataAdapter();
			adapter.FillLoadOption = LoadOption.PreserveChanges;

			SqlCeCommand commandSelect = new SqlCeCommand();
			commandSelect.Connection = conn;
			commandSelect.CommandText = Strings.SELECT_RULES;
			adapter.SelectCommand = commandSelect;

			SqlCeCommand commandInsert = new SqlCeCommand();
			commandInsert.Connection = conn;
			commandInsert.CommandText = Strings.INSERT_RULES;
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_RULE_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_RULE_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_TOPIC_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_TOPIC_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_POSITION, SqlDbType.SmallInt, 0, Strings.COL_POSITION));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_COMMENT, SqlDbType.NVarChar, 3000, Strings.COL_COMMENT));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_ACTIVE, SqlDbType.Bit, 0, Strings.COL_ACTIVE));
			adapter.InsertCommand = commandInsert;

			SqlCeCommand commandUpdate = new SqlCeCommand();
			commandUpdate.Connection = conn;
			commandUpdate.CommandText = Strings.UPDATE_RULES;
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_TOPIC_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_TOPIC_ID));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_POSITION, SqlDbType.SmallInt, 0, Strings.COL_POSITION));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_COMMENT, SqlDbType.NVarChar, 3000, Strings.COL_COMMENT));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_ACTIVE, SqlDbType.Bit, 0, Strings.COL_ACTIVE));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_RULE_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_RULE_ID)).SourceVersion = DataRowVersion.Original;
			adapter.UpdateCommand = commandUpdate;

			SqlCeCommand commandDelete = new SqlCeCommand();
			commandDelete.Connection = conn;
			commandDelete.CommandText = Strings.DELETE_RULES;
			commandDelete.Parameters.Add(new SqlCeParameter(Strings.COL_RULE_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_RULE_ID)).SourceVersion = DataRowVersion.Original;
			adapter.DeleteCommand = commandDelete;

			return adapter;
		}
	}
}
