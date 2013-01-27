using System;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Linq;
using System.Linq.Expressions;
using LinguaSpace.Common.Diagnostics;
using LinguaSpace.Common.Text;
using LinguaSpace.Common.Data;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Grammar.Resources;
using LinguaSpace.Grammar.Data;

namespace LinguaSpace.Grammar.ComponentModel
{
	public class ApplicationModel : PresentationModel, ITransactionContextAware
	{
		private GrammarDataSet _dataSet;
		private bool _isDirty;
		private String _path;

		public ApplicationModel()
			: base(new GrammarDataSet())
		{
			_isDirty = false;
			_path = String.Empty;
		
			_dataSet = (GrammarDataSet)this.Data;
			foreach (DataTable table in _dataSet.Tables)
				table.DefaultView.ListChanged += new ListChangedEventHandler(View_ListChanged);
		}

		private void View_ListChanged(Object sender, ListChangedEventArgs e)
		{
			if (!_isDirty)
			{
				_isDirty = true;
				RaisePropertyChangedEvent("IsDirty");
			}
			RaisePropertyChangedEvent("RulesCount");
			RaisePropertyChangedEvent("ExamplesCount");
		}

		public ITransactionContext TransactionContext
		{
			get
			{
				return (ITransactionContext)this._dataSet;
			}
		}

		public void CreateGrammar()
		{
			Debug.Assert(_dataSet.Tables[Strings.TABLE_GRAMMAR].Rows.Count == 0);
			Debug.Assert(_dataSet.Tables[Strings.TABLE_TOPICS].Rows.Count == 0);
			_dataSet.Tables[Strings.TABLE_GRAMMAR].DefaultView.AddNew().EndEdit();
			_dataSet.Tables[Strings.TABLE_TOPICS].DefaultView.AddNew().EndEdit();
		}
		
		public GrammarEditModel Grammar
		{
			get
			{
				if (!IsOpen)
					return null;
				return new GrammarEditModel(_dataSet.Tables[Strings.TABLE_GRAMMAR].DefaultView[0]);
			}
		}
		
		public String GrammarTitle
		{
			get
			{
				GrammarEditModel grammar = Grammar;
				if (grammar == null)
					return String.Empty;
				return grammar.Title;
			}
		}
		
		public String GrammarLanguages
		{
			get
			{
				GrammarEditModel grammar = Grammar;
				if (grammar == null)
					return String.Empty;
				return String.Format("{0}/{1}", grammar.TargetLang.NativeName, grammar.NativeLang.NativeName);
			}
		}
		
		public int RulesCount
		{
			get
			{
				return _dataSet.Tables[Strings.TABLE_RULES].Rows.Count;
			}
		}

		public int ExamplesCount
		{
			get
			{
				return _dataSet.Tables[Strings.TABLE_EXAMPLES].Rows.Count;
			}
		}
		
		public bool IsDirty
		{
			get
			{
				return _isDirty;
			}
		}
		
		public String Path
		{
			get
			{
				return _path;
			}
		}

		public String Name
		{
			get
			{
				if (!IsOpen)
					return String.Empty;
					
				return (String)_dataSet.Tables[Strings.TABLE_GRAMMAR].DefaultView[0][Strings.COL_TITLE];
			}
		}
		
		public bool IsOpen
		{
			get
			{
				return (_dataSet.Tables[Strings.TABLE_GRAMMAR].Rows.Count > 0);
			}
		}
		
		public void Load(String path)
		{
			Debug.Assert(!IsOpen);
			_dataSet.Load(path);
			_path = path;
			_isDirty = false;
			RaisePropertyChangedEvent("Path");
			RaisePropertyChangedEvent("Name");
			RaisePropertyChangedEvent("IsOpen");
			RaisePropertyChangedEvent("IsDirty");
			RaisePropertyChangedEvent("GrammarTitle");
			RaisePropertyChangedEvent("GrammarLanguages");
			RaisePropertyChangedEvent("RulesCount");
			RaisePropertyChangedEvent("ExamplesCount");
		}

		public void SaveAs(String path)
		{
			Debug.Assert(IsOpen);
			if (path != _path && File.Exists(_path))
				File.Copy(_path, path);
			_dataSet.Save(path);
			_path = path;
			_isDirty = false;
			RaisePropertyChangedEvent("Path");
			RaisePropertyChangedEvent("IsDirty");
		}

		public void Save()
		{
			Debug.Assert(!StringUtils.IsEmpty(_path));
			SaveAs(_path);
		}

		public void Close()
		{
			Debug.Assert(IsOpen);
			_dataSet.Close();
			_path = String.Empty;
			_isDirty = false;
			RaisePropertyChangedEvent("Path");
			RaisePropertyChangedEvent("Name");
			RaisePropertyChangedEvent("IsOpen");
			RaisePropertyChangedEvent("IsDirty");
			RaisePropertyChangedEvent("GrammarTitle");
			RaisePropertyChangedEvent("GrammarLanguages");
			RaisePropertyChangedEvent("RulesCount");
			RaisePropertyChangedEvent("ExamplesCount");
		}

		public override void NotifyPropertyChanged()
		{
			base.NotifyPropertyChanged();

			RaisePropertyChangedEvent("Path");
			RaisePropertyChangedEvent("Name");
			RaisePropertyChangedEvent("IsOpen");
			RaisePropertyChangedEvent("IsDirty");
			RaisePropertyChangedEvent("GrammarTitle");
			RaisePropertyChangedEvent("GrammarLanguages");
			RaisePropertyChangedEvent("RulesCount");
			RaisePropertyChangedEvent("ExamplesCount");
		}
	}
}
