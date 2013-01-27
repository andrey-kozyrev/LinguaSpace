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
	public class TopicsTable : DataTableBase<DataRowBase>
	{
		protected DataColumn _columnTopicId;
		protected DataColumn _columnParentTopicId;
		protected DataColumn _columnPosition;
		protected DataColumn _columnTitle;
        protected DataColumn _columnExpanded;

		protected UniqueConstraint _constraintPkTopics;

		public TopicsTable()
			: base(Strings.TABLE_TOPICS)
		{
			_columnTopicId = CreateDataColumn(Strings.COL_TOPIC_ID, typeof(Guid), true, Guid.Empty);
			_columnParentTopicId = CreateDataColumn(Strings.COL_PARENT_TOPIC_ID, typeof(Guid), true, DBNull.Value, true);
			_columnPosition = CreateDataColumn(Strings.COL_POSITION, typeof(Int16), false, 0);
			_columnTitle = CreateDataColumn(Strings.COL_TITLE, typeof(String), false, String.Empty, 200);
            _columnExpanded = CreateDataColumn(Strings.COL_EXPANDED, typeof(bool), false, false);

			Columns.AddRange(new DataColumn[] { _columnTopicId, _columnParentTopicId, _columnPosition, _columnTitle, _columnExpanded });

			_constraintPkTopics = new UniqueConstraint(Strings.PK_TOPICS, _columnTopicId, true);
			Constraints.Add(_constraintPkTopics);
		}

		protected override DbDataAdapter CreateAdapter(DbConnection c)
		{
			SqlCeConnection conn = (SqlCeConnection)c;
			SqlCeDataAdapter adapter = new SqlCeDataAdapter();
			adapter.FillLoadOption = LoadOption.PreserveChanges;

			SqlCeCommand commandSelect = new SqlCeCommand();
			commandSelect.Connection = conn;
			commandSelect.CommandText = Strings.SELECT_TOPICS;
			adapter.SelectCommand = commandSelect;

			SqlCeCommand commandInsert = new SqlCeCommand();
			commandInsert.Connection = conn;
			commandInsert.CommandText = Strings.INSERT_TOPICS;
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_TOPIC_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_TOPIC_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_PARENT_TOPIC_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_PARENT_TOPIC_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_POSITION, SqlDbType.SmallInt, 0, Strings.COL_POSITION));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_TITLE, SqlDbType.NVarChar, 200, Strings.COL_TITLE));
            commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_EXPANDED, SqlDbType.Bit, 1, Strings.COL_EXPANDED));
			adapter.InsertCommand = commandInsert;

			SqlCeCommand commandUpdate = new SqlCeCommand();
			commandUpdate.Connection = conn;
			commandUpdate.CommandText = Strings.UPDATE_TOPICS;
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_PARENT_TOPIC_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_PARENT_TOPIC_ID));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_POSITION, SqlDbType.SmallInt, 0, Strings.COL_POSITION));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_TITLE, SqlDbType.NVarChar, 200, Strings.COL_TITLE));
            commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_EXPANDED, SqlDbType.Bit, 1, Strings.COL_EXPANDED));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_TOPIC_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_TOPIC_ID)).SourceVersion = DataRowVersion.Original;
			adapter.UpdateCommand = commandUpdate;

			SqlCeCommand commandDelete = new SqlCeCommand();
			commandDelete.Connection = conn;
			commandDelete.CommandText = Strings.DELETE_TOPICS;
			commandDelete.Parameters.Add(new SqlCeParameter(Strings.COL_TOPIC_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_TOPIC_ID)).SourceVersion = DataRowVersion.Original;
			adapter.DeleteCommand = commandDelete;
			
			return adapter;
		}
	}
}
