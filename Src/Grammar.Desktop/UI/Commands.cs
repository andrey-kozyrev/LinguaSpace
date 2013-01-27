using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace LinguaSpace.Grammar.UI
{
    [System.Reflection.Obfuscation(Exclude = true)]
	public sealed class GrammarCommands
	{
		public readonly static RoutedUICommand ToggleActive = new RoutedUICommand("_Toggle Active...", "ToggleActive", typeof(GrammarCommands));

		static GrammarCommands()
		{
			ToggleActive.InputGestures.Add(new KeyGesture(Key.Space));
		}
	}
}
