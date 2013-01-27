using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Data;
using System.Diagnostics;
using System.Data.SqlServerCe;
using LinguaSpace.Common.Data;
using LinguaSpace.Words.Resources;

namespace LinguaSpace.Words.Data
{
	public class LinguaSpaceDataSet : TransactedDataSet
	{
		private VocabulariesDataTable tableVocabularies;
		private TypesDataTable tableTypes;
		private WordsDataTable tableWords;
		private MeaningsDataTable tableMeanings;
		private TranslationsDataTable tableTranslations;
		private SynonymsDataTable tableSynonyms;
		private AntonymsDataTable tableAntonyms;
		
		private ProfilesDataTable tableProfiles;
		private ActionsDataTable tableActions;
		private HistoryDataTable tableHistory;

		public LinguaSpaceDataSet()
		{
			// Vocabulary 
			tableVocabularies = new VocabulariesDataTable();
			tableTypes = new TypesDataTable();
			tableWords = new WordsDataTable();
			tableMeanings = new MeaningsDataTable();
			tableTranslations = new TranslationsDataTable();
			tableSynonyms = new SynonymsDataTable();
			tableAntonyms = new AntonymsDataTable();

			// Profile
			tableProfiles = new ProfilesDataTable();
			tableActions = new ActionsDataTable();
			tableHistory = new HistoryDataTable();

			this.Tables.AddRange(new DataTable[] { tableVocabularies, tableTypes, tableWords, tableMeanings, tableTranslations, tableSynonyms, tableAntonyms, tableProfiles, tableActions, tableHistory });

			tableTypes.DefaultView.Sort = Strings.POSITION;
			tableWords.DefaultView.Sort = "Word, TypeId";
			tableMeanings.DefaultView.Sort = "WordId, Position";
			tableTranslations.DefaultView.Sort = "MeaningId, Position";
			tableSynonyms.DefaultView.Sort = "MeaningId, Position";
			tableAntonyms.DefaultView.Sort = "MeaningId, Position";
			tableHistory.DefaultView.Sort = "MeaningId";

			AddForeignKey(tableTypes, tableWords, Strings.TYPE_ID, Strings.FK_WORDS_TYPES, false);
			AddForeignKey(tableWords, tableMeanings, Strings.WORD_ID, Strings.FK_MEANINGS_WORDS, true);
			AddForeignKey(tableMeanings, tableTranslations, Strings.MEANING_ID, Strings.FK_TRANSLATIONS_MEANINGS, true);
			AddForeignKey(tableMeanings, tableSynonyms, Strings.MEANING_ID, Strings.FK_SYNONYMS_MEANINGS, true);
			AddForeignKey(tableMeanings, tableAntonyms, Strings.MEANING_ID, Strings.FK_ANTONYMS_MEANINGS, true);
			AddForeignKey(tableWords, tableSynonyms, Strings.WORD_ID, Strings.FK_SYNONYMS_WORDS, true);
			AddForeignKey(tableWords, tableAntonyms, Strings.WORD_ID, Strings.FK_ANTONYMS_WORDS, true);
			AddForeignKey(tableMeanings, tableHistory, Strings.MEANING_ID, Strings.MEANING_ID, Strings.FK_HISTORY_MEANINGS, false, false);
			AddForeignKey(tableVocabularies, tableHistory, Strings.VOCABULARY_ID, Strings.VOCABULARY_ID, Strings.FK_HISTORY_VOCABULARIES, false, false);

			this.EnforceConstraints = true;
		}

		public void LoadVocabulary(String path)
		{
			Debug.Assert(System.IO.File.Exists(path));
            using (SqlCeConnection conn = DataUtils.OpenDatabase(path, Strings.PASSWORD_VOCABULARY))
            {
                    DataUtils.AssertValidSchema(conn, Strings.META_VOCABULARY, 1, 0);
				LoadHelper(new DataTableRoot[] { tableVocabularies, tableTypes, tableWords, tableMeanings, tableTranslations, tableSynonyms, tableAntonyms }, conn);
            }
		}
		
		public void SaveVocabulary(String path)
		{
			if (!File.Exists(path))
			{
				using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Strings.SCHEMA_VOCABULARY)))
				{
					DataUtils.CreateDatabase(path, Strings.PASSWORD_VOCABULARY, reader);
				}
			}

            using (SqlCeConnection conn = DataUtils.OpenDatabase(path, Strings.PASSWORD_VOCABULARY))
            {
                SaveHelper(new DataTableRoot[] { tableVocabularies, tableTypes, tableWords, tableMeanings, tableTranslations, tableSynonyms, tableAntonyms }, conn);
            }
		}

		public void CloseVocabulary()
		{
			CloseHelper(new DataTableRoot[] { tableVocabularies, tableTypes, tableWords, tableMeanings, tableTranslations, tableSynonyms, tableAntonyms });
		}
		
		public void LoadProfile(String path)
		{
			if (!File.Exists(path))
			{
				using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Strings.SCHEMA_PROFILE)))
				{
					DataUtils.CreateDatabase(path, Strings.PASSWORD_PROFILE, reader);
				}
			}

            using (SqlCeConnection conn = DataUtils.OpenDatabase(path, Strings.PASSWORD_PROFILE))
            {
                DataUtils.AssertValidSchema(conn, Strings.META_PROFILE, 1, 0);
                LoadHelper(new DataTableRoot[] { tableProfiles, tableActions, tableHistory }, conn);
            }
	 	}
		
		public void SaveProfile(String path)
		{
            if (!File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Strings.SCHEMA_PROFILE)))
                {
                    DataUtils.CreateDatabase(path, Strings.PASSWORD_PROFILE, reader);
                }
            }

            using (SqlCeConnection conn = DataUtils.OpenDatabase(path, Strings.PASSWORD_PROFILE))
            {
                SaveHelper(new DataTableRoot[] { tableProfiles, tableActions, tableHistory }, conn);

				using (SqlCeCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = "INSERT INTO Sync (SyncId, SyncMod) VALUES (@SyncId, @SyncMod)";
					cmd.Parameters.Add(new SqlCeParameter("SyncId", Guid.NewGuid()));
					cmd.Parameters.Add(new SqlCeParameter("SyncMod", DateTime.Now));
					cmd.ExecuteNonQuery();
				}
            }
		}
		
		public void CloseProfile()
		{
			CloseHelper(new DataTableRoot[] { tableProfiles, tableActions, tableHistory });
		}
	}
}
