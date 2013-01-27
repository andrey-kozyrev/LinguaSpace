using System;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using LinguaSpace.Common.Data;
using LinguaSpace.Words.Resources;

namespace LinguaSpace.Words.Data
{
	public class VocabulariesDataTable : DataTableBase<DataRowBase>
	{
		protected DataColumn columnVocabularyId;
		protected DataColumn columnName;
		protected DataColumn columnDescription;
		protected DataColumn columnTargetLang;
		protected DataColumn columnNativeLang;

		protected UniqueConstraint constraintPkVocabularies;

		public VocabulariesDataTable()
			: base(Strings.VOCABULARIES)
		{
			this.columnVocabularyId = CreateDataColumn(Strings.VOCABULARY_ID, typeof(Guid), true, Guid.Empty);
			this.columnName = CreateDataColumn(Strings.NAME, typeof(String), false, String.Empty, 100);
			this.columnDescription = CreateDataColumn(Strings.DESCRIPTION, typeof(String), false, String.Empty, 1000);
			this.columnTargetLang = CreateDataColumn(Strings.TARGET_LANG, typeof(String), false, CultureInfo.CurrentCulture.Name, 8);
			this.columnNativeLang = CreateDataColumn(Strings.NATIVE_LANG, typeof(String), false, CultureInfo.CurrentCulture.Name, 8);

			this.Columns.AddRange(new DataColumn[] { this.columnVocabularyId, this.columnName, this.columnDescription, this.columnTargetLang, this.columnNativeLang });

			this.constraintPkVocabularies = new UniqueConstraint(Strings.PK_VOCABULARIES, this.columnVocabularyId, true);
			this.Constraints.Add(this.constraintPkVocabularies);

			this.DefaultView.AllowDelete = false;
		}

