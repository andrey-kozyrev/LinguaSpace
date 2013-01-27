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
using LinguaSpace.Grammar.Resources;

namespace LinguaSpace.Grammar.Data
{
	class GrammarDataSet : TransactedDataSet
	{
		private GrammarTable _tableGrammar;
		private TopicsTable _tableTopics;
		private RulesTable _tableRules;
		private ExamplesTable _tableExamples;

		public GrammarDataSet()
		{
			_tableGrammar = new GrammarTable();
			_tableTopics = new TopicsTable();
			_tableRules = new RulesTable();
			_tableExamples = new ExamplesTable();
			
			this.Tables.AddRange(new DataTable[] {_tableGrammar, _tableTopics, _tableRules, _tableExamples} );
			
			AddForeignKey(_tableTopics, _tableTopics, Strings.COL_TOPIC_ID, Strings.COL_PARENT_TOPIC_ID, Strings.FK_TOPICS_TOPICS, true, true);
			AddForeignKey(_tableTopics, _tableRules, Strings.COL_TOPIC_ID, Strings.FK_RULES_TOPICS, true);
			AddForeignKey(_tableRules, _tableExamples, Strings.COL_RULE_ID, Strings.FK_EXAMPLES_RULES, true);

			_tableGrammar.DefaultView.Sort = Strings.COL_GRAMMAR_ID;
			_tableTopics.DefaultView.Sort = "ParentTopicId, Position";
			_tableRules.DefaultView.Sort = "TopicId, Position";
			_tableExamples.DefaultView.Sort = "RuleId, Position";

			this.EnforceConstraints = true;
		}

		public void Load(String path)
		{
			Debug.Assert(System.IO.File.Exists(path));
			using (SqlCeConnection conn = DataUtils.OpenDatabase(path, Strings.PASSWORD_GRAMMAR))
			{
                DataUtils.AssertValidSchema(conn, Strings.META_GRAMMAR, 1, 0);
				LoadHelper(new DataTableRoot[] { _tableGrammar, _tableTopics, _tableRules, _tableExamples }, conn );
			}
		}

		public void Save(String path)
		{
			if (!File.Exists(path))
			{
				using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Strings.SCHEMA_GRAMMAR)))
				{
					DataUtils.CreateDatabase(path, Strings.PASSWORD_GRAMMAR, reader);
				}
			}

			using (SqlCeConnection conn = DataUtils.OpenDatabase(path, Strings.PASSWORD_GRAMMAR))
			{
				SaveHelper(new DataTableRoot[] { _tableGrammar, _tableTopics, _tableRules, _tableExamples }, conn);	
			}
		}

		public void Close()
		{
			CloseHelper(new DataTableRoot[] { _tableGrammar, _tableTopics, _tableRules, _tableExamples });
		}
	}
}
