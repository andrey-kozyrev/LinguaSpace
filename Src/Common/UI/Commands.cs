using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace LinguaSpace.Common.UI
{
    [System.Reflection.Obfuscation(Exclude = true)]
	public sealed class FileCommands
	{
		public readonly static RoutedUICommand New = new RoutedUICommand("_New...", "New", typeof(FileCommands));
		public readonly static RoutedUICommand Open = new RoutedUICommand("_Open...", "Open", typeof(FileCommands));
		public readonly static RoutedUICommand Properties = new RoutedUICommand("_Properties...", "Properties", typeof(FileCommands));
		public readonly static RoutedUICommand Save = new RoutedUICommand("_Save", "Save", typeof(FileCommands));
		public readonly static RoutedUICommand SaveAs = new RoutedUICommand("Save _As...", "SaveAs", typeof(FileCommands));
		public readonly static RoutedUICommand Close = new RoutedUICommand("_Close", "Close", typeof(FileCommands));
		public readonly static RoutedUICommand Exit = new RoutedUICommand("E_xit", "Exit", typeof(FileCommands));

		static FileCommands()
		{
			New.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
			Open.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
			Properties.InputGestures.Add(new KeyGesture(Key.F4));
			Save.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
			SaveAs.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control|ModifierKeys.Shift));
			Exit.InputGestures.Add(new KeyGesture(Key.F4, ModifierKeys.Alt));
		}
	}

    [System.Reflection.Obfuscation(Exclude = true)]
	public sealed class EditCommands
	{
		public readonly static RoutedUICommand New = new RoutedUICommand("_New...", "New", typeof(EditCommands));
		public readonly static RoutedUICommand NewChild = new RoutedUICommand("_New Child...", "NewChild", typeof(EditCommands));
		public readonly static RoutedUICommand Edit = new RoutedUICommand("_Edit...", "Edit", typeof(EditCommands));
		public readonly static RoutedUICommand Delete = new RoutedUICommand("_Delete", "Delete", typeof(EditCommands));
        public readonly static RoutedUICommand Highlight = new RoutedUICommand("_Highlight", "Highlight", typeof(EditCommands));
		public readonly static RoutedUICommand MoveUp = new RoutedUICommand("Move _Up", "MoveUp", typeof(EditCommands));
		public readonly static RoutedUICommand MoveDown = new RoutedUICommand("Move Do_wn", "MoveDown", typeof(EditCommands));
        public readonly static RoutedUICommand Touch = new RoutedUICommand("_Touch", "Touch", typeof(EditCommands));
        public readonly static RoutedUICommand FindWord = new RoutedUICommand("_Find Word...", "FindWord", typeof(EditCommands));
        public readonly static RoutedUICommand FindMeaning = new RoutedUICommand("_Find Translation...", "FindTranslation", typeof(EditCommands));
		public readonly static RoutedUICommand Filter = new RoutedUICommand("Filter...", "Filter", typeof(EditCommands));

		public readonly static RoutedUICommand Cut = new RoutedUICommand("_Cut", "Cut", typeof(EditCommands));
		public readonly static RoutedUICommand Copy = new RoutedUICommand("_Copy", "Copy", typeof(EditCommands));
		public readonly static RoutedUICommand Paste = new RoutedUICommand("_Paste", "Paste", typeof(EditCommands));

		static EditCommands()
		{
			New.InputGestures.Add(new KeyGesture(Key.Insert));
			NewChild.InputGestures.Add(new KeyGesture(Key.Insert, ModifierKeys.Control));
			Edit.InputGestures.Add(new KeyGesture(Key.Enter, ModifierKeys.Alt));
			Edit.InputGestures.Add(new KeyGesture(Key.F2));
			Edit.InputGestures.Add(new MouseGesture(MouseAction.LeftDoubleClick));
			Delete.InputGestures.Add(new KeyGesture(Key.Delete));
			MoveDown.InputGestures.Add(new KeyGesture(Key.Down, ModifierKeys.Control));
			MoveUp.InputGestures.Add(new KeyGesture(Key.Up, ModifierKeys.Control));
            Touch.InputGestures.Add(new KeyGesture(Key.Space));
			FindWord.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control));
            FindMeaning.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control|ModifierKeys.Shift));
			Highlight.InputGestures.Add(new KeyGesture(Key.H, ModifierKeys.Control));
			Cut.InputGestures.Add(new KeyGesture(Key.X, ModifierKeys.Control));
			Copy.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control));
			Paste.InputGestures.Add(new KeyGesture(Key.V, ModifierKeys.Control));
		}
	}

    [System.Reflection.Obfuscation(Exclude = true)]
	public sealed class NavigateCommands
	{
		public readonly static RoutedUICommand Back = new RoutedUICommand("_Back", "Back", typeof(NavigateCommands));
		public readonly static RoutedUICommand Forward = new RoutedUICommand("_Forward", "Forward", typeof(NavigateCommands));
        public readonly static RoutedUICommand GoToWord = new RoutedUICommand("Go to word", "GoToWord", typeof(NavigateCommands));

		static NavigateCommands()
		{
			Back.InputGestures.Add(new KeyGesture(Key.Left, ModifierKeys.Alt));
			Forward.InputGestures.Add(new KeyGesture(Key.Right, ModifierKeys.Alt));
		}
	}

    [System.Reflection.Obfuscation(Exclude = true)]
	public sealed class UserCommands
	{
		public readonly static RoutedUICommand New = new RoutedUICommand("_New...", "New", typeof(UserCommands));
		public readonly static RoutedUICommand Settings = new RoutedUICommand("S_ettings...", "Settings", typeof(UserCommands));
		public readonly static RoutedUICommand Switch = new RoutedUICommand("_Switch...", "Switch", typeof(UserCommands));

		static UserCommands()
		{
			Settings.InputGestures.Add(new KeyGesture(Key.F10));
		}
	}

    [System.Reflection.Obfuscation(Exclude = true)]
	public sealed class ExerciseCommands
    {
		public readonly static RoutedUICommand FlashCards = new RoutedUICommand("_Flash Cards...", "FlashCards", typeof(ExerciseCommands));
		public readonly static RoutedUICommand Practice = new RoutedUICommand("_Practice...", "Practice", typeof(ExerciseCommands));

        static ExerciseCommands()
        {
            FlashCards.InputGestures.Add(new KeyGesture(Key.F5));
			Practice.InputGestures.Add(new KeyGesture(Key.F5));
        }
    }

	[System.Reflection.Obfuscation(Exclude = true)]
	public sealed class MobileCommands
	{
		public readonly static RoutedUICommand Explore = new RoutedUICommand("_Explore...", "Explore", typeof(MobileCommands));
		public readonly static RoutedUICommand Synchronize = new RoutedUICommand("_Synchronize", "Synchronize", typeof(MobileCommands));

		public readonly static RoutedUICommand CopyFromDesktopToDevice = new RoutedUICommand("Copy from Desktop to Device", "CopyFromDesktopToDevice", typeof(MobileCommands));
		public readonly static RoutedUICommand CopyFromDeviceToDesktop = new RoutedUICommand("Copy from Device to Desktop", "CopyFromDeviceToDesktop", typeof(MobileCommands));

		static MobileCommands()
		{
			CopyFromDesktopToDevice.InputGestures.Add(new KeyGesture(Key.Right, ModifierKeys.Control));
			CopyFromDeviceToDesktop.InputGestures.Add(new KeyGesture(Key.Left, ModifierKeys.Control));
		}
	}


    [System.Reflection.Obfuscation(Exclude = true)]
	public sealed class FlashCardsCommands
	{
		public readonly static RoutedUICommand Next = new RoutedUICommand("_Next", "Next", typeof(FlashCardsCommands));
		public readonly static RoutedUICommand Answer = new RoutedUICommand("_Answer", "Answer", typeof(FlashCardsCommands));
		public readonly static RoutedUICommand Prompt = new RoutedUICommand("_Prompt", "Prompt", typeof(FlashCardsCommands));
		public readonly static RoutedUICommand Sleep = new RoutedUICommand("_Sleep", "Sleep", typeof(FlashCardsCommands));
		public readonly static RoutedUICommand Stop = new RoutedUICommand("S_top", "Stop", typeof(FlashCardsCommands));
		public readonly static RoutedUICommand Skip = new RoutedUICommand("S_kip", "Skip", typeof(FlashCardsCommands));

		static FlashCardsCommands()
		{
		}
	}

    [System.Reflection.Obfuscation(Exclude = true)]
	public sealed class HelpCommands
	{
		public readonly static RoutedUICommand QuickStart = new RoutedUICommand("_Quick Start", "QuickStart", typeof(HelpCommands));
		public readonly static RoutedUICommand Register = new RoutedUICommand("_Register...", "Register", typeof(HelpCommands));
		public readonly static RoutedUICommand About = new RoutedUICommand("_About LinguaSpace...", "About", typeof(HelpCommands));

		static HelpCommands()
		{
			QuickStart.InputGestures.Add(new KeyGesture(Key.F1));
		}
	}

    [System.Reflection.Obfuscation(Exclude = true)]
	public sealed class DialogCommands
    {
        public readonly static RoutedUICommand OK = new RoutedUICommand("OK", "OK", typeof(DialogCommands));
        public readonly static RoutedUICommand Cancel = new RoutedUICommand("Cancel", "Cancel", typeof(DialogCommands));

        static DialogCommands()
        {
        }
    }

    [System.Reflection.Obfuscation(Exclude = true)]
    public sealed class ShellCommands
    {
        public readonly static RoutedUICommand Open = new RoutedUICommand("Open", "Open", typeof(ShellCommands));
    }
}
