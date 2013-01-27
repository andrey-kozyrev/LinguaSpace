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
using LinguaSpace.Common.Diagnostics;
using LinguaSpace.Common.Text;
using LinguaSpace.Common.Data;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Grammar.Resources;
using LinguaSpace.Grammar.Data;

namespace LinguaSpace.Grammar.ComponentModel
{
	public class GrammarEditModel : DataPresentationModel
	{
		public GrammarEditModel(DataRowView row)
			: base(row)
		{
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

		public CultureInfo TargetLang
		{
			get
			{
				return AdoUtils.GetCultureInfo(RowView, Strings.COL_TARGET_LANG);
			}

			set
			{
				AdoUtils.SetCultureInfo(RowView, Strings.COL_TARGET_LANG, value);
			}
		}

		public CultureInfo NativeLang
		{
			get
			{
				return AdoUtils.GetCultureInfo(RowView, Strings.COL_NATIVE_LANG);
			}

			set
			{
				AdoUtils.SetCultureInfo(RowView, Strings.COL_NATIVE_LANG, value);
			}
		}

        public bool Shuffle
        {
            get
            {
                return (bool)RowView[Strings.COL_SHUFFLE];
            }
            set
            {
                RowView[Strings.COL_SHUFFLE] = value;
            }
        }

        public bool ShowRule
        {
            get
            {
                return (bool)RowView[Strings.COL_SHOWRULE];
            }
            set
            {
                RowView[Strings.COL_SHOWRULE] = value;
            }
        }

		public TopicModel RootTopic
		{
			get
			{
				return new TopicModel(AdoUtils.GetDataView(RowView, Strings.TABLE_TOPICS)[0]);
			}
		}
	}
	
	public class GrammarValidator : DataValidator
	{
		public GrammarValidator(DataRowView row)
			: base(row)
		{
			Hook(AdoUtils.GetDataView(row, Strings.TABLE_GRAMMAR));
			Validate();
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();
			
			if (IsValid)
			{
				if (StringUtils.IsEmpty((String)m_rv[Strings.COL_TITLE]))
				{
					SetStatus(false, "Grammar title cannot be blank", ValidationMessageType.Error);
				}
			}
		}
	}
}
