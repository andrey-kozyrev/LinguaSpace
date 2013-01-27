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
using LinguaSpace.Common.Diagnostics;
using LinguaSpace.Common.Text;
using LinguaSpace.Common.Data;
using LinguaSpace.Common.UI;
using LinguaSpace.Common.Collections;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Grammar.Resources;
using LinguaSpace.Grammar.Data;
using LinguaSpace.Grammar.UI;

namespace LinguaSpace.Grammar.ComponentModel
{
	public enum PracticeState
	{
		Invalid,
		Question,
		Answer
	}

	public class PracticeModel : PresentationModel
	{
		private String _nativeLang;
		private String _targetLang;
	
		private Queue<DataRowView> _examples;
		private DataRowView _current;
		private String _targetTextAnswer;
		private PracticeState _state;
		private bool _isAnswerCorrect;
        private bool _showRuleText;

		private EventHandler _ruleTextChanged;

		private static readonly ImageSource BmpsInfo32;
		private static readonly ImageSource BmpsQuestion32;
		private static readonly ImageSource BmpsError32;
		private static readonly ImageSource BmpsOK32;

		static PracticeModel()
		{
			BmpsError32 = WPFUtils.GetPngImageSource("Grammar", "Status", "Error", 32);
			BmpsQuestion32 = WPFUtils.GetPngImageSource("Grammar", "Status", "Question", 32);
			BmpsInfo32 = WPFUtils.GetPngImageSource("Grammar", "Status", "Info", 32);
			BmpsOK32 = WPFUtils.GetPngImageSource("Grammar", "Status", "OK", 32);
		}

		public PracticeModel(TopicModel topic)
			: base()
		{
            DataRowView grammar = AdoUtils.GetDataView(topic.RowView, Strings.TABLE_GRAMMAR)[0];

			CultureInfo nativeInfo = AdoUtils.GetCultureInfo(grammar, Strings.COL_NATIVE_LANG);
			if (nativeInfo != null)
				_nativeLang = nativeInfo.NativeName;

			CultureInfo targetInfo = AdoUtils.GetCultureInfo(grammar, Strings.COL_TARGET_LANG);
			if (targetInfo != null)
				_targetLang = targetInfo.NativeName;

            _showRuleText = (bool)grammar[Strings.COL_SHOWRULE];

			_state = PracticeState.Invalid;
			_examples = new Queue<DataRowView>();

            IList<DataRowView> list = new List<DataRowView>();
			AddTopic(list, topic.RowView);

            if ((bool)grammar[Strings.COL_SHUFFLE])
                ListUtils.Shuffle(list);

            foreach (DataRowView example in list)
                _examples.Enqueue(example);

			Next();
		}

		private void AddTopic(IList<DataRowView> list, DataRowView topic)
		{
			Debug.Assert(topic != null);

			DataRowView[] rules = AdoUtils.FindRelatedRows(topic, Strings.TABLE_RULES, Strings.COL_TOPIC_ID);
			if (rules != null)
			{
				foreach (DataRowView rule in rules)
				{
					if ((bool)rule[Strings.COL_ACTIVE])
					{
						DataRowView[] examples = AdoUtils.FindRelatedRows(rule, Strings.TABLE_EXAMPLES, Strings.COL_RULE_ID);
						if (examples != null)
						{
							foreach (DataRowView example in examples)
							{
								if ((bool)example[Strings.COL_ACTIVE])
								{
                                    list.Add(example);
								}
							}
						}
					}
				}
			}

			DataRowView[] subtopics = AdoUtils.FindRelatedRows(topic, Strings.TABLE_TOPICS, Strings.COL_TOPIC_ID, Strings.COL_PARENT_TOPIC_ID);
			if (subtopics != null)
				foreach (DataRowView subtopic in subtopics)
					AddTopic(list, subtopic);	
		}

		private String BuildTitle(String title, DataRowView topic)
		{
			String t = title.Length > 0 ? String.Format("{0} / {1}", (String)topic[Strings.COL_TITLE], title) : (String)topic[Strings.COL_TITLE];
			topic = AdoUtils.FindRelatedRow(topic, Strings.TABLE_TOPICS, Strings.COL_PARENT_TOPIC_ID, Strings.COL_TOPIC_ID);
			return topic != null ? BuildTitle(t, topic) : title;
		}

		public String TitleText
		{
			get
			{
				if (_current == null)
					return String.Empty;

				DataRowView rule = AdoUtils.FindRelatedRow(_current, Strings.TABLE_RULES, Strings.COL_RULE_ID);
				DataRowView topic = AdoUtils.FindRelatedRow(rule, Strings.TABLE_TOPICS, Strings.COL_TOPIC_ID);
				return BuildTitle(String.Empty, topic);
			}
		}
		
		public String RuleText
		{
			get
			{
				if (_current == null)
					return String.Empty;

                if (!_showRuleText && _state != PracticeState.Answer)
                    return "Rule is hidden.";

				DataRowView rule = AdoUtils.FindRelatedRow(_current, Strings.TABLE_RULES, Strings.COL_RULE_ID);
				return (String)rule[Strings.COL_COMMENT];
			}
		}
		
