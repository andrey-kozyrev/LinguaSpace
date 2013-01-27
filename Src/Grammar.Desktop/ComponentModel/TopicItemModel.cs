using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using LinguaSpace.Common.Diagnostics;
using LinguaSpace.Common.Text;
using LinguaSpace.Common.Data;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Grammar.Resources;
using LinguaSpace.Grammar.Data;

namespace LinguaSpace.Grammar.ComponentModel
{
	public class TopicModel : DataPresentationModel
	{
		private PresentationBindingList<TopicModel> _topics;
		private PresentationBindingList<RuleItemModel> _rules;

		public TopicModel(DataRowView row)
			: base(row)
		{
		}

        protected override void SerializeOverride(BinaryWriter writer)
        {
            DataSet.Serialize(RowView.Row, writer, new String[] { Strings.FK_TOPICS_TOPICS, Strings.FK_RULES_TOPICS, Strings.FK_EXAMPLES_RULES });
        }

        protected override void DeserializeOverride(BinaryReader reader)
        {
            DataSet.Deserialize(RowView.Row, reader, Strings.FK_TOPICS_TOPICS);
        }

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				_topics.Dispose();
				_rules.Dispose();
			}
		}

		public String Title
		{
			get
			{
				return (String)RowView[Strings.COL_TITLE];
			}
			set
			{
				Debug.Assert(value != null);
				RowView[Strings.COL_TITLE] = value.Trim();
			}
		}
	
	    public bool IsExpanded
        {
            get
            {
                return (bool)RowView[Strings.COL_EXPANDED];
            }
            set
            {
                RowView[Strings.COL_EXPANDED] = value;
            }
        }

		public String RulesCount
		{
			get
			{
				DataRowView[] rules = AdoUtils.FindRelatedRows(RowView, Strings.TABLE_RULES, Strings.COL_TOPIC_ID);
				if (rules == null)
					return String.Empty;
				return String.Format("({0})", rules.Length);
			}
		}
		
		public Visibility RulesTextVisibility
		{
			get
			{
				return Visibility.Visible;
			}
		}
		
		public PresentationBindingList<TopicModel> Topics
		{
			get
			{
				if (_topics == null)
					_topics = PresentationUtils.CreateFilteredPresentationList<TopicModel>(AdoUtils.GetDataView(RowView, Strings.TABLE_TOPICS), Strings.COL_PARENT_TOPIC_ID, RowView[Strings.COL_TOPIC_ID]);
				return _topics;
			}
		}

		public PresentationBindingList<RuleItemModel> Rules
		{
			get
			{
				if (_rules == null)
					_rules = PresentationUtils.CreateFilteredPresentationList<RuleItemModel>(AdoUtils.GetDataView(RowView, Strings.TABLE_RULES), Strings.COL_TOPIC_ID, RowView[Strings.COL_TOPIC_ID]);
				return _rules;
			}
		}
	}

	public class TopicValidator : DataValidator
	{
		public TopicValidator(DataRowView row)
			: base(row)
		{
			Hook(AdoUtils.GetDataView(row, Strings.TABLE_TOPICS));
			Validate();
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();

			if (IsValid)
			{
				if (StringUtils.IsEmpty((String)m_rv[Strings.COL_TITLE]))
				{
					SetStatus(false, "Topic title cannot be blank", ValidationMessageType.Error);
				}
			}
		}
	}
	
}
