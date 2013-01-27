using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;
using System.Data.SqlServerCe;
using LinguaSpace.Words.Practice.Resources;
using LinguaSpace.Common.Diagnostics;
using LinguaSpace.Common.Collections;
using LinguaSpace.Common.Data;
using LinguaSpace.Common.Text;

namespace LinguaSpace.Words.Practice
{
	public class FlashCardsGenerator : IFlashCardsGenerator, IDisposable
	{
		private const bool TRACE = true;

		private const Int32 SLEEP_INIT = 0;
		private const Int32 SLEEP_INC = 50;

#if PLATFORM_DESKTOP
        private const Int32 ANSWERS_NUMBER = 5;
#else
        private const Int32 ANSWERS_NUMBER = 4;
#endif

		private bool _closing;
		
		private EventWaitHandle _eventPipelineFrame;
		private EventWaitHandle _eventPipelineCardsIn;
		private EventWaitHandle _eventPipelineCardsOut;
		private EventWaitHandle _eventUI;
		
		private Thread _threadMeanings;
		private Thread _threadFrame;
		private Thread _threadCardsIn;
		private Thread _threadCardsOut;
		
		private Int32 _sleep;
	
		private static int FRAME_SIZE = 20;
	
		private List<MeaningData> _meanings;
		private Queue<MeaningData> _frame;
		private Queue<FlashCard> _cardsIn;
		private Queue<FlashCard> _cardsOut;

		private readonly SqlCeConnection _connVocabulary;
		private readonly SqlCeConnection _connProfile;
	
		private readonly Guid _vocabularyId;

		private IList<DGenerateCard> _generators;
		private IDictionary<String, SqlCeCommand> _commands;
		
		public FlashCardsGenerator(String vocabulary, String profile)
		{
			_closing = false;
		
			// pipeline
			_eventPipelineFrame = new EventWaitHandle(true, EventResetMode.AutoReset);
			_eventPipelineCardsIn = new EventWaitHandle(true, EventResetMode.AutoReset);
			_eventPipelineCardsOut = new EventWaitHandle(true, EventResetMode.AutoReset);
			
			_eventUI = new EventWaitHandle(false, EventResetMode.ManualReset);
			
			_meanings = new List<MeaningData>();
			_frame = new Queue<MeaningData>();
			_cardsIn = new Queue<FlashCard>();
			_cardsOut = new Queue<FlashCard>();
		
			// Connections
			_connVocabulary = DataUtils.OpenDatabase(vocabulary, PracticeStrings.PASSWORD_VOCABULARY);
			_connProfile = DataUtils.OpenDatabase(profile, PracticeStrings.PASSWORD_PROFILE);

			// Commands
			_commands = new Dictionary<String, SqlCeCommand>();
			
			SqlCeCommand cmd = null;
			cmd = new SqlCeCommand(PracticeStrings.SQL_VOCABULARY_ID, _connVocabulary);
			AddCommand(cmd);

			cmd = new SqlCeCommand(PracticeStrings.SQL_SELECT_MEANINGS, _connVocabulary);
			AddCommand(cmd);

			cmd = new SqlCeCommand(PracticeStrings.SQL_SELECT_HISTORY, _connProfile);
			cmd.Parameters.Add(new SqlCeParameter("MeaningId", SqlDbType.UniqueIdentifier));
			AddCommand(cmd);

			cmd = new SqlCeCommand(PracticeStrings.SQL_UPDATE_HISTORY, _connProfile);
			cmd.Parameters.Add(new SqlCeParameter("VocabularyId", SqlDbType.UniqueIdentifier));
			cmd.Parameters.Add(new SqlCeParameter("MeaningId", SqlDbType.UniqueIdentifier));
			cmd.Parameters.Add(new SqlCeParameter("ActionId", SqlDbType.NVarChar));
			AddCommand(cmd);

			cmd = new SqlCeCommand(PracticeStrings.SQL_HISTORY, _connProfile);
			cmd.Parameters.Add(new SqlCeParameter("VocabularyId", SqlDbType.UniqueIdentifier));
			cmd.Parameters.Add(new SqlCeParameter("SyncMod", SqlDbType.DateTime));
			AddCommand(cmd);

			cmd = new SqlCeCommand(PracticeStrings.SQL_MEANING_CHECK, _connVocabulary);
			cmd.Parameters.Add(new SqlCeParameter("MeaningId", SqlDbType.UniqueIdentifier));
			AddCommand(cmd);

			cmd = new SqlCeCommand(PracticeStrings.SQL_TYPE_ID, _connVocabulary);
			cmd.Parameters.Add(new SqlCeParameter("MeaningId", SqlDbType.UniqueIdentifier));
			AddCommand(cmd);
			
			_generators = new List<DGenerateCard>();
			_generators.Add(DoGenerateCardWordTranslations);
			_generators.Add(DoGenerateCardWordSynonyms);
			_generators.Add(DoGenerateCardWordAntonyms);
			_generators.Add(DoGenerateCardWordDefinitions);
			_generators.Add(DoGenerateCardTranslationsWords);
			_generators.Add(DoGenerateCardDefinitionWords);

			_vocabularyId = (Guid)_commands[PracticeStrings.SQL_VOCABULARY_ID].ExecuteScalar();

			_threadMeanings = new Thread(PipelineMeanings);
			_threadMeanings.Start();
			
			_threadFrame = new Thread(PipelineFrame);
			_threadFrame.Start();
			
			_threadCardsIn = new Thread(PipelineCardsIn);
			_threadCardsIn.Start();
			
			_threadCardsOut = new Thread(PipelineCardsOut);
			_threadCardsOut.Start();
		}

