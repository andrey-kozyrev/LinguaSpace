using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Data.SqlServerCe;
using System.Data;
using System.Threading;

using LinguaSpace.Common;
using LinguaSpace.Common.Data;
using LinguaSpace.Data;

namespace VocabularyData2SqlCe
{
    class Program
    {
        static void Usage()
        {
            Console.WriteLine("Usage VocabularyData2SqlCe <vocabulary>.lsv");
        }

        static void Main(string[] args)
        {
            Debug.Assert(args.Length == 1);
            if (args.Length != 1)
            {
                Usage();
                return;
            }

            String srcPath = args[0];

            IVocabulary voc = new VocabularyFactory().CreateVocabulary();

            try
            {
                Console.Write("Loading source vocabulary [{0}]...", Path.GetFileName(srcPath));
                voc.Load(srcPath, null);
                Console.WriteLine("ok");

                String dstPath = srcPath + ".sdf";

                if (File.Exists(dstPath))
                {
                    File.Delete(dstPath);
                }

                Console.Write("Opening destination database [{0}]...", Path.GetFileName(dstPath));
				using (SqlCeConnection conn = DataUtils.OpenDatabase(dstPath, DataUtils.PASSWORD_VOCABULARY, DataUtils.SCHEMA_VOCABULARY))
                {
                    Console.WriteLine("ok");

                    Console.WriteLine("Conversion started...");
                    Convert(voc, conn);
                    Console.WriteLine("Conversion finished.");
                    
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine("*** ERROR: " + e.Message);
            }
        }

        private static void Convert(IVocabulary voc, SqlCeConnection conn)
        {
            ConvertVocabulary(voc, conn);
        }

        private static void ConvertVocabulary(IVocabulary voc, SqlCeConnection conn)
        {
            Console.Write("vocabulary [{0}]...", voc.Name);

            using (SqlCeCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Vocabularies (Name, Description, TargetLang, NativeLang) VALUES (@Name, @Description, @TargetLang, @NativeLang)";
                cmd.Parameters.Add(new SqlCeParameter("Name", voc.Name));
                cmd.Parameters.Add(new SqlCeParameter("Description", voc.Description));
                cmd.Parameters.Add(new SqlCeParameter("TargetLang", voc.TargetLanguage.InputLocale.Name));
                cmd.Parameters.Add(new SqlCeParameter("NativeLang", voc.NativeLanguage.InputLocale.Name));
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("ok");

            ConvertTypes(voc, conn);
            ConvertCategories(voc, conn);
            ConvertWords(voc, conn);
        }

        private static void ConvertTypes(IVocabulary voc, SqlCeConnection conn)
        {
            using (SqlCeCommand cmd = conn.CreateCommand())
            {
				cmd.CommandText = "INSERT INTO Types (Position, Type) VALUES (@Position, @Type)";
				cmd.Parameters.Add(new SqlCeParameter("Position", SqlDbType.SmallInt));
				cmd.Parameters.Add(new SqlCeParameter("Type", SqlDbType.NVarChar));
                cmd.Prepare();

				int position = 0;
                foreach (String type in voc.WordTypes)
                {
                    Console.Write("type [{0}]...", type);

					cmd.Parameters["Position"].Value = ++position;
                    cmd.Parameters["Type"].Value = type;
                    cmd.ExecuteNonQuery();

                    Console.WriteLine("ok");
                }
            }
        }

		private static void ConvertCategories(IVocabulary voc, SqlCeConnection conn)
		{
			using (SqlCeCommand cmd = conn.CreateCommand())
			{
				cmd.CommandText = "INSERT INTO Categories (Position, Category) VALUES (@Position, @Category)";
				cmd.Parameters.Add(new SqlCeParameter("Position", SqlDbType.SmallInt));
				cmd.Parameters.Add(new SqlCeParameter("Category", SqlDbType.NVarChar));
				cmd.Prepare();

				int position = 0;
				foreach (String category in voc.Categories)
				{
					Console.Write("category [{0}]...", category);

					cmd.Parameters["Position"].Value = ++position;
					cmd.Parameters["Category"].Value = category;
					cmd.ExecuteNonQuery();

					Console.WriteLine("ok");
				}
			}
		}

        private static Guid GetTypeId(String type, SqlCeConnection conn)
        {
            Guid type_id = Guid.Empty;
            using (SqlCeCommand cmd = conn.CreateCommand())
            {
				cmd.CommandText = "SELECT TypeId FROM Types WHERE Type = @Type";
				cmd.Parameters.Add(new SqlCeParameter("Type", type));
                cmd.Prepare();
                Object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    type_id = (Guid)result;
                }
            }
            return type_id;
        }

		private static Guid GetCategoryId(String category, SqlCeConnection conn)
		{
			Guid categoryId = Guid.Empty;
			using (SqlCeCommand cmd = conn.CreateCommand())
			{
				cmd.CommandText = "SELECT CategoryId FROM Categories WHERE Category = @Category";
				cmd.Parameters.Add(new SqlCeParameter("Category", category));
				cmd.Prepare();
				Object result = cmd.ExecuteScalar();
				if (result != null)
				{
					categoryId = (Guid)result;
				}
			}
			return categoryId;
		}


        private static Guid GetWordId(IWord word, SqlCeConnection conn)
        {
            Guid word_id = Guid.Empty;

            Guid type_id = GetTypeId(word.Type, conn);
            if (type_id != Guid.Empty)
            {
                using (SqlCeCommand cmd = conn.CreateCommand())
                {
					cmd.CommandText = "SELECT WordId FROM Words WHERE TypeId = @TypeId AND Prefix = @Prefix AND Word = @Word";
					cmd.Parameters.Add(new SqlCeParameter("TypeId", type_id));
                    cmd.Parameters.Add(new SqlCeParameter("Prefix", word.Prefix));
					cmd.Parameters.Add(new SqlCeParameter("Word", word.Data));
                    cmd.Prepare();
                    Object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        word_id = (Guid)result;
                    }
                }
            }

            return word_id;
        }

