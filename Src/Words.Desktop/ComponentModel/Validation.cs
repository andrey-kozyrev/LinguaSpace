using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Windows.Media;
using LinguaSpace.Common.Text;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Common.Data;
using LinguaSpace.Common.UI;
using LinguaSpace.Words.Resources;

namespace LinguaSpace.Words.ComponentModel
{
	public class VocabularyValidator : DataValidator
	{
		public VocabularyValidator(DataRowView rv) 
			: base(rv)
		{
			Hook(AdoUtils.GetDataView(this.m_rv, Strings.TYPES));
			Validate();
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();
			
			if (this.IsValid)
			{
				if (StringUtils.IsEmpty((String)m_rv[Strings.NAME]))
				{
					SetStatus(false, "Vocabulary name cannot be blank", ValidationMessageType.Error);
				}
				else if (AdoUtils.GetDataView(this.m_rv, Strings.TYPES).Count == 0)
				{
					SetStatus(false, "Word types are required", ValidationMessageType.Error);
				}
				else if (m_rv[Strings.TARGET_LANG] == null)
				{
					SetStatus(false, "Target language keyboard is required", ValidationMessageType.Error);
				}
				else if (m_rv[Strings.NATIVE_LANG] == null)
				{
					SetStatus(false, "Native language keyboard is required", ValidationMessageType.Error);
				}
			}
		}
	}

	public class TextValidator : DataValidator
	{
		protected String m_columnName;

		public TextValidator(DataRowView rv, String columnName) : base(rv)
		{
			Debug.Assert(columnName != null);
			
			m_columnName = columnName;
			
			Validate();
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();
			
			if (this.IsValid)
			{
				String text = (String)m_rv[m_columnName];
				if (StringUtils.IsEmpty(text))
				{
					SetStatus(false, "Text cannot be blank", ValidationMessageType.Error);
				}
				else if (Contains(text))
				{
					SetStatus(false, "Duplicate item", ValidationMessageType.Error);
				}
			}
		}
		
		protected virtual bool Contains(String text)
		{
			bool contains = false;
			foreach (DataRowView rv in m_rv.DataView)
			{
				if (text.Equals(rv[m_columnName]) && rv != m_rv)
				{
					contains = true;
					break;
				}
			}
			return contains;
		}
	}
	
	public class TypeValidator : TextValidator
	{
		public TypeValidator(DataRowView rv) 
			: base(rv, Strings.TYPE)
		{
		}
	}

	public class TranslationValidator : TextValidator
	{
		public TranslationValidator(DataRowView rv)
			: base(rv, Strings.TRANSLATION)
		{
		}

		protected override bool Contains(String text)
		{
			DataView view = this.m_rv.DataView;
			PropertyDescriptor property = BindingListUtils.GetProperty(view, Strings.MEANING_ID);
			DataRowView[] rows = BindingListUtils.FindItems<DataRowView>(view, property, m_rv[Strings.MEANING_ID]);
			int count = 0;
			foreach (DataRowView row in rows)
				if (row[Strings.TRANSLATION].Equals(m_rv[Strings.TRANSLATION]))
					++count;
			return count > 1;
		}
	}
	
	public class WordValidator : DataValidator
	{
		public WordValidator(DataRowView rv) : base(rv)
		{
			Validate();
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();
			
			if (IsValid)
			{
				String prefix = (String)m_rv[Strings.PREFIX];
				String word = (String)m_rv[Strings.WORD];
				Object typeId = m_rv[Strings.TYPE_ID];

				if (StringUtils.IsEmpty(word))
				{
					SetStatus(false, "Word cannot be blank", ValidationMessageType.Error);
				}
				else if (typeId == DBNull.Value)
				{
					SetStatus(false, "Type cannot be blank", ValidationMessageType.Error);
				}
				else if (Contains(prefix, word, (Guid)typeId))
				{
					SetStatus(false, "Duplicate item", ValidationMessageType.Error);
				}
			}
		}
		