		public IFlashCard Generate()
		{
			FlashCard card = null;

#if PLATFORM_DESKTOP			
			if (!_eventUI.WaitOne(3000, false))
#else
			if (!_eventUI.WaitOne(30000, false))
#endif
				return null;
			
			lock (_cardsIn)
			{
				card = _cardsIn.Dequeue();
				if (_cardsIn.Count == 0)
				{
					_eventUI.Reset();
				}
			}

			card.StatusChanged += new StatusChangedHandler(OnCardStatusChanged);
			
			return card;
		}

		private void AddCommand(SqlCeCommand cmd)
		{
			cmd.Prepare();
			_commands[cmd.CommandText] = cmd;
		}

		private void OnCardStatusChanged(Object sender, StatusEventArgs args)
		{
			FlashCard card = (FlashCard)sender;
			lock (_cardsOut)
			{
				_cardsOut.Enqueue(card);
			}

			_eventPipelineFrame.Set();
			_eventPipelineCardsIn.Set();
			_eventPipelineCardsOut.Set();
		}
	
		private bool DoCheckMeaningId(Guid meaningId)
		{
			SqlCeCommand cmd = _commands[PracticeStrings.SQL_MEANING_CHECK];
			cmd.Parameters["MeaningId"].Value = meaningId;
			return ( (int)cmd.ExecuteScalar() > 0 );
		}
	
		private FlashCard DoGenerate(Guid meaningId)
		{
			Guid typeId = DoGetTypeId(meaningId);
			if (typeId == Guid.Empty)
				return null;

			FlashCard card = null;
		
			ListUtils.Shuffle<DGenerateCard>(_generators);
			for (int i = 0; i < _generators.Count && card == null; ++i)
				card = _generators[i](meaningId, typeId);

			return card;
		}

		private delegate FlashCard DGenerateCard(Guid meaningId, Guid typeId);

		private FlashCard DoGenerateCardWordTranslations(Guid meaningId, Guid typeId)
		{
			FlashCard card = new FlashCard();

			if (!DoAddQuestion(card, meaningId, PracticeStrings.SQL_QUESTION_WORD, GetWordText, FlashCardItemType.Word))
				return null;

			if (!DoAddAnswer(card, meaningId, PracticeStrings.SQL_ANSWER_TRANSLATIONS, GetTranslationText, FlashCardItemType.Translations))
				return null;

			if (!DoAddAnswers(card, meaningId, typeId, PracticeStrings.SQL_ANSWERS_TRANSLATIONS, GetTranslationText, FlashCardItemType.Translations))
				return null;

			return card;
		}