        private static void ConvertWords(IVocabulary voc, SqlCeConnection conn)
        {
            using (SqlCeCommand cmd = conn.CreateCommand())
            {
				cmd.CommandText = "INSERT INTO Words (TypeId, Prefix, Word) VALUES (@TypeId, @Prefix, @Word)";
				cmd.Parameters.Add(new SqlCeParameter("TypeId", SqlDbType.UniqueIdentifier));
				cmd.Parameters.Add(new SqlCeParameter("Prefix", SqlDbType.NVarChar));
				cmd.Parameters.Add(new SqlCeParameter("Word", SqlDbType.NVarChar));
                cmd.Prepare();

                foreach (IWord word in voc.Words)
                {
                    Console.Write("word [{0}]...", word.Text);

                    Guid type_id = GetTypeId(word.Type, conn);
                    if (type_id != Guid.Empty)
                    {
						cmd.Parameters["TypeId"].Value = type_id;
						cmd.Parameters["Prefix"].Value = word.Prefix;
						cmd.Parameters["Word"].Value = word.Data;
                        cmd.ExecuteNonQuery();

                        Console.WriteLine("ok");
                    }
                    else
                    {
                        Console.WriteLine("bad type");
                    }
                }
            }

            foreach (IWord word in voc.Words)
            {
                ConvertMeanings(word, conn);
            }
        }

        private static void ConvertMeanings(IWord word, SqlCeConnection conn)
        {
            Guid word_id = GetWordId(word, conn);
            if (word_id != Guid.Empty)
            {
                using (SqlCeCommand cmd = conn.CreateCommand())
                {
					cmd.CommandText = "INSERT INTO Meanings (MeaningId, Position, Prefix, WordId, Postfix, Definition, Example) VALUES (@MeaningId, @Position, @Prefix, @WordId, @Postfix, @Definition, @Example)";
					cmd.Parameters.Add(new SqlCeParameter("MeaningId", SqlDbType.UniqueIdentifier));
					cmd.Parameters.Add(new SqlCeParameter("WordId", SqlDbType.UniqueIdentifier));
					cmd.Parameters.Add(new SqlCeParameter("Position", SqlDbType.SmallInt));
					cmd.Parameters.Add(new SqlCeParameter("Prefix", SqlDbType.NVarChar));
					cmd.Parameters.Add(new SqlCeParameter("Postfix", SqlDbType.NVarChar));
					cmd.Parameters.Add(new SqlCeParameter("Definition", SqlDbType.NVarChar));
					cmd.Parameters.Add(new SqlCeParameter("Example", SqlDbType.NVarChar));
                    cmd.Prepare();

                    int position = 0;
                    foreach (IMeaning meaning in word.Meanings)
                    {
                        Console.Write("word [{0}], meaning #{1}...", word.Text, meaning.Index);

						try
						{
							Guid id = meaning.Guid;

							cmd.Parameters["MeaningId"].Value = id;
							cmd.Parameters["WordId"].Value = word_id;
							cmd.Parameters["Position"].Value = ++position;
							cmd.Parameters["Prefix"].Value = meaning.Prefix;
							cmd.Parameters["Postfix"].Value = meaning.Postfix;
							cmd.Parameters["Definition"].Value = meaning.Definition;
							cmd.Parameters["Example"].Value = meaning.Example;
							cmd.ExecuteNonQuery();

							Console.WriteLine("ok");

							ConvertMeaningCategories(meaning, conn, id);
							ConvertTranslations(meaning, conn, id);
							ConvertSynonyms(meaning, conn, id);
							ConvertAntonyms(meaning, conn, id);
						}
						catch (Exception e)
						{
							Console.WriteLine();
							Console.WriteLine("*** ERROR: " + e.Message);
						}
                    }
                }
            }
        }

