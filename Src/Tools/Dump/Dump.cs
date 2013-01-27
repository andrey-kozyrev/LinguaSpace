using System;
using System.Diagnostics;
using System.IO;
using LinguaSpace.Data;

namespace LinguaSpace.Dump
{
	class Dumper
	{
		protected void Dump(StringCollection strings)
		{
			foreach (String s in strings)
			{
				Console.Out.Write(String.Format("{0}, ", s));
			}
			Console.Out.WriteLine();
		}

		protected void Dump(IMeaning meaning)
		{
			Dump(meaning.Translations);
			// Dump(meaning.Synonyms);
			// Dump(meaning.Antonyms);
			Console.Out.WriteLine(meaning.Example);
			Console.Out.WriteLine(meaning.Definition);
		}

		protected void Dump(IWord word)
		{
			Console.Out.WriteLine(word.Text + ", " + word.Type);
			int n = 1;
			foreach (IMeaning meaning in word.Meanings)
			{
				Console.Out.WriteLine(String.Format("{0}.", n));
				Dump(meaning);
			}
		}

		protected void Dump(ILanguage lang)
		{
			Console.Out.WriteLine(lang.InputLocale);
		}

		protected void Dump(IVocabulary voc)
		{
			Console.Out.WriteLine(voc.Name);
			Console.Out.WriteLine(voc.Description);
			Dump(voc.TargetLanguage);
			Dump(voc.NativeLanguage);
			foreach(IWord word in voc.Words)
			{
				Dump(word);
			}
		}

		protected void Dump(String path)
		{
			path = Path.GetFullPath(path);
			IVocabulary voc = new VocabularyFactory().CreateVocabulary();
			voc.Load(path);
			Console.Out.WriteLine(path);
			Dump(voc);
		}

		protected void Usage()
		{
			Console.Out.WriteLine("LinguaSpace.Dump <voc>.lsv");
		}

		protected void Run(string[] args)
		{
			if (args.Length == 1)
			{
				Dump(args[0]);
			}
			else
			{
				Usage();
			}
		}
		
		
		[STAThread]
		static void Main(string[] args)
		{
			new Dumper().Run(args);
		}
	}
}