		private FlashCard DoGenerateCardWordSynonyms(Guid meaningId, Guid typeId)
		{
			FlashCard card = new FlashCard();

			if (!DoAddQuestion(card, meaningId, PracticeStrings.SQL_QUESTION_WORD, GetWordText, FlashCardItemType.Word))
				return null;

			if (!DoAddAnswer(card, meaningId, PracticeStrings.SQL_ANSWER_SYNONYMS, GetWordText, FlashCardItemType.Synonyms))
				return null;

			if (!DoAddAnswers(card, meaningId, typeId, PracticeStrings.SQL_ANSWERS_RELATIONS, GetWordText, FlashCardItemType.Synonyms))
				return null;

			return card;
		}

		private FlashCard DoGenerateCardWordAntonyms(Guid meaningId, Guid typeId)
		{
			FlashCard card = new FlashCard();

			if (!DoAddQuestion(card, meaningId, PracticeStrings.SQL_QUESTION_WORD, GetWordText, FlashCardItemType.Word))
				return null;

			if (!DoAddAnswer(card, meaningId, PracticeStrings.SQL_ANSWER_ANTONYMS, GetWordText, FlashCardItemType.Antonyms))
				return null;

			if (!DoAddAnswers(card, meaningId, typeId, PracticeStrings.SQL_ANSWERS_RELATIONS, GetWordText, FlashCardItemType.Antonyms))
				return null;

			return card;
		}

		private FlashCard DoGenerateCardWordDefinitions(Guid meaningId, Guid typeId)
		{
			FlashCard card = new FlashCard();

			if (!DoAddQuestion(card, meaningId, PracticeStrings.SQL_QUESTION_WORD, GetWordText, FlashCardItemType.Word))
				return null;

			if (!DoAddAnswer(card, meaningId, PracticeStrings.SQL_ANSWER_DEFINITION, GetDefinitionText, FlashCardItemType.Definition))
				return null;

			if (!DoAddAnswers(card, meaningId, typeId, PracticeStrings.SQL_ANSWERS_DEFINITION, GetDefinitionText, FlashCardItemType.Definition))
				return null;
			
			return card;
		}

		private FlashCard DoGenerateCardTranslationsWords(Guid meaningId, Guid typeId)
		{
			FlashCard card = new FlashCard();

			if (!DoAddQuestion(card, meaningId, PracticeStrings.SQL_QUESTION_TRANSLATIONS, GetTranslationText, FlashCardItemType.Translations))
				return null;

			if (!DoAddAnswer(card, meaningId, PracticeStrings.SQL_ANSWER_WORD, GetWordText, FlashCardItemType.Word))
				return null;

			if (!DoAddAnswers(card, meaningId, typeId, PracticeStrings.SQL_ANSWERS_WORD, GetWordText, FlashCardItemType.Word))
				return null;

			return card;
		}

		private FlashCard DoGenerateCardDefinitionWords(Guid meaningId, Guid typeId)
		{
			FlashCard card = new FlashCard();

			if (!DoAddQuestion(card, meaningId, PracticeStrings.SQL_QUESTION_DEFINITION, GetDefinitionText, FlashCardItemType.Definition))
				return null;

			if (!DoAddAnswer(card, meaningId, PracticeStrings.SQL_ANSWER_WORD, GetWordText, FlashCardItemType.Word))
				return null;

			if (!DoAddAnswers(card, meaningId, typeId, PracticeStrings.SQL_ANSWERS_WORD, GetWordText, FlashCardItemType.Word))
				return null;

			return card;
		}

		private delegate String DGetText(SqlCeDataReader reader);

		private String GetTranslationText(SqlCeDataReader reader)
		{
			return reader.GetString(reader.GetOrdinal("Translation"));
		}

		private void AddString(StringBuilder builder, SqlCeDataReader reader, String column)
		{
			String text = reader.GetString(reader.GetOrdinal(column));
			if (!StringUtils.IsEmpty(text))
			{
				if (builder.Length > 0)
				{
					builder.Append(" ");	
				}
				builder.Append(text);
			}
		}

		private String GetWordText(SqlCeDataReader reader)
		{
			StringBuilder builder = new StringBuilder();
			AddString(builder, reader, "UsagePrefix");
			AddString(builder, reader, "Prefix");
			AddString(builder, reader, "Word");
			AddString(builder, reader, "UsagePostfix");
			return builder.ToString();
		}

		private String GetExampleText(SqlCeDataReader reader)
		{
			return reader.GetString(reader.GetOrdinal("Example"));
		}