		private static void ConvertMeaningCategories(IMeaning meaning, SqlCeConnection conn, Guid meaningId)
		{
			using (SqlCeCommand cmd = conn.CreateCommand())
			{
				cmd.CommandText = "INSERT INTO MeaningsCategories (MeaningId, CategoryId) VALUES (@MeaningId, @CategoryId)";
				cmd.Parameters.Add(new SqlCeParameter("MeaningId", meaningId));
				cmd.Parameters.Add(new SqlCeParameter("CategoryId", SqlDbType.UniqueIdentifier));
				
				foreach (String category in meaning.Categories)
				{
					Guid categoryId = GetCategoryId(category, conn);
					if (categoryId != Guid.Empty)
					{
						cmd.Parameters["CategoryId"].Value = categoryId;
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

        private static void ConvertTranslations(IMeaning meaning, SqlCeConnection conn, Guid meaning_id)
        {
            using (SqlCeCommand cmd = conn.CreateCommand())
            {
				cmd.CommandText = "INSERT INTO Translations (MeaningId, Position, Translation) VALUES (@MeaningId, @Position, @Translation)";
				cmd.Parameters.Add(new SqlCeParameter("MeaningId", meaning_id));
				cmd.Parameters.Add(new SqlCeParameter("Position", SqlDbType.SmallInt));
				cmd.Parameters.Add(new SqlCeParameter("Translation", SqlDbType.NVarChar));
                cmd.Prepare();

                int position = 0;
                foreach (String translation in meaning.Translations)
                {
					Console.Write("MeaningId [{0}]...", translation);
					cmd.Parameters["Position"].Value = ++position;
					cmd.Parameters["Translation"].Value = translation;
                    cmd.ExecuteNonQuery();

                    Console.WriteLine("ok");
                }
            }
        }

        private static void ConvertSynonyms(IMeaning meaning, SqlCeConnection conn, Guid meaning_id)
        {
            using (SqlCeCommand cmd = conn.CreateCommand())
            {
				cmd.CommandText = "INSERT INTO Relations (MeaningId, Relation, Position, WordId) VALUES (@MeaningId, @Relation, @Position, @WordId)";
				cmd.Parameters.Add(new SqlCeParameter("MeaningId", meaning_id));
				cmd.Parameters.Add(new SqlCeParameter("Relation", SqlDbType.NVarChar));
				cmd.Parameters.Add(new SqlCeParameter("Position", SqlDbType.SmallInt));
				cmd.Parameters.Add(new SqlCeParameter("WordId", SqlDbType.UniqueIdentifier));
                cmd.Prepare();

                int position = 0;
                foreach (IWord word in meaning.Synonyms)
                {
                    Console.Write("synonym [{0}]...", word.Text);

					cmd.Parameters["Relation"].Value = "S";
					cmd.Parameters["Position"].Value = ++position;
					cmd.Parameters["WordId"].Value = GetWordId(word, conn);
                    cmd.ExecuteNonQuery();

                    Console.WriteLine("ok");
                }
            }
        }

        private static void ConvertAntonyms(IMeaning meaning, SqlCeConnection conn, Guid meaning_id)
        {
			using (SqlCeCommand cmd = conn.CreateCommand())
			{
				cmd.CommandText = "INSERT INTO Relations (MeaningId, Relation, Position, WordId) VALUES (@MeaningId, @Relation, @Position, @WordId)";
				cmd.Parameters.Add(new SqlCeParameter("MeaningId", meaning_id));
				cmd.Parameters.Add(new SqlCeParameter("Relation", SqlDbType.NVarChar));
				cmd.Parameters.Add(new SqlCeParameter("Position", SqlDbType.SmallInt));
				cmd.Parameters.Add(new SqlCeParameter("WordId", SqlDbType.UniqueIdentifier));
				cmd.Prepare();

				int position = 0;
				foreach (IWord word in meaning.Antonyms)
				{
					Console.Write("synonym [{0}]...", word.Text);

					cmd.Parameters["Relation"].Value = "A";
					cmd.Parameters["Position"].Value = ++position;
					cmd.Parameters["WordId"].Value = GetWordId(word, conn);
					cmd.ExecuteNonQuery();

					Console.WriteLine("ok");
				}
			}
		}

    }
}
