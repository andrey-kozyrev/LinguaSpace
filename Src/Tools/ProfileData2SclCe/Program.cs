using System;
using System.Collections;
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


namespace ProfileData2SclCe
{
	class Program
	{
		static void Usage()
		{
			Console.WriteLine("Usage ProfileData2SclCe <profile>.lsp");
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
			
			IUserProfile profile = new UserProfileFactory().CreateUserProfile();
			
			try
			{
				Console.Write("Loading source profile [{0}]...", Path.GetFileName(srcPath));
				profile.Load(srcPath, null);
				Console.WriteLine("ok");

				String dstPath = srcPath + ".sdf";

				if (File.Exists(dstPath))
				{
					File.Delete(dstPath);
				}

				Console.Write("Opening destination database [{0}]...", Path.GetFileName(dstPath));
				using (SqlCeConnection conn = DataUtils.OpenDatabase(dstPath, DataUtils.PASSWORD_PROFILE, DataUtils.SCHEMA_PROFILE))
				{
					Console.WriteLine("ok");

					Console.WriteLine("Conversion started...");
					Convert(profile, conn);
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
		
        private static void Convert(IUserProfile profile, SqlCeConnection conn)
        {
			ConvertProfile(profile, conn);
		}
		
        private static void ConvertProfile(IUserProfile profile, SqlCeConnection conn)
        {
			Console.Write("profile [{0}]...", profile.Name);
			
            using (SqlCeCommand cmd = conn.CreateCommand())
            {
				cmd.CommandText = "INSERT INTO Profiles (Name, Description, DefaultVocabularyPath, Sleep, Beep) VALUES (@Name, @Description, @DefaultVocabularyPath, @Sleep, @Beep)";
				cmd.Parameters.Add(new SqlCeParameter("Name", profile.Name));
				cmd.Parameters.Add(new SqlCeParameter("Description", String.Empty));
				cmd.Parameters.Add(new SqlCeParameter("DefaultVocabularyPath", profile.DefaultVocabulary));
				cmd.Parameters.Add(new SqlCeParameter("Sleep", profile.SleepInterval));
				cmd.Parameters.Add(new SqlCeParameter("Beep", profile.Beep));
				cmd.Prepare();
				cmd.ExecuteNonQuery();
			}

			Console.WriteLine("ok");
			
			ConvertActions(profile, conn);
			
			foreach (IStatistics statistics in profile as IEnumerable)
			{
				ConvertStatistics(statistics, conn);
			}
        }

		private static void ConvertActions(IUserProfile profile, SqlCeConnection conn)
		{
			using (SqlCeCommand cmd = conn.CreateCommand())
			{
				cmd.CommandText = "INSERT INTO Actions (ActionId, Action, Weight) VALUES (@ActionId, @Action, @Weight)";
				cmd.Parameters.Add(new SqlCeParameter("ActionId", SqlDbType.NVarChar, 1));
				cmd.Parameters.Add(new SqlCeParameter("Action", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter("Weight", SqlDbType.Int));
				cmd.Prepare();

				ConvertAction("M", "Modification", 10, cmd);
				ConvertAction("R", "Right Answer", -1, cmd);
				ConvertAction("W", "Wrong Answer", 3, cmd);
				ConvertAction("P", "No Answer", 5, cmd);
			}
		}

		private static void ConvertAction(String actionId, String action, int weight, SqlCeCommand cmd)
		{
			Console.Write("answer [{0}]...", actionId);
			cmd.Parameters["ActionId"].Value = actionId;
			cmd.Parameters["Action"].Value = action;
			cmd.Parameters["Weight"].Value = weight;
			cmd.ExecuteNonQuery();
			Console.WriteLine("ok");		
		}

		private static void ConvertStatistics(IStatistics statistics, SqlCeConnection conn)
		{
			using (SqlCeCommand cmd = conn.CreateCommand())
			{
				cmd.CommandText = "INSERT INTO History (VocabularyId, MeaningId, ActionId, SyncMod) VALUES (@VocabularyId, @MeaningId, @ActionId, @SyncMod)";
				cmd.Parameters.Add(new SqlCeParameter("VocabularyId", new Guid("ad4c2b38-80fd-4920-a727-bd6bbd24cc28")));
				cmd.Parameters.Add(new SqlCeParameter("MeaningId", SqlDbType.UniqueIdentifier));
				cmd.Parameters.Add(new SqlCeParameter("ActionId", SqlDbType.NVarChar));
				cmd.Parameters.Add(new SqlCeParameter("SyncMod", SqlDbType.DateTime));
				cmd.Prepare();

				Console.Write("modified on [{0}]...", statistics.Modified);

				DateTime mininum = new DateTime(2000, 1, 1);
				cmd.Parameters["MeaningId"].Value = statistics.Guid;
				cmd.Parameters["ActionId"].Value = "M";
				cmd.Parameters["SyncMod"].Value = statistics.Modified > mininum ? statistics.Modified : mininum;
				cmd.ExecuteNonQuery();

				Console.WriteLine("ok");

				foreach (DateTime date in statistics.EnumerateRightAnswers())
				{
					Console.Write("answer right on [{0}]...", date);

					cmd.Parameters["MeaningId"].Value = statistics.Guid;
					cmd.Parameters["ActionId"].Value = "R";
					cmd.Parameters["SyncMod"].Value = date;
					cmd.ExecuteNonQuery();

					Console.WriteLine("ok");
				}

				foreach (DateTime date in statistics.EnumerateWrongAnswers())
				{
					Console.Write("answer wrong on [{0}]...", date);

					cmd.Parameters["MeaningId"].Value = statistics.Guid;
					cmd.Parameters["ActionId"].Value = "W";
					cmd.Parameters["SyncMod"].Value = date;
					cmd.ExecuteNonQuery();

					Console.WriteLine("ok");
				}

				foreach (DateTime date in statistics.EnumeratePromptAnswers())
				{
					Console.Write("answer prompt on [{0}]...", date);

					cmd.Parameters["MeaningId"].Value = statistics.Guid;
					cmd.Parameters["ActionId"].Value = "P";
					cmd.Parameters["SyncMod"].Value = date;
					cmd.ExecuteNonQuery();

					Console.WriteLine("ok");
				}
			}
		}
		
		
		
	}
}