		private String GetDefinitionText(SqlCeDataReader reader)
		{
			return reader.GetString(reader.GetOrdinal("Definition"));
		}

		private FlashCardItem DoCreateCardItem(Guid meaningId, String query, DGetText GetText, FlashCardItemType itemType, bool useExample)
		{
			FlashCardItem item = null;

			SqlCeCommand cmd;
			lock (_commands)
			{
				if (!_commands.TryGetValue(query, out cmd))
				{
					cmd = new SqlCeCommand(query, _connVocabulary);
					cmd.Parameters.Add(new SqlCeParameter("MeaningId", SqlDbType.UniqueIdentifier));
					AddCommand(cmd);
				}
			}
		
			lock (_connVocabulary)
			{
				cmd.Parameters["MeaningId"].Value = meaningId;
				using (SqlCeDataReader reader = cmd.ExecuteReader())
				{
					StringBuilder sb = new StringBuilder();
					String example = String.Empty;
					while (reader.Read())
					{
						if (sb.Length > 0)
							sb.Append(", ");
						sb.Append(GetText(reader));

						if (useExample)
							example = GetExampleText(reader);
					}

					if (sb.Length > 0)
						item = new FlashCardItem(meaningId, itemType, sb.ToString(), example);
				}
			}
			
			return item;
		}

		private bool DoAddQuestion(FlashCard card, Guid meaningId, String query, DGetText GetText, FlashCardItemType itemType)
		{
			Debug.Assert(card != null);
			
			FlashCardItem item = DoCreateCardItem(meaningId, query, GetText, itemType, true);
			if (item == null)
				return false;
			card.Question = item;
			
			return true;
		}

		private bool DoAddAnswer(FlashCard card, Guid meaningId, String query, DGetText GetText, FlashCardItemType itemType)
		{
			Debug.Assert(card != null);
			Debug.Assert(card.InnerAnswers.Count == 0);
			FlashCardItem item = DoCreateCardItem(meaningId, query, GetText, itemType, false);
			if (item == null)
				return false;
			card.InnerAnswers.Add(item);
			return true;
		}

		private bool DoAddAnswers(FlashCard card, Guid meaningId, Guid typeId, String query, DGetText GetText, FlashCardItemType itemType)
		{
			Debug.Assert(card != null);

			SqlCeCommand cmd = null;
			lock (_commands)
			{
				if (!_commands.TryGetValue(query, out cmd))
				{
					cmd = new SqlCeCommand(query, _connVocabulary);
					cmd.Parameters.Add(new SqlCeParameter("TypeId", SqlDbType.UniqueIdentifier));
					cmd.Parameters.Add(new SqlCeParameter("SeedId", SqlDbType.UniqueIdentifier));
					AddCommand(cmd);
					_commands[query] = cmd;
				}
			}

			lock (_connVocabulary)
			{
				cmd.Parameters["TypeId"].Value = typeId;

				for (int i = 0; i < 10 && card.InnerAnswers.Count < ANSWERS_NUMBER; ++i)
				{
					cmd.Parameters["SeedId"].Value = Guid.NewGuid();

					using (SqlCeDataReader reader = cmd.ExecuteReader())
					{
						StringBuilder sb = new StringBuilder();
						Guid previousMeaningId = Guid.Empty;

                        while (card.InnerAnswers.Count < ANSWERS_NUMBER && reader.Read())
						{
							Guid currentMeaningId = reader.GetGuid(0);

							if (currentMeaningId != meaningId && card.InnerAnswers.Count<FlashCardItem>(x => x.Id == currentMeaningId) == 0)
							{
								if (currentMeaningId != previousMeaningId && previousMeaningId != Guid.Empty)
								{
									Debug.Assert(sb.Length > 0);
									FlashCardItem item = new FlashCardItem(previousMeaningId, itemType, sb.ToString());
									card.InnerAnswers.Add(item);
									sb.Length = 0;
								}

								if (sb.Length > 0)
									sb.Append(", ");

								sb.Append(GetText(reader));

								previousMeaningId = currentMeaningId;
							}
						}

                        if (card.InnerAnswers.Count < ANSWERS_NUMBER && previousMeaningId != Guid.Empty && sb.Length > 0)
						{
							FlashCardItem item = new FlashCardItem(previousMeaningId, itemType, sb.ToString());
							card.InnerAnswers.Add(item);
						}
					}
				}
			}

			return card.InnerAnswers.Count > 2;
		}

