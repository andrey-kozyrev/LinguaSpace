﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LinguaSpace.Words.Practice.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class PracticeStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal PracticeStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LinguaSpace.Words.Practice.Resources.PracticeStrings", typeof(PracticeStrings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {4AEA9869-B312-4177-BDBB-3088F0410C5D}.
        /// </summary>
        internal static string PASSWORD_PROFILE {
            get {
                return ResourceManager.GetString("PASSWORD_PROFILE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {F87B1C75-0486-4cec-ACBC-7B158DD75B4B}.
        /// </summary>
        internal static string PASSWORD_VOCABULARY {
            get {
                return ResourceManager.GetString("PASSWORD_VOCABULARY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT m.MeaningId, m.Prefix AS UsagePrefix, rw.Prefix, rw.Word, m.Postfix AS UsagePostfix
        ///FROM Meanings AS m INNER JOIN Relations AS r ON m.MeaningId = r.MeaningId 
        ///		   INNER JOIN Words AS rw ON r.WordId = rw.WordId
        ///WHERE m.MeaningId = @MeaningId AND r.Relation = &apos;A&apos; AND r.SyncLive = 1
        ///ORDER BY r.Position.
        /// </summary>
        internal static string SQL_ANSWER_ANTONYMS {
            get {
                return ResourceManager.GetString("SQL_ANSWER_ANTONYMS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT m.MeaningId, m.Definition
        ///FROM Meanings AS m
        ///WHERE m.MeaningId = @MeaningId AND m.SyncLive = 1 AND m.Definition IS NOT NULL AND LEN(m.Definition) &gt; 0.
        /// </summary>
        internal static string SQL_ANSWER_DEFINITION {
            get {
                return ResourceManager.GetString("SQL_ANSWER_DEFINITION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT m.MeaningId, m.Prefix AS UsagePrefix, rw.Prefix, rw.Word, m.Postfix AS UsagePostfix
        ///FROM Meanings AS m INNER JOIN Relations AS r ON m.MeaningId = r.MeaningId 
        ///		   INNER JOIN Words AS rw ON r.WordId = rw.WordId
        ///WHERE m.MeaningId = @MeaningId AND r.Relation = &apos;S&apos; AND r.SyncLive = 1
        ///ORDER BY r.Position.
        /// </summary>
        internal static string SQL_ANSWER_SYNONYMS {
            get {
                return ResourceManager.GetString("SQL_ANSWER_SYNONYMS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT m.MeaningId,  t.Translation 
        ///FROM Meanings AS m INNER JOIN Translations AS t ON m.MeaningId = t.MeaningId
        ///WHERE m.MeaningId = @MeaningId AND t.SyncLive = 1
        ///ORDER BY t.Position.
        /// </summary>
        internal static string SQL_ANSWER_TRANSLATIONS {
            get {
                return ResourceManager.GetString("SQL_ANSWER_TRANSLATIONS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT m.MeaningId, m.Prefix AS UsagePrefix, w.Prefix, w.Word, m.Postfix AS UsagePostfix
        ///FROM Meanings AS m INNER JOIN Words AS w ON m.WordId = w.WordId
        ///WHERE m.MeaningId = @MeaningId.
        /// </summary>
        internal static string SQL_ANSWER_WORD {
            get {
                return ResourceManager.GetString("SQL_ANSWER_WORD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT m.MeaningId, m.Definition
        ///FROM Meanings AS m INNER JOIN Words AS w ON m.WordId = w.WordId
        ///WHERE w.TypeId = @TypeId AND m.MeaningId &gt; @SeedId AND m.SyncLive = 1 AND m.Definition IS NOT NULL AND LEN(m.Definition) &gt; 0.
        /// </summary>
        internal static string SQL_ANSWERS_DEFINITION {
            get {
                return ResourceManager.GetString("SQL_ANSWERS_DEFINITION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT m.MeaningId, m.Prefix AS UsagePrefix, rw.Prefix, rw.Word, m.Postfix AS UsagePostfix
        ///FROM Words AS rw INNER JOIN Relations AS r ON rw.WordId = r.WordId 
        ///INNER JOIN Meanings AS m ON r.MeaningId = m.MeaningId
        ///INNER JOIN Words AS w ON m.WordId = w.WordId
        ///WHERE w.TypeId = @TypeId AND m.MeaningId &gt; @SeedId AND r.SyncLive = 1 AND m.SyncLive = 1 AND w.SyncLive = 1
        ///ORDER BY m.MeaningId, r.Relation, r.Position.
        /// </summary>
        internal static string SQL_ANSWERS_RELATIONS {
            get {
                return ResourceManager.GetString("SQL_ANSWERS_RELATIONS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT m.MeaningId, t.Translation
        ///FROM Translations AS t INNER JOIN Meanings AS m ON t.MeaningId = m.MeaningId
        ///INNER JOIN Words AS w ON m.WordId = w.WordId
        ///WHERE w.TypeId = @TypeId AND m.MeaningId &gt; @SeedId AND t.SyncLive = 1 AND m.SyncLive = 1 AND w.SyncLive = 1
        ///ORDER BY m.MeaningId, t.Position.
        /// </summary>
        internal static string SQL_ANSWERS_TRANSLATIONS {
            get {
                return ResourceManager.GetString("SQL_ANSWERS_TRANSLATIONS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT m.MeaningId, m.Prefix AS UsagePrefix, w.Prefix, w.Word, m.Postfix AS UsagePostfix
        ///FROM Meanings AS m INNER JOIN Words AS w ON m.WordId = w.WordId
        ///WHERE w.TypeId = @TypeId AND m.MeaningId &gt; @SeedId AND m.SyncLive = 1.
        /// </summary>
        internal static string SQL_ANSWERS_WORD {
            get {
                return ResourceManager.GetString("SQL_ANSWERS_WORD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT h.MeaningId, SUM(365000 * a.Weight / (1 + ABS(DATEDIFF(d, h.SyncMod, GETDATE())))) AS Score
        ///FROM History AS h
        ///INNER JOIN Actions AS a ON h.ActionId = a.ActionId 
        ///WHERE (h.VocabularyId = @Vocabularyid) AND (h.SyncMod &gt; @SyncMod)
        ///GROUP BY h.MeaningId
        ///ORDER BY Score DESC.
        /// </summary>
        internal static string SQL_HISTORY {
            get {
                return ResourceManager.GetString("SQL_HISTORY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT COUNT(*) 
        ///FROM Meanings AS m
        ///WHERE m.MeaningId = @MeaningId AND SyncLive = 1.
        /// </summary>
        internal static string SQL_MEANING_CHECK {
            get {
                return ResourceManager.GetString("SQL_MEANING_CHECK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT m.MeaningId, m.Definition, m.Example
        ///FROM Meanings AS m
        ///WHERE m.MeaningId = @MeaningId AND m.SyncLive = 1.
        /// </summary>
        internal static string SQL_QUESTION_DEFINITION {
            get {
                return ResourceManager.GetString("SQL_QUESTION_DEFINITION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT m.MeaningId, t.Translation, m.Example
        ///FROM Meanings AS m INNER JOIN Translations AS t ON m.MeaningId = t.MeaningId
        ///WHERE m.MeaningId = @MeaningId AND t.SyncLive = 1 AND m.SyncLive = 1.
        /// </summary>
        internal static string SQL_QUESTION_TRANSLATIONS {
            get {
                return ResourceManager.GetString("SQL_QUESTION_TRANSLATIONS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT m.MeaningId, m.Prefix AS UsagePrefix, w.Prefix, w.Word, m.Example, m.Postfix AS UsagePostfix
        ///FROM Words AS w INNER JOIN Meanings AS m ON w.WordId = m.WordId
        ///WHERE m.MeaningId = @MeaningId AND m.SyncLive = 1.
        /// </summary>
        internal static string SQL_QUESTION_WORD {
            get {
                return ResourceManager.GetString("SQL_QUESTION_WORD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT SUM(365000 * a.Weight / (1 + ABS(DATEDIFF(d, h.SyncMod, GETDATE())))) AS Score
        ///FROM History AS h
        ///INNER JOIN Actions AS a ON h.ActionId = a.ActionId
        ///WHERE (h.MeaningId = @MeaningId)
        ///GROUP BY h.MeaningId.
        /// </summary>
        internal static string SQL_SELECT_HISTORY {
            get {
                return ResourceManager.GetString("SQL_SELECT_HISTORY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT m.MeaningId
        ///FROM Meanings AS m
        ///WHERE m.SyncLive = 1
        ///ORDER BY m.SyncMod DESC.
        /// </summary>
        internal static string SQL_SELECT_MEANINGS {
            get {
                return ResourceManager.GetString("SQL_SELECT_MEANINGS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT w.TypeId 
        ///FROM Words AS w INNER JOIN Meanings AS m ON w.WordId = m.WordId
        ///WHERE m.MeaningId = @MeaningId.
        /// </summary>
        internal static string SQL_TYPE_ID {
            get {
                return ResourceManager.GetString("SQL_TYPE_ID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO History (VocabularyId, MeaningId, ActionId)
        ///VALUES (@VocabularyId, @MeaningId, @ActionId).
        /// </summary>
        internal static string SQL_UPDATE_HISTORY {
            get {
                return ResourceManager.GetString("SQL_UPDATE_HISTORY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT v.VocabularyId 
        ///FROM Vocabularies AS v.
        /// </summary>
        internal static string SQL_VOCABULARY_ID {
            get {
                return ResourceManager.GetString("SQL_VOCABULARY_ID", resourceCulture);
            }
        }
    }
}