		protected bool Contains(String prefix, String word, Guid typeId)
		{
			bool contains = false;
			
			foreach (DataRowView rv in m_rv.DataView)
			{
				if (rv != m_rv && prefix == (String)rv[Strings.PREFIX] && word == (String)rv[Strings.WORD] && typeId == (Guid)rv[Strings.TYPE_ID])
				{
					contains = true;
					break;
				}
			}
			
			return contains;
		}
	}

	public class MeaningValidator : DataValidator
	{
		public MeaningValidator(DataRowView rv) : base(rv)
		{
			Hook(AdoUtils.GetDataView(rv, Strings.TRANSLATIONS));
			Hook(AdoUtils.GetDataView(rv, Strings.SYNONYMS));
			Hook(AdoUtils.GetDataView(rv, Strings.ANTONYMS));
			Validate();
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();
			
			if (this.IsValid)
			{
				if (AdoUtils.FindRelatedRow(this.m_rv, Strings.TRANSLATIONS, Strings.MEANING_ID) == null)
				{
					SetStatus(false, "At least one translation is required", ValidationMessageType.Error);
				}
				else if (StringUtils.IsEmpty((String)this.m_rv[Strings.EXAMPLE]))
				{
					SetStatus(true, "An example is recommended", ValidationMessageType.Warning);
				}
				else if (StringUtils.IsEmpty((String)this.m_rv[Strings.DEFINITION]))
				{
					SetStatus(true, "A definition is recommended", ValidationMessageType.Warning);
				}
			}
		}
	}
	
	public class WordRefValidator : DataValidator
	{
		public WordRefValidator(DataRowView rv) 
			: base(rv)
		{
			Validate();
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();
			
			if (this.IsValid)
			{
				if (AdoUtils.FindRelatedRow(this.m_rv, Strings.WORDS, Strings.WORD_ID) == null)
				{
					SetStatus(false, "Word cannot be blank", ValidationMessageType.Error);
				}
				else if (HasDuplicatesHelper())
				{
					SetStatus(false, "Duplicate item", ValidationMessageType.Error);
				}
			}
		}
		
		private bool HasDuplicatesHelper()
		{
			DataView view = this.m_rv.DataView;
			PropertyDescriptor property = BindingListUtils.GetProperty(view, Strings.MEANING_ID);
			DataRowView[] rows = BindingListUtils.FindItems<DataRowView>(view, property, m_rv[Strings.MEANING_ID]);
			int count = 0;
			foreach (DataRowView row in rows)
				if (row[Strings.WORD_ID] == m_rv[Strings.WORD_ID])
					++count;
			return count > 1;
		}
	}
	
	public class ProfileValidator : DataValidator
	{
		public ProfileValidator(DataRowView row)
			: base(row)
		{
			Hook(AdoUtils.GetDataView(row, Strings.ACTIONS));
			Validate();
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();
			
			if (this.IsValid)
			{
				if (StringUtils.IsEmpty((String)this.m_rv[Strings.NAME]))
				{
					SetStatus(false, "Name cannot be blank", ValidationMessageType.Error);
				}
				else if ((int)this.m_rv[Strings.SLEEP] <= 0)
				{
					SetStatus(false, "Invalid sleep value", ValidationMessageType.Error);
				}
				else
				{
					DataView actions = AdoUtils.GetDataView(this.m_rv, Strings.ACTIONS);
					foreach (DataRowView action in actions)
					{
						String id = (String)action[Strings.ACTION_ID];
						
						int weight = (int)action[Strings.WEIGHT];
						if (id == "R")
							weight = -weight;
						
						if (weight <= 0)
						{
							SetStatus(false, String.Format("Invalid {0} value", (String)action[Strings.ACTION]), ValidationMessageType.Error);
						}
					}
				}
			}
		}
	}
}