		private Guid DoGetTypeId(Guid meaningId)
		{
			SqlCeCommand cmd = DoGetCachedCommand(PracticeStrings.SQL_TYPE_ID);
		
			Guid typeId = Guid.Empty;
			
			lock (_connVocabulary)
			{
				cmd.Parameters["MeaningId"].Value = meaningId;
				Object obj = cmd.ExecuteScalar();
				if (obj is Guid)
					typeId = (Guid)obj;
			}

			return typeId;
		}
		
		#region IDisposable
		
		private bool _disposed = false;
		
		public void Dispose()
		{
			Dispose(true);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_closing = true;
				
				_eventPipelineFrame.Set();
				_eventPipelineCardsIn.Set();
				_eventPipelineCardsOut.Set();
				
				_threadMeanings.Join();
				_threadFrame.Join();
				_threadCardsIn.Join();
				_threadCardsOut.Join();

				if (disposing)
				{
					foreach (SqlCeCommand cmd in _commands.Values)
						cmd.Dispose();
					
					_connVocabulary.Close();
					_connProfile.Close();
				}
				
				_disposed = true;
			}
		}
		
		#endregion		
		
		private int DoGetCount(ICollection collection)
		{
			lock (collection)
			{
				return collection.Count;
			}
		}
		
		private SqlCeCommand DoGetCachedCommand(String sql)
		{
			lock (_commands)
			{
				return _commands[sql];
			}
		}
		
		private void DoUpdateMeaningWeight(Guid meaningId)
		{
			Int32 meaningWeight = 0;
			
			SqlCeCommand cmd = DoGetCachedCommand(PracticeStrings.SQL_SELECT_HISTORY);
			cmd.Parameters["MeaningId"].Value = meaningId;

			lock (_connProfile)
			{
				Object obj = cmd.ExecuteScalar();
				if (obj is Int32)
					meaningWeight = (Int32)obj;
			}
		
			lock (_meanings)
			{
				int index = _meanings.FindIndex(x => x.MeaningId == meaningId);
				MeaningData meaningData = index < 0 ? new MeaningData(meaningId, meaningWeight) : _meanings[index];

				if (meaningWeight != meaningData.Weight)
				{
					meaningData.Weight = meaningWeight;
					if (index >= 0)
					{
						_meanings.RemoveAt(index);
						index = -1;
					}
				}

				if (index < 0)
				{
					index = _meanings.BinarySearch(meaningData);

					if (index < 0)
					{
						index = ~index;
					}

					_meanings.Insert(index, meaningData);
				}
			}
		}
		
		private SqlCeCommand DoCreateCommandSelectMeaning()
		{
			lock (_connVocabulary)
			{
				return new SqlCeCommand(PracticeStrings.SQL_SELECT_MEANINGS, _connVocabulary);
			}
		}
		
		private SqlCeDataReader DoExecuteReaderSelectMeanings(SqlCeCommand cmd)
		{
			lock (_connVocabulary)
			{
				return cmd.ExecuteReader();
			}
		}
		
		private bool DoReadMeaningId(SqlCeDataReader reader, ref Guid meaningId)
		{
			lock (_connVocabulary)
			{
				bool success = reader.Read();
				if (success)
				{
					meaningId = (Guid)reader[0];
				}
				return success;
			}
		}

		private SqlCeCommand DoCreateCommandUpdateHistory()
		{
			lock (_connProfile)
			{
				SqlCeCommand cmd = new SqlCeCommand(PracticeStrings.SQL_UPDATE_HISTORY, _connProfile);
				cmd.Parameters.Add(new SqlCeParameter("VocabularyId", SqlDbType.UniqueIdentifier));
				cmd.Parameters.Add(new SqlCeParameter("MeaningId", SqlDbType.UniqueIdentifier));
				cmd.Parameters.Add(new SqlCeParameter("ActionId", SqlDbType.NVarChar));
				cmd.Prepare();
				return cmd;
			}
		}
		
