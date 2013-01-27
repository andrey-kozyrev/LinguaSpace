using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using LinguaSpace.Common.Text;
using LinguaSpace.Common.Data;
using LinguaSpace.Words.Resources;

namespace LinguaSpace.Words.ComponentModel
{
	public static class WordsTextUtils
	{
		public static String GetWordText(DataRowView row)
		{
			if (row == null)
				return String.Empty;
		
			StringBuilder sb = new StringBuilder();

			String prefix = (String)row[Strings.PREFIX];
			if (!StringUtils.IsEmpty(prefix))
			{
				sb.Append(prefix);
				sb.Append(" ");
			}

			String word = (String)row[Strings.WORD];
			sb.Append(word);

			return sb.ToString();
		}
		
		public static String GetMeaningTranslations(DataRowView row)
		{
			StringBuilder sb = new StringBuilder(128);
		
			DataRowView[] translations = AdoUtils.FindRelatedRows(row, Strings.TRANSLATIONS, Strings.MEANING_ID);

			if (translations != null)
			{
				for (int j = 0; j < translations.Length; ++j)
				{
					if (j > 0)
						sb.Append(", ");

					sb.Append(translations[j][Strings.TRANSLATION]);
				}
			}
			
			return sb.ToString();
		}
	}
}
