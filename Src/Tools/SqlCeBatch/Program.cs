using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlServerCe;
using System.IO;

namespace SqlCeBatch
{
    class Program
    {
        static void Usage()
        {
            Console.WriteLine("Usage: SqlCeBatch.exe <database> <password> <batch>");
        }

        static void Main(string[] args)
        {
            Debug.Assert(args.Length == 3);
            if (args.Length != 3)
            {
                Usage();
                return;
            }

            String database = args[0];
            String password = args[1];
            String batch = args[2];

            using (StreamReader reader = File.OpenText(batch))
            {
                using (SqlCeConnection conn = new SqlCeConnection(String.Format(@"Data Source = {0}; Password = '{1}'", database, password)))
                {
                    conn.Open();
                    using (SqlCeTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            while (!reader.EndOfStream)
                            {
                                String commandText = Read(reader);
                                if (commandText.Length > 0)
                                {
                                    Console.WriteLine(commandText);
                                    using (SqlCeCommand command = conn.CreateCommand())
                                    {
                                        command.CommandText = commandText;
                                        command.ExecuteNonQuery();
                                        Console.WriteLine();
                                    }
                                }
                            }
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: " + e.Message);
                            transaction.Rollback();
                        }
                    }
                    conn.Close();
                }
            }
        }

        static String Read(TextReader reader)
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
    }
}