		private void PipelineMeanings()
		{
			Trace.WriteLineIf(TRACE, "PipelineMeanings started");
		
			using (SqlCeCommand cmd = DoCreateCommandSelectMeaning())
			{
				using (SqlCeDataReader reader = DoExecuteReaderSelectMeanings(cmd))
				{
					Guid meaningId = Guid.Empty;
					while (!_closing && DoReadMeaningId(reader, ref meaningId))
					{
						DoUpdateMeaningWeight(meaningId);
					}
				}
			}

			Trace.WriteLineIf(TRACE, "PipelineMeanings exited");
		}
		
		private void PipelineFrame()
		{
			Trace.WriteLineIf(TRACE, "PipelineFrame started");
		
			Thread.Sleep(250);
			
			Int32 time = SLEEP_INIT;
		
			while (!_closing)
			{
				if (_eventPipelineFrame.WaitOne(time, false))
					time = SLEEP_INIT;
				else
					time = time + SLEEP_INC;

				Trace.WriteLineIf(TRACE, "PipelineFrame " + time);
				
				if (DoGetCount(_frame) > 0)
					continue;

				if (DoGetCount(_cardsIn) > 1)
					continue;

				IList<MeaningData> temp = new List<MeaningData>();

				lock (_meanings)
				{
					if (_meanings.Count == 0)
						continue;

					for (int i = 0; i < Math.Min(_meanings.Count / 10 + 1, FRAME_SIZE); ++i)
					{
						temp.Add(_meanings[i]);
					}
				}

				ListUtils.Shuffle<MeaningData>(temp);

				lock (_frame)
				{
					foreach (MeaningData data in temp)
					{
						_frame.Enqueue(data);
					}
				}
			}
			
			Trace.WriteLineIf(TRACE, "PipelineFrame exited");
		}
		
		private void PipelineCardsIn()
		{
			Trace.WriteLineIf(TRACE, "PipelineCardsIn started");
		
			Int32 time = SLEEP_INIT;
		
			while (!_closing)
			{
				if (_eventPipelineCardsIn.WaitOne(time, false))
					time = SLEEP_INIT;
				else
					time = time + SLEEP_INC;

				Trace.WriteLineIf(TRACE, "PipelineCardsIn " + time);
				
				MeaningData meaningData = null;
				lock (_frame)
				{
					if (_frame.Count == 0)
						continue;

					meaningData = _frame.Dequeue();
				}

				Guid meaningId = meaningData.MeaningId;
				FlashCard card = DoGenerate(meaningId);
				
				if (card != null)
				{
					card.Shuffle();
					
					lock (_cardsIn)
					{
						_cardsIn.Enqueue(card);
						_eventUI.Set();
					}
				}
				else
				{
					lock (_meanings)
					{
						_meanings.Remove(meaningData);
					}
				}
			}
			
			Trace.WriteLineIf(TRACE, "PipelineCardsIn exited");
		}
		
		private void PipelineCardsOut()
		{
			Trace.WriteLineIf(TRACE, "PipelineCardsOut started");

			Thread.Sleep(500);

			Int32 time = SLEEP_INIT;
		
			using (SqlCeCommand cmd = DoCreateCommandUpdateHistory())
			{
				while (!_closing)
				{
					if (_eventPipelineCardsOut.WaitOne(time, false))
						time = SLEEP_INIT;
					else
						time = time + SLEEP_INC;

					Trace.WriteLineIf(TRACE, "PipelineCardsOut " + time);
				
					FlashCard card = null;
					lock (_cardsOut)
					{
						if (_cardsOut.Count == 0)
							continue;
					
						card = _cardsOut.Dequeue();
					}

					FlashCardItem question = (FlashCardItem)card.Question;
					cmd.Parameters["VocabularyId"].Value = _vocabularyId;
					cmd.Parameters["MeaningId"].Value = question.Id;
					SqlCeParameter paramActionId = cmd.Parameters["ActionId"];

					switch (card.Status)
					{
						case FlashCardStatus.Right:
							paramActionId.Value = "R";
							break;
						case FlashCardStatus.Wrong:
							paramActionId.Value = "W";
							break;
						case FlashCardStatus.Prompt:
							paramActionId.Value = "P";
							break;
					}
				
					lock (_connProfile)
					{
						cmd.ExecuteNonQuery();
					}

					DoUpdateMeaningWeight(question.Id);
				}
			}
			
			Trace.WriteLineIf(TRACE, "PipelineCardsOut exited");
		}
	}
}
