using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using LinguaSpace.Common.Diagnostics;
using LinguaSpace.Common.Text;
using LinguaSpace.Common.Data;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Grammar.Resources;
using LinguaSpace.Grammar.Data;

namespace LinguaSpace.Grammar.ComponentModel
{
	public class ExampleItemModel : DataPresentationModel
	{
		public ExampleItemModel(DataRowView row)
			: base(row)
		{
		}

		public String NativeText
		{
			get
			{
				return (String)RowView[Strings.COL_NATIVE_TEXT];
			}
			set
			{
				Debug.Assert(value != null);
				RowView[Strings.COL_NATIVE_TEXT] = value.Trim();
			}
		}

		public String TargetText
		{
			get
			{
				return (String)RowView[Strings.COL_TARGET_TEXT];
			}
			set
			{
				Debug.Assert(value != null);
				RowView[Strings.COL_TARGET_TEXT] = value.Trim();
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
		
		public Brush Foreground
		{
			get
			{
				return (bool)RowView[Strings.COL_ACTIVE] ? SystemColors.WindowTextBrush : SystemColors.GrayTextBrush;
			}
		}

		public override void NotifyPropertyChanged()
		{
			base.NotifyPropertyChanged();
			RaisePropertyChangedEvent("NativeText");
			RaisePropertyChangedEvent("TargetText");
			RaisePropertyChangedEvent("Active");
			RaisePropertyChangedEvent("Foreground");
		}

        protected override void DeserializeOverride(BinaryReader reader)
        {
            DataSet.Deserialize(RowView.Row, reader, Strings.FK_EXAMPLES_RULES);
        }
	}

	public class ExampleEditModel : ExampleItemModel
	{
		public ExampleEditModel(DataRowView row)
			: base(row)
		{
		}

		public bool Exception
		{
			get
			{
				return (bool)RowView[Strings.COL_EXCEPTION];
			}
			set
			{
				RowView[Strings.COL_EXCEPTION] = value;
			}
		}
		
	}

	public class ExampleValidator : DataValidator
	{
		public ExampleValidator(DataRowView row)
			: base(row)
		{
			Hook(AdoUtils.GetDataView(row, Strings.TABLE_EXAMPLES));
			Validate();
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();

			if (IsValid)
			{
				if (StringUtils.IsEmpty((String)m_rv[Strings.COL_NATIVE_TEXT]))
				{
					SetStatus(false, "Example translation cannot be blank", ValidationMessageType.Error);
				}
				else if (StringUtils.IsEmpty((String)m_rv[Strings.COL_TARGET_TEXT]))
				{
					SetStatus(false, "Example text cannot be blank", ValidationMessageType.Error);
				}
			}
		}
	}
}
