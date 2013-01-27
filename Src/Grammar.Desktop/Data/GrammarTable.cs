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
	class GrammarTable : DataTableBase<DataRowBase>
	{
		protected DataColumn _columnGrammarId;
		protected DataColumn _columnTitle;
		protected DataColumn _columnComment;
		protected DataColumn _columnTargetLang;
		protected DataColumn _columnNativeLang;
        protected DataColumn _columnShuffle;
        protected DataColumn _columnShowRule;

		protected UniqueConstraint _constraintPkGrammar;

		public GrammarTable()
			: base(Strings.TABLE_GRAMMAR)
		{
			_columnGrammarId = CreateDataColumn(Strings.COL_GRAMMAR_ID, typeof(Guid), true, Guid.Empty);
			_columnTitle = CreateDataColumn(Strings.COL_TITLE, typeof(String), false, String.Empty, 100);
			_columnComment = CreateDataColumn(Strings.COL_COMMENT, typeof(String), false, String.Empty, 1000);
			_columnTargetLang = CreateDataColumn(Strings.COL_TARGET_LANG, typeof(String), false, CultureInfo.CurrentCulture.Name, 8);
			_columnNativeLang = CreateDataColumn(Strings.COL_NATIVE_LANG, typeof(String), false, CultureInfo.CurrentCulture.Name, 8);
            _columnShuffle = CreateDataColumn(Strings.COL_SHUFFLE, typeof(bool), false, false);
            _columnShowRule = CreateDataColumn(Strings.COL_SHOWRULE, typeof(bool), false, true);

			Columns.AddRange(new DataColumn[] { _columnGrammarId, _columnTitle, _columnComment, _columnTargetLang, _columnNativeLang, _columnShuffle, _columnShowRule });

			_constraintPkGrammar = new UniqueConstraint(Strings.PK_GRAMMAR, _columnGrammarId, true);
			Constraints.Add(_constraintPkGrammar);

			DefaultView.AllowDelete = false;
		}

		protected override DbDataAdapter CreateAdapter(DbConnection c)
		{
			SqlCeConnection conn = (SqlCeConnection)c;
			SqlCeDataAdapter adapter = new SqlCeDataAdapter();
			adapter.FillLoadOption = LoadOption.PreserveChanges;

			SqlCeCommand commandSelect = new SqlCeCommand();
			commandSelect.Connection = conn;
			commandSelect.CommandText = Strings.SELECT_GRAMMAR;
			adapter.SelectCommand = commandSelect;

			SqlCeCommand commandInsert = new SqlCeCommand();
			commandInsert.Connection = conn;
			commandInsert.CommandText = Strings.INSERT_GRAMMAR;
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_GRAMMAR_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_GRAMMAR_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_TITLE, SqlDbType.NVarChar, 100, Strings.COL_TITLE));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_COMMENT, SqlDbType.NVarChar, 1000, Strings.COL_COMMENT));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_TARGET_LANG, SqlDbType.NVarChar, 8, Strings.COL_TARGET_LANG));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_NATIVE_LANG, SqlDbType.NVarChar, 8, Strings.COL_NATIVE_LANG));
            commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_SHUFFLE, SqlDbType.Bit, 1, Strings.COL_SHUFFLE));
            commandInsert.Parameters.Add(new SqlCeParameter(Strings.COL_SHOWRULE, SqlDbType.Bit, 1, Strings.COL_SHOWRULE));
			adapter.InsertCommand = commandInsert;

			SqlCeCommand commandUpdate = new SqlCeCommand();
			commandUpdate.Connection = conn;
			commandUpdate.CommandText = Strings.UPDATE_GRAMMAR;
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_TITLE, SqlDbType.NVarChar, 100, Strings.COL_TITLE));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_COMMENT, SqlDbType.NVarChar, 1000, Strings.COL_COMMENT));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_TARGET_LANG, SqlDbType.NVarChar, 8, Strings.COL_TARGET_LANG));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_NATIVE_LANG, SqlDbType.NVarChar, 8, Strings.COL_NATIVE_LANG));

			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_GRAMMAR_ID, SqlDbType.UniqueIdentifier, 0, Strings.COL_GRAMMAR_ID)).SourceVersion = DataRowVersion.Original;
            commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_SHUFFLE, SqlDbType.Bit, 1, Strings.COL_SHUFFLE));
            commandUpdate.Parameters.Add(new SqlCeParameter(Strings.COL_SHOWRULE, SqlDbType.Bit, 1, Strings.COL_SHOWRULE));

			adapter.UpdateCommand = commandUpdate;

			return adapter;
		}
	}
}
