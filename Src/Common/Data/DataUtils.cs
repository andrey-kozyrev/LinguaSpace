using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SqlServerCe;
using System.Reflection;
using System.Net;
using LinguaSpace.Common.Resources;

namespace LinguaSpace.Common.Data
{
    public class DataUtils
    {
        public static String LCID = "1033";
		/*
		public const String PASSWORD_VOCABULARY = "{F87B1C75-0486-4cec-ACBC-7B158DD75B4B}";
		public const String PASSWORD_PROFILE = "{4AEA9869-B312-4177-BDBB-3088F0410C5D}";
		public const String SCHEMA_VOCABULARY = "Vocabulary";
		public const String SCHEMA_PROFILE = "Profile";
		 */

        private static String ReadCommand(TextReader reader)
        {
            StringBuilder builder = new StringBuilder(1024);
            int c = -1;
            while ((c = reader.Read()) >= 0)
            {
                if (c == ';')
                    break;
                builder.Append((char)c);
            }
            return builder.ToString().Trim();
        }

        private static void ExecuteCommand(SqlCeConnection conn, String commandText)
        {
            using (SqlCeCommand command = conn.CreateCommand())
            {
                command.CommandText = commandText;
                command.ExecuteNonQuery();
            }
        }

        private static void ExecuteCommands(SqlCeConnection conn, StreamReader reader)
        {
            using (SqlCeTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    while (!reader.EndOfStream)
                    {
                        String commandText = ReadCommand(reader);
                        if (commandText.Length > 0)
                        {
                            ExecuteCommand(conn, commandText);
                        }
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }

		public static void CreateDatabase(String path, String password, StreamReader schema)
		{
			Debug.Assert(!File.Exists(path));
			using (SqlCeEngine engine = new SqlCeEngine(String.Format("Data Source='{0}'; LCID={1}; Password='{2}'; Encrypt = TRUE;", path, LCID, password)))
			{
				engine.CreateDatabase();
				using (SqlCeConnection conn = OpenDatabase(path, password))
				{
					ExecuteCommands(conn, schema);
				}
			}
		}

		public static SqlCeConnection OpenDatabase(String path, String password)
		{
            SqlCeConnection conn = new SqlCeConnection(String.Format("Data Source='{0}'; Password='{1}'", path, password));
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot open file", e);
            }
            return conn;
		}

        public static void AssertValidSchema(SqlCeConnection conn, String schema, int major, int minor)
        {
            using (SqlCeCommand cmd = new SqlCeCommand(CommonStrings.SELECT_META, conn))
            {
                using (SqlCeDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        throw new ApplicationException("Meta information is missing");

                    if (schema != (String)reader.GetString(0))
                        throw new ApplicationException("Unexpected schema identifier");

                    if (major != (int)reader.GetInt32(1))
                        throw new ApplicationException("Unsupported schema version");
                }
            }
        }

		public static bool IsDatabase(String path, String password)
		{
			if (!File.Exists(path))
				return false;
				
			try
			{
				using (SqlCeConnection conn = OpenDatabase(path, password))
				{
					return (conn.State == System.Data.ConnectionState.Open);
				}
			}
			catch
			{
			}
			
			return false;
		}
    }
}