		public String NativeCaption
		{
			get
			{
				return _nativeLang;
			}
		}
		
		public String TargetCaption
		{
			get
			{
				return _targetLang;
			}
		}
		
		public String NativeText
		{
			get
			{
				if (_current == null)
					return String.Empty;
				return (String)_current[Strings.COL_NATIVE_TEXT];
			}
		}
		
		public String TargetText
		{
			get
			{
				if (_current == null || _state != PracticeState.Answer)
					return String.Empty;
				return (String)_current[Strings.COL_TARGET_TEXT];
			}
		}
		
		public String TargetTextAnswer
		{
			get
			{
				return _targetTextAnswer;
			}
			set
			{
				Debug.Assert(value != null);
				_targetTextAnswer = value;
			}
		}

		public String ExampleHint
		{
			get
			{
				if (_current == null)
					return String.Empty;

                if (!_showRuleText && _state != PracticeState.Answer)
                    return String.Empty;

				return (bool)_current[Strings.COL_EXCEPTION] ? "Exception!" : "Regular.";
			}
		}
		
		public Brush ExampleHintBrush
		{
			get
			{
				if (_current == null)
					return Brushes.Transparent;

				return (bool)_current[Strings.COL_EXCEPTION] ? Brushes.Red : Brushes.Green;
			}
		}
		
		public bool IsNextEnabled
		{
			get
			{
				return _state == PracticeState.Answer && _isAnswerCorrect;
			}
		}
		
		public bool IsAnswerEnabled
		{
			get
			{
				return _state == PracticeState.Question || _state == PracticeState.Answer && !_isAnswerCorrect;
			}
		}

		public bool IsAnswerReadOnly
		{
			get
			{
				return !IsAnswerEnabled;
			}
		}

		private String PrepareAnswerTemplate(String answer)
		{
			bool hide = false;
			StringBuilder sb = new StringBuilder();
			foreach (char c in answer)
			{
				if (c == '[')
				{
					hide = true;
				}
				else if (c == ']')
				{
					hide = false;
				}
				else if (hide)
				{
					sb.Append(".");
				}
				else
				{
					sb.Append(c);
				}
			}
			return sb.ToString();
		}

		private String NormalizeString(String text)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char c in text)
				if (c != '[' && c != ']' && !Char.IsWhiteSpace(c) && !Char.IsPunctuation(c))
					sb.Append(c);
			return sb.ToString().ToLower();
		}
		
		public String StatusText
		{
			get
			{
				String text = String.Empty;
			
				switch (_state)
				{
					case PracticeState.Question:
						text = String.Format("Please read the rule above and translate the text.", _nativeLang, _targetLang);
						break;
					case PracticeState.Answer:
						text = _isAnswerCorrect ? "Your translation was correct!" : "Your translation was not correct. Please read the correct translation";
						break;
					default:
						text = "Internal error";
						break;
				}
				
				return text;
			}
		}
		
		public ImageSource StatusImage
		{
			get
			{
				ImageSource src = null;
			
				switch (_state)
				{
					case PracticeState.Question:
						src = BmpsQuestion32;
						break;
					case PracticeState.Answer:
						src = _isAnswerCorrect ? BmpsOK32 : BmpsError32;
						break;
					default:
						src = BmpsError32;
						break;
				}
				
				return src;
			}
		}

		public void Next()
		{
			if (_examples.Count > 0)
			{
				DataRowView previous = _current;
				_current = _examples.Dequeue();

				_state =  PracticeState.Question;
				_targetTextAnswer = PrepareAnswerTemplate((String)_current[Strings.COL_TARGET_TEXT]);
				
				NotifyPropertyChanged();

				if (previous == null || !previous[Strings.COL_RULE_ID].Equals(_current[Strings.COL_RULE_ID]))
					if (_ruleTextChanged != null)
						_ruleTextChanged(this, new EventArgs());
			}
		}
		
		public void Answer()
		{
			_state = PracticeState.Answer;
			_isAnswerCorrect = NormalizeString(TargetText) == NormalizeString(_targetTextAnswer);
			NotifyPropertyChanged();
		}

		public void Skip()
		{
			Next();	
		}

		public override void NotifyPropertyChanged()
		{
			base.NotifyPropertyChanged();
			RaisePropertyChangedEvent("TitleText");
			RaisePropertyChangedEvent("RuleText");
			RaisePropertyChangedEvent("NativeText");
			RaisePropertyChangedEvent("TargetText");
			RaisePropertyChangedEvent("TargetTextAnswer");
			RaisePropertyChangedEvent("ExampleHint");
			RaisePropertyChangedEvent("ExampleHintBrush");
			RaisePropertyChangedEvent("IsNextEnabled");
			RaisePropertyChangedEvent("IsAnswerEnabled");
			RaisePropertyChangedEvent("IsAnswerReadOnly");
			RaisePropertyChangedEvent("StatusText");
			RaisePropertyChangedEvent("StatusImage");
		}
		
		public bool IsEmpty
		{
			get
			{
				return (_examples.Count == 0);
			}
		}

		public event EventHandler RuleTextChanged
		{
			add
			{
				_ruleTextChanged += value;
			}
			remove
			{
				_ruleTextChanged -= value;
			}
		}
	}
}