		protected override DbDataAdapter CreateAdapter(DbConnection c)
		{
			SqlCeConnection conn = (SqlCeConnection)c;
			SqlCeDataAdapter adapter = new SqlCeDataAdapter();
			adapter.FillLoadOption = LoadOption.PreserveChanges;

			SqlCeCommand commandSelect = new SqlCeCommand();
			commandSelect.Connection = conn;
			commandSelect.CommandText = "SELECT VocabularyId, Name, Description, TargetLang, NativeLang FROM Vocabularies";
			adapter.SelectCommand = commandSelect;

			SqlCeCommand commandInsert = new SqlCeCommand();
			commandInsert.Connection = conn;
			commandInsert.CommandText = "INSERT INTO Vocabularies (VocabularyId, Name, Description, TargetLang, NativeLang, SyncMod) VALUES (@VocabularyId, @Name, @Description, @TargetLang, @NativeLang, @SyncMod)";
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.VOCABULARY_ID, SqlDbType.UniqueIdentifier, 0, Strings.VOCABULARY_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.NAME, SqlDbType.NVarChar, 100, Strings.NAME));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.DESCRIPTION, SqlDbType.NVarChar, 1000, Strings.DESCRIPTION));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.TARGET_LANG, SqlDbType.NVarChar, 8, Strings.TARGET_LANG));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.NATIVE_LANG, SqlDbType.NVarChar, 8, Strings.NATIVE_LANG));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.InsertCommand = commandInsert;

			SqlCeCommand commandUpdate = new SqlCeCommand();
			commandUpdate.Connection = conn;
			commandUpdate.CommandText = "UPDATE Vocabularies SET Name = @Name, Description = @Description, TargetLang = @TargetLang, NativeLang = @NativeLang, SyncMod = @SyncMod WHERE VocabularyId = @VocabularyId";
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.NAME, SqlDbType.NVarChar, 100, Strings.NAME));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.DESCRIPTION, SqlDbType.NVarChar, 1000, Strings.DESCRIPTION));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.TARGET_LANG, SqlDbType.NVarChar, 8, Strings.TARGET_LANG));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.NATIVE_LANG, SqlDbType.NVarChar, 8, Strings.NATIVE_LANG));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.VOCABULARY_ID, SqlDbType.UniqueIdentifier, 0, Strings.VOCABULARY_ID)).SourceVersion = DataRowVersion.Original;
			adapter.UpdateCommand = commandUpdate;
			return adapter;
		}
	}

	public class TypesDataTable : DataTableBase<DataRowBase>
	{
		protected DataColumn columnTypeId;
		protected DataColumn columnPosition;
		protected DataColumn columnType;

		protected UniqueConstraint constraintPkTypes;

		public TypesDataTable()
			: base(Strings.TYPES)
		{
			this.columnTypeId = CreateDataColumn(Strings.TYPE_ID, typeof(Guid), true, Guid.Empty);
			this.columnPosition = CreateDataColumn(Strings.POSITION, typeof(Int16), false, 0);
			this.columnType = CreateDataColumn(Strings.TYPE, typeof(String), false, String.Empty, 100);

			this.Columns.AddRange(new DataColumn[] { this.columnTypeId, this.columnPosition, this.columnType });

			this.constraintPkTypes = new UniqueConstraint(Strings.PK_TYPES, this.columnTypeId, true);
			this.Constraints.Add(this.constraintPkTypes);
		}

		protected override DbDataAdapter CreateAdapter(DbConnection c)
		{
			SqlCeConnection conn = (SqlCeConnection)c;
			SqlCeDataAdapter adapter = new SqlCeDataAdapter();
			adapter.FillLoadOption = LoadOption.PreserveChanges;

			SqlCeCommand commandSelect = new SqlCeCommand();
			commandSelect.Connection = conn;
			commandSelect.CommandText = "SELECT TypeId, Position, Type FROM Types WHERE SyncLive = 1 ORDER BY Position";
			adapter.SelectCommand = commandSelect;

			SqlCeCommand commandInsert = new SqlCeCommand();
			commandInsert.Connection = conn;
			commandInsert.CommandText = "INSERT INTO Types (TypeId, Position, Type, SyncMod) VALUES (@TypeId, @Position, @Type, @SyncMod)";
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.TYPE_ID, SqlDbType.UniqueIdentifier, 0, Strings.TYPE_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.POSITION, SqlDbType.SmallInt, 0, Strings.POSITION));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.TYPE, SqlDbType.NVarChar, 100, Strings.TYPE));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.InsertCommand = commandInsert;

			SqlCeCommand commandUpdate = new SqlCeCommand();
			commandUpdate.Connection = conn;
			commandUpdate.CommandText = "UPDATE Types SET Type = @Type, Position = @Position, SyncMod = @SyncMod WHERE TypeId = @TypeId";
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.TYPE_ID, SqlDbType.UniqueIdentifier, 0, Strings.TYPE_ID)).SourceVersion = DataRowVersion.Original;
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.TYPE, SqlDbType.NVarChar, 100, Strings.TYPE));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.POSITION, SqlDbType.SmallInt, 0, Strings.POSITION));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.UpdateCommand = commandUpdate;

			SqlCeCommand commandDelete = new SqlCeCommand();
			commandDelete.Connection = conn;
			commandDelete.CommandText = "UPDATE Types SET SyncMod = @SyncMod, SyncLive = 0 WHERE TypeId = @TypeId";
			commandDelete.Parameters.Add(new SqlCeParameter(Strings.TYPE_ID, SqlDbType.UniqueIdentifier, 0, Strings.TYPE_ID)).SourceVersion = DataRowVersion.Original;
			commandDelete.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.DeleteCommand = commandDelete;

			return adapter;
		}
	}

	public class WordsDataTable : DataTableBase<DataRowBase>
	{
		protected DataColumn columnWordId;
		protected DataColumn columnTypeId;
		protected DataColumn columnPrefix;
		protected DataColumn columnWord;

		protected DataColumn columnType;

		protected UniqueConstraint constraintPkWords;

		public WordsDataTable()
			: base(Strings.WORDS)
		{
			this.columnWordId = CreateDataColumn(Strings.WORD_ID, typeof(Guid), true, Guid.Empty);
			this.columnTypeId = CreateDataColumn(Strings.TYPE_ID, typeof(Guid), false, DBNull.Value, true);
			this.columnPrefix = CreateDataColumn(Strings.PREFIX, typeof(String), false, String.Empty, 20);
			this.columnWord = CreateDataColumn(Strings.WORD, typeof(String), false, String.Empty, 200);

			this.Columns.AddRange(new DataColumn[] { this.columnWordId, this.columnTypeId, this.columnPrefix, this.columnWord });

			this.constraintPkWords = new UniqueConstraint(Strings.PK_WORDS, this.columnWordId, true);

			this.Constraints.Add(this.constraintPkWords);
		}

		protected override DbDataAdapter CreateAdapter(DbConnection c)
		{
			SqlCeConnection conn = (SqlCeConnection)c;
			SqlCeDataAdapter adapter = new SqlCeDataAdapter();

			SqlCeCommand commandSelect = new SqlCeCommand();
			commandSelect.Connection = conn;
			commandSelect.CommandText = "SELECT WordId, TypeId, Prefix, Word FROM Words WHERE SyncLive = 1 ORDER BY Word, TypeId";
			adapter.SelectCommand = commandSelect;

			SqlCeCommand commandInsert = new SqlCeCommand();
			commandInsert.Connection = conn;
			commandInsert.CommandText = "INSERT INTO Words (WordId, TypeId, Prefix, Word, SyncMod) VALUES (@WordId, @TypeId, @Prefix, @Word, @SyncMod)";
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.WORD_ID, SqlDbType.UniqueIdentifier, 0, Strings.WORD_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.TYPE_ID, SqlDbType.UniqueIdentifier, 0, Strings.TYPE_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.PREFIX, SqlDbType.NVarChar, 20, Strings.PREFIX));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.WORD, SqlDbType.NVarChar, 200, Strings.WORD));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.InsertCommand = commandInsert;

			SqlCeCommand commandUpdate = new SqlCeCommand();
			commandUpdate.Connection = conn;
			commandUpdate.CommandText = "UPDATE Words SET TypeId = @TypeId, Prefix = @Prefix, Word = @Word, SyncMod = @SyncMod WHERE WordId = @WordId";
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.WORD_ID, SqlDbType.UniqueIdentifier, 0, Strings.WORD_ID)).SourceVersion = DataRowVersion.Original;
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.TYPE_ID, SqlDbType.UniqueIdentifier, 0, Strings.TYPE_ID));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.PREFIX, SqlDbType.NVarChar, 20, Strings.PREFIX));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.WORD, SqlDbType.NVarChar, 200, Strings.WORD));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.UpdateCommand = commandUpdate;

			SqlCeCommand commandDelete = new SqlCeCommand();
			commandDelete.Connection = conn;
			commandDelete.CommandText = "UPDATE Words SET SyncMod = @SyncMod, SyncLive = 0 WHERE WordId = @WordId";
			commandDelete.Parameters.Add(new SqlCeParameter(Strings.WORD_ID, SqlDbType.UniqueIdentifier, 0, Strings.WORD_ID)).SourceVersion = DataRowVersion.Original;
			commandDelete.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.DeleteCommand = commandDelete;

			return adapter;
		}
	}

	public class MeaningsDataTable : DataTableBase<DataRowBase>
	{
		protected DataColumn columnMeaningId;
		protected DataColumn columnPosition;
		protected DataColumn columnPrefix;
		protected DataColumn columnWordId;
		protected DataColumn columnPostfix;
		protected DataColumn columnDefinition;
		protected DataColumn columnExample;

		protected UniqueConstraint constraintPkMeanings;

		public MeaningsDataTable()
			: base(Strings.MEANINGS)
		{
			this.columnMeaningId = CreateDataColumn(Strings.MEANING_ID, typeof(Guid), true, Guid.Empty);
			this.columnPosition = CreateDataColumn(Strings.POSITION, typeof(Int16), false, 0);
			this.columnPrefix = CreateDataColumn(Strings.PREFIX, typeof(String), false, String.Empty, 20);
			this.columnWordId = CreateDataColumn(Strings.WORD_ID, typeof(Guid), false, DBNull.Value, true);
			this.columnPostfix = CreateDataColumn(Strings.POSTFIX, typeof(String), false, String.Empty, 20);
			this.columnDefinition = CreateDataColumn(Strings.DEFINITION, typeof(String), false, String.Empty, 1000);
			this.columnExample = CreateDataColumn(Strings.EXAMPLE, typeof(String), false, String.Empty, 1000);

			this.Columns.AddRange(new DataColumn[] { this.columnMeaningId, this.columnPosition, this.columnPrefix, this.columnWordId, this.columnPostfix, this.columnDefinition, this.columnExample });

			this.constraintPkMeanings = new UniqueConstraint(Strings.PK_MEANINGS, this.columnMeaningId, true);
			this.Constraints.Add(this.constraintPkMeanings);
		}

		protected override DbDataAdapter CreateAdapter(DbConnection c)
		{
			SqlCeConnection conn = (SqlCeConnection)c;
			SqlCeDataAdapter adapter = new SqlCeDataAdapter();
			adapter.FillLoadOption = LoadOption.PreserveChanges;

			SqlCeCommand commandSelect = new SqlCeCommand();
			commandSelect.Connection = conn;
			commandSelect.CommandText = "SELECT MeaningId, Position, Prefix, WordId, Postfix, Definition, Example FROM Meanings WHERE SyncLive = 1 ORDER BY WordId, Position";
			adapter.SelectCommand = commandSelect;

			SqlCeCommand commandInsert = new SqlCeCommand();
			commandInsert.Connection = conn;
			commandInsert.CommandText = "INSERT INTO Meanings (MeaningId, Position, Prefix, WordId, Postfix, Definition, Example, SyncMod) VALUES (@MeaningId, @Position, @Prefix, @WordId, @Postfix, @Definition, @Example, @SyncMod)";
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.MEANING_ID, SqlDbType.UniqueIdentifier, 0, Strings.MEANING_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.POSITION, SqlDbType.SmallInt, 0, Strings.POSITION));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.PREFIX, SqlDbType.NVarChar, 100, Strings.PREFIX));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.WORD_ID, SqlDbType.UniqueIdentifier, 0, Strings.WORD_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.POSTFIX, SqlDbType.NVarChar, 100, Strings.POSTFIX));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.DEFINITION, SqlDbType.NVarChar, 1000, Strings.DEFINITION));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.EXAMPLE, SqlDbType.NVarChar, 1000, Strings.EXAMPLE));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.InsertCommand = commandInsert;

			SqlCeCommand commandUpdate = new SqlCeCommand();
			commandUpdate.Connection = conn;
			commandUpdate.CommandText = "UPDATE Meanings SET Position = @Position, Prefix = @Prefix, Postfix = @Postfix, Definition = @Definition, Example = @Example, SyncMod = @SyncMod WHERE MeaningId = @MeaningId";
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.MEANING_ID, SqlDbType.UniqueIdentifier, 0, Strings.MEANING_ID)).SourceVersion = DataRowVersion.Original;
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.POSITION, SqlDbType.SmallInt, 0, Strings.POSITION));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.PREFIX, SqlDbType.NVarChar, 100, Strings.PREFIX));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.POSTFIX, SqlDbType.NVarChar, 100, Strings.POSTFIX));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.DEFINITION, SqlDbType.NVarChar, 1000, Strings.DEFINITION));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.EXAMPLE, SqlDbType.NVarChar, 1000, Strings.EXAMPLE));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.UpdateCommand = commandUpdate;

			SqlCeCommand commandDelete = new SqlCeCommand();
			commandDelete.Connection = conn;
			commandDelete.CommandText = "UPDATE Meanings SET SyncMod = @SyncMod, SyncLive = 0 WHERE MeaningId = @MeaningId";
			commandDelete.Parameters.Add(new SqlCeParameter(Strings.MEANING_ID, SqlDbType.UniqueIdentifier, 0, Strings.MEANING_ID)).SourceVersion = DataRowVersion.Original;
			commandDelete.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.DeleteCommand = commandDelete;

			return adapter;
		}
	}

	public class TranslationsDataTable : DataTableBase<DataRowBase>
	{
		protected DataColumn columnTranslationId;
		protected DataColumn columnMeaningId;
		protected DataColumn columnPosition;
		protected DataColumn columnTranslation;

		protected UniqueConstraint constraintPkTranslations;

		public TranslationsDataTable()
			: base(Strings.TRANSLATIONS)
		{
			this.columnTranslationId = CreateDataColumn(Strings.TRANSLATION_ID, typeof(Guid), true, Guid.Empty);
			this.columnMeaningId = CreateDataColumn(Strings.MEANING_ID, typeof(Guid), false, Guid.Empty);
			this.columnPosition = CreateDataColumn(Strings.POSITION, typeof(Int16), false, 0);
			this.columnTranslation = CreateDataColumn(Strings.TRANSLATION, typeof(String), false, String.Empty, 200);

			this.Columns.AddRange(new DataColumn[] { this.columnTranslationId, this.columnMeaningId, this.columnPosition, this.columnTranslation });

			this.constraintPkTranslations = new UniqueConstraint(Strings.PK_TRANSLATIONS, this.columnTranslationId, true);
			this.Constraints.Add(this.constraintPkTranslations);
		}

		protected override DbDataAdapter CreateAdapter(DbConnection c)
		{
			SqlCeConnection conn = (SqlCeConnection)c;
			SqlCeDataAdapter adapter = new SqlCeDataAdapter();

			SqlCeCommand commandSelect = new SqlCeCommand();
			commandSelect.Connection = conn;
			commandSelect.CommandText = "SELECT TranslationId, MeaningId, Position, Translation FROM Translations WHERE SyncLive = 1 ORDER BY MeaningId, Position";
			adapter.SelectCommand = commandSelect;

			SqlCeCommand commandInsert = new SqlCeCommand();
			commandInsert.Connection = conn;
			commandInsert.CommandText = "INSERT INTO Translations (TranslationId, MeaningId, Position, Translation, SyncMod) VALUES (@TranslationId, @MeaningId, @Position, @Translation, @SyncMod)";
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.TRANSLATION_ID, SqlDbType.UniqueIdentifier, 0, Strings.TRANSLATION_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.MEANING_ID, SqlDbType.UniqueIdentifier, 0, Strings.MEANING_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.POSITION, SqlDbType.SmallInt, 0, Strings.POSITION));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.TRANSLATION, SqlDbType.NVarChar, 100, Strings.TRANSLATION));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.InsertCommand = commandInsert;

			SqlCeCommand commandUpdate = new SqlCeCommand();
			commandUpdate.Connection = conn;
			commandUpdate.CommandText = "UPDATE Translations SET Translation = @Translation, Position = @Position, SyncMod = @SyncMod WHERE TranslationId = @TranslationId";
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.TRANSLATION_ID, SqlDbType.UniqueIdentifier, 0, Strings.TRANSLATION_ID)).SourceVersion = DataRowVersion.Original;
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.POSITION, SqlDbType.SmallInt, 0, Strings.POSITION));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.TRANSLATION, SqlDbType.NVarChar, 100, Strings.TRANSLATION));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.UpdateCommand = commandUpdate;

			SqlCeCommand commandDelete = new SqlCeCommand();
			commandDelete.Connection = conn;
			commandDelete.CommandText = "UPDATE Translations SET SyncMod = @SyncMod, SyncLive = 0 WHERE TranslationId = @TranslationId";
			commandDelete.Parameters.Add(new SqlCeParameter(Strings.TRANSLATION_ID, SqlDbType.UniqueIdentifier, 0, Strings.TRANSLATION_ID)).SourceVersion = DataRowVersion.Original;
			commandDelete.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.DeleteCommand = commandDelete;
			
			return adapter;
		}
	}

	public abstract class RelationsDataTable : DataTableBase<DataRowBase>
	{
		protected DataColumn columnRelationId;
		protected DataColumn columnMeaningId;
		protected DataColumn columnPosition;
		protected DataColumn columnWordId;

		protected UniqueConstraint constraintPkRelations;
		
		protected String relation;

		public RelationsDataTable(String name, String relation)
			: base(name)
		{
			Debug.Assert(relation != null);
			this.relation = relation;
		
			this.columnRelationId = CreateDataColumn(Strings.RELATION_ID, typeof(Guid), true, Guid.Empty);
			this.columnMeaningId = CreateDataColumn(Strings.MEANING_ID, typeof(Guid), false, DBNull.Value, true);
			this.columnPosition = CreateDataColumn(Strings.POSITION, typeof(Int16), false, 0);
			this.columnWordId = CreateDataColumn(Strings.WORD_ID, typeof(Guid), false, DBNull.Value, true);

			this.Columns.AddRange(new DataColumn[] { this.columnRelationId, this.columnMeaningId, this.columnPosition, this.columnWordId });

			this.constraintPkRelations = new UniqueConstraint(Strings.PK_RELATIONS, this.columnRelationId, true);
			this.Constraints.Add(this.constraintPkRelations);
		}

		protected override DbDataAdapter CreateAdapter(DbConnection c)
		{
			SqlCeConnection conn = (SqlCeConnection)c;
			SqlCeDataAdapter adapter = new SqlCeDataAdapter();

			SqlCeCommand commandSelect = new SqlCeCommand();
			commandSelect.Connection = conn;
			commandSelect.CommandText = "SELECT RelationId, MeaningId, Position, WordId FROM Relations WHERE SyncLive = 1 AND Relation = @Relation ORDER BY MeaningId, Position";
			commandSelect.Parameters.Add(new SqlCeParameter(Strings.RELATION, this.relation));
			adapter.SelectCommand = commandSelect;

			SqlCeCommand commandInsert = new SqlCeCommand();
			commandInsert.Connection = conn;
			commandInsert.CommandText = "INSERT INTO Relations (RelationId, MeaningId, Relation, Position, WordId, SyncMod) VALUES (@RelationId, @MeaningId, @Relation, @Position, @WordId, @SyncMod)";
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.RELATION_ID, SqlDbType.UniqueIdentifier, 0, Strings.RELATION_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.MEANING_ID, SqlDbType.UniqueIdentifier, 0, Strings.MEANING_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.RELATION, this.relation));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.POSITION, SqlDbType.SmallInt, 0, Strings.POSITION));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.WORD_ID, SqlDbType.UniqueIdentifier, 0, Strings.WORD_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.InsertCommand = commandInsert;

			SqlCeCommand commandUpdate = new SqlCeCommand();
			commandUpdate.Connection = conn;
			commandUpdate.CommandText = "UPDATE Relations SET WordId = @WordId, Position = @Position, SyncMod = @SyncMod WHERE RelationId = @RelationId";
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.RELATION_ID, SqlDbType.UniqueIdentifier, 0, Strings.RELATION_ID)).SourceVersion = DataRowVersion.Original;
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.POSITION, SqlDbType.SmallInt, 0, Strings.POSITION));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.WORD_ID, SqlDbType.UniqueIdentifier, 0, Strings.WORD_ID));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.UpdateCommand = commandUpdate;

			SqlCeCommand commandDelete = new SqlCeCommand();
			commandDelete.Connection = conn;
			commandDelete.CommandText = "UPDATE Relations SET SyncMod = @SyncMod, SyncLive = 0 WHERE RelationId = @RelationId";
			commandDelete.Parameters.Add(new SqlCeParameter(Strings.RELATION_ID, SqlDbType.UniqueIdentifier, 0, Strings.RELATION_ID)).SourceVersion = DataRowVersion.Original;
			commandDelete.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.DeleteCommand = commandDelete;

			return adapter;
		}
	}

	public class SynonymsDataTable : RelationsDataTable
	{
		public SynonymsDataTable()
			: base(Strings.SYNONYMS, "S")
		{
		}
	}

	public class AntonymsDataTable : RelationsDataTable
	{
		public AntonymsDataTable()
			: base(Strings.ANTONYMS, "A")
		{
		}
	}

	public class ProfilesDataTable : DataTableBase<DataRowBase>
	{
		protected DataColumn columnProfileId;
		protected DataColumn columnName;
		protected DataColumn columnDescription;
		protected DataColumn columnDefaultVocabularyPath;
		protected DataColumn columnSleep;
		protected DataColumn columnBeep;
		
		protected UniqueConstraint constraintPkProfiles;

		public ProfilesDataTable()
			: base(Strings.PROFILES)
		{
			this.columnProfileId = CreateDataColumn(Strings.PROFILE_ID, typeof(Guid), true, Guid.Empty);
			this.columnName = CreateDataColumn(Strings.NAME, typeof(String), false, String.Empty, 100);
			this.columnDescription = CreateDataColumn(Strings.DESCRIPTION, typeof(String), false, String.Empty, 1000);
			this.columnDefaultVocabularyPath = CreateDataColumn(Strings.DEFAULT_VOCABULARY_PATH, typeof(String), false, String.Empty, 1000);
			this.columnSleep = CreateDataColumn(Strings.SLEEP, typeof(int), false, 10);
			this.columnBeep = CreateDataColumn(Strings.BEEP, typeof(bool), false, true);

			this.Columns.AddRange(new DataColumn[] { this.columnProfileId, this.columnName, this.columnDescription, this.columnDefaultVocabularyPath, this.columnSleep, this.columnBeep });

			this.constraintPkProfiles = new UniqueConstraint(Strings.PK_PROFILES, this.columnProfileId, true);
			this.Constraints.Add(this.constraintPkProfiles);
		}

		protected override DbDataAdapter CreateAdapter(DbConnection c)
		{
			SqlCeConnection conn = (SqlCeConnection)c;
			SqlCeDataAdapter adapter = new SqlCeDataAdapter();

			SqlCeCommand commandSelect = new SqlCeCommand();
			commandSelect.Connection = conn;
			commandSelect.CommandText = "SELECT ProfileId, Name, Description, DefaultVocabularyPath, Sleep, Beep FROM Profiles";
			adapter.SelectCommand = commandSelect;

			SqlCeCommand commandInsert = new SqlCeCommand();
			commandInsert.Connection = conn;
			commandInsert.CommandText = "INSERT INTO Profiles (ProfileId, Name, Description, DefaultVocabularyPath, Sleep, Beep, SyncMod) VALUES (@ProfileId, @Name, @Description, @DefaultVocabularyPath, @Sleep, @Beep, @SyncMod)";
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.PROFILE_ID, SqlDbType.UniqueIdentifier, 0, Strings.PROFILE_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.NAME, SqlDbType.NVarChar, 100, Strings.NAME));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.DESCRIPTION, SqlDbType.NVarChar, 1000, Strings.DESCRIPTION));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.DEFAULT_VOCABULARY_PATH, SqlDbType.NVarChar, 1000, Strings.DEFAULT_VOCABULARY_PATH));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.SLEEP, 10));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.BEEP, true));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.InsertCommand = commandInsert;

			SqlCeCommand commandUpdate = new SqlCeCommand();
			commandUpdate.Connection = conn;
			commandUpdate.CommandText = "UPDATE Profiles SET Name = @Name, Description = @Description, DefaultVocabularyPath = @DefaultVocabularyPath, Sleep = @Sleep, Beep = @Beep, SyncMod = @SyncMod WHERE ProfileId = @ProfileId";
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.PROFILE_ID, SqlDbType.UniqueIdentifier, 0, Strings.PROFILE_ID)).SourceVersion = DataRowVersion.Original;
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.NAME, SqlDbType.NVarChar, 100, Strings.NAME));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.DESCRIPTION, SqlDbType.NVarChar, 1000, Strings.DESCRIPTION));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.DEFAULT_VOCABULARY_PATH, SqlDbType.NVarChar, 1000, Strings.DEFAULT_VOCABULARY_PATH));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.SLEEP, SqlDbType.Int, 0, Strings.SLEEP));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.BEEP, SqlDbType.Bit, 0, Strings.BEEP));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.UpdateCommand = commandUpdate;
			
			return adapter;
		}
	}

	public class ActionsDataTable : DataTableBase<DataRowBase>
	{
		protected DataColumn columnActionId;
		protected DataColumn columnAction;
		protected DataColumn columnWeight;

		protected UniqueConstraint constraintPkActions;

		public ActionsDataTable()
			: base(Strings.ACTIONS)
		{
			this.columnActionId = CreateDataColumn(Strings.ACTION_ID, typeof(String), true, String.Empty, 1);
			this.columnAction = CreateDataColumn(Strings.ACTION, typeof(String), true, String.Empty, 100);
			this.columnWeight = CreateDataColumn(Strings.WEIGHT, typeof(int), false, 0);

			this.Columns.AddRange(new DataColumn[] { this.columnActionId, this.columnAction, this.columnWeight });

			this.constraintPkActions = new UniqueConstraint(Strings.PK_ACTIONS, this.columnActionId, true);
			this.Constraints.Add(this.constraintPkActions);
		}

		protected override DbDataAdapter CreateAdapter(DbConnection c)
		{
			SqlCeConnection conn = (SqlCeConnection)c;
			SqlCeDataAdapter adapter = new SqlCeDataAdapter();

			SqlCeCommand commandSelect = new SqlCeCommand();
			commandSelect.Connection = conn;
			commandSelect.CommandText = "SELECT ActionId, Action, Weight FROM Actions";
			adapter.SelectCommand = commandSelect;

			SqlCeCommand commandInsert = new SqlCeCommand();
			commandInsert.Connection = conn;
			commandInsert.CommandText = "INSERT INTO Actions (ActionId, Action, Weight, SyncMod) VALUES (@ActionId, @Action, @Weight, @SyncMod)";
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.ACTION_ID, SqlDbType.NVarChar, 1, Strings.ACTION_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.ACTION, SqlDbType.NVarChar, 100, Strings.ACTION));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.WEIGHT, SqlDbType.Int, 0, Strings.WEIGHT));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.InsertCommand = commandInsert;


			SqlCeCommand commandUpdate = new SqlCeCommand();
			commandUpdate.Connection = conn;
			commandUpdate.CommandText = "UPDATE Actions SET Weight = @Weight, SyncMod = @SyncMod WHERE ActionId = @ActionId";
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.ACTION_ID, SqlDbType.NVarChar, 1, Strings.ACTION_ID)).SourceVersion = DataRowVersion.Original;
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.WEIGHT, SqlDbType.Int, 0, Strings.WEIGHT));
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, DateTime.UtcNow));
			adapter.UpdateCommand = commandUpdate;
			
			return adapter;
		}
	}

	public class HistoryDataTable : DataTableBase<DataRowBase>
	{
		protected DataColumn columnHistoryId;
		protected DataColumn columnVocabularyId;
		protected DataColumn columnMeaningId;
		protected DataColumn columnSyncMod;

		protected UniqueConstraint constraintPkHistory;

		public HistoryDataTable()
			: base(Strings.HISTORY)
		{
			this.columnHistoryId = CreateDataColumn(Strings.HISTORY_ID, typeof(Guid), true, Guid.Empty);
			this.columnVocabularyId = CreateDataColumn(Strings.VOCABULARY_ID, typeof(Guid), true, Guid.Empty);
			this.columnMeaningId = CreateDataColumn(Strings.MEANING_ID, typeof(Guid), true, Guid.Empty);
			this.columnSyncMod = CreateDataColumn(Strings.SYNC_MOD, typeof(DateTime), false, DateTime.UtcNow);

			this.Columns.AddRange(new DataColumn[] {this.columnHistoryId, this.columnVocabularyId, this.columnMeaningId, this.columnSyncMod });
			
			this.constraintPkHistory = new UniqueConstraint(Strings.PK_HISTORY, this.columnHistoryId, true);
			this.Constraints.Add(this.constraintPkHistory);
		}

		protected override DbDataAdapter CreateAdapter(DbConnection c)
		{
			SqlCeConnection conn = (SqlCeConnection)c;
			SqlCeDataAdapter adapter = new SqlCeDataAdapter();

			SqlCeCommand commandSelect = new SqlCeCommand();
			commandSelect.Connection = conn;
			commandSelect.CommandText = "SELECT HistoryId, VocabularyId, MeaningId, SyncMod FROM History WHERE ActionId = 'M' ORDER BY MeaningId";
			adapter.SelectCommand = commandSelect;

			SqlCeCommand commandInsert = new SqlCeCommand();
			commandInsert.Connection = conn;
			commandInsert.CommandText = "INSERT INTO History (HistoryId, VocabularyId, MeaningId, ActionId, SyncMod) VALUES (@HistoryId, @VocabularyId, @MeaningId, 'M', @SyncMod)";
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.HISTORY_ID, SqlDbType.UniqueIdentifier, 0, Strings.HISTORY_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.VOCABULARY_ID, SqlDbType.UniqueIdentifier, 0, Strings.VOCABULARY_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.MEANING_ID, SqlDbType.UniqueIdentifier, 0, Strings.MEANING_ID));
			commandInsert.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, SqlDbType.DateTime, 0, Strings.SYNC_MOD));
			adapter.InsertCommand = commandInsert;

			SqlCeCommand commandUpdate = new SqlCeCommand();
			commandUpdate.Connection = conn;
			commandUpdate.CommandText = "UPDATE History SET SyncMod = @SyncMod WHERE HistoryId = @HistoryId";
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.HISTORY_ID, SqlDbType.UniqueIdentifier, 0, Strings.HISTORY_ID)).SourceVersion = DataRowVersion.Original;
			commandUpdate.Parameters.Add(new SqlCeParameter(Strings.SYNC_MOD, SqlDbType.DateTime, 0, Strings.SYNC_MOD));
			adapter.UpdateCommand = commandUpdate;

			return adapter;
		}
	}
}
