using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using LinguaSpace.Common.Diagnostics;
using LinguaSpace.Common.Text;
using LinguaSpace.Common.Data;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Grammar.Resources;
using LinguaSpace.Grammar.Data;

namespace LinguaSpace.Grammar.ComponentModel
{
	public class RuleModelBase : DataPresentationModel
	{
		public RuleModelBase(DataRowView row)
			: base(row)
		{
		}

		public String Comment
		{
			get
			{
				return (String)RowView[Strings.COL_COMMENT];
			}
			set
			{
				Debug.Assert(value != null);
				RowView[Strings.COL_COMMENT] = value.Trim();
			}
		}

        protected override void SerializeOverride(BinaryWriter writer)
        {
            DataSet.Serialize(RowView.Row, writer, new String[] { Strings.FK_EXAMPLES_RULES });
        }

        protected override void DeserializeOverride(BinaryReader reader)
        {
            DataSet.Deserialize(RowView.Row, reader, Strings.FK_RULES_TOPICS);
        }
	}

	public class RuleItemModel : RuleModelBase
	{
		public RuleItemModel(DataRowView row)
			: base(row)
		{
		}

		public String Examples
		{
			get
			{
				DataRowView[] examples = AdoUtils.FindRelatedRows(RowView, Strings.TABLE_EXAMPLES, Strings.COL_RULE_ID);
				if (examples == null)
					return String.Empty;
					
				StringBuilder sb = new StringBuilder(examples.Length * 200);

                int count = 3;
				foreach (DataRowView example in examples)
				{
					String text = (String)example[Strings.COL_TARGET_TEXT];
					if (text != String.Empty)
					{
						if (sb.Length > 0)
							sb.Append("\n");
						sb.Append(text);

                        if (--count == 0)
                            break;
					}
				}
				
				return sb.ToString();
			}
		}
		
		public Visibility ExamplesVisibility
		{
			get
			{
				if (!(bool)RowView[Strings.COL_ACTIVE])
					return Visibility.Collapsed;
			
				DataRowView example = AdoUtils.FindRelatedRow(RowView, Strings.TABLE_EXAMPLES, Strings.COL_RULE_ID);
				return (example != null) ? Visibility.Visible : Visibility.Collapsed;
			}
		}
		
		public override void NotifyPropertyChanged()
		{
			base.NotifyPropertyChanged();
			RaisePropertyChangedEvent("Examples");
			RaisePropertyChangedEvent("ExamplesVisibility");
			RaisePropertyChangedEvent("Foreground");
			RaisePropertyChangedEvent("Active");
		}
		
		public Brush Foreground
		{
			get
			{
				return (bool)RowView[Strings.COL_ACTIVE] ? SystemColors.WindowTextBrush : SystemColors.GrayTextBrush;
			}
		}

		public bool Active
		{
			get
			{
				return (bool)RowView[Strings.COL_ACTIVE];
			}
			set
			{
				RowView[Strings.COL_ACTIVE] = value;
			}
		}
	}

	public class RuleEditModel : RuleModelBase
	{
		private PresentationBindingList<ExampleItemModel> _examples;
	
		public RuleEditModel(DataRowView row)
			: base(row)
		{
		}

		public bool Active
		{
			get
			{
				return (bool)RowView[Strings.COL_ACTIVE];
			}
			set
			{
				RowView[Strings.COL_ACTIVE] = value;
			}
		}

		public PresentationBindingList<ExampleItemModel> Examples
		{
			get
			{
				if (_examples == null)
					_examples = PresentationUtils.CreateFilteredPresentationList<ExampleItemModel>(AdoUtils.GetDataView(RowView, Strings.TABLE_EXAMPLES), Strings.COL_RULE_ID, RowView[Strings.COL_RULE_ID]);
				return _examples;
			}
		}
	}

	public class RuleValidator : DataValidator
	{
		public RuleValidator(DataRowView row)
			: base(row)
		{
			Hook(AdoUtils.GetDataView(row, Strings.TABLE_RULES));
			Hook(AdoUtils.GetDataView(row, Strings.TABLE_EXAMPLES));
			Validate();
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();

			if (IsValid)
			{
				if (StringUtils.IsEmpty((String)m_rv[Strings.COL_COMMENT]))
				{
					SetStatus(false, "Rule cannot be blank", ValidationMessageType.Error);
				}
				/*
				else if (AdoUtils.FindRelatedRow(m_rv, Strings.TABLE_EXAMPLES, Strings.COL_RULE_ID) == null)
				{
					SetStatus(false, "At least one example is required", ValidationMessageType.Error);
				}
				 */
			}
		}
	}
	
}
