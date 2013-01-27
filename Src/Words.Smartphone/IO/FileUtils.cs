using System;
using System.IO;
using System.Diagnostics;
using LinguaSpace.Common;
using LinguaSpace.Common.Resources;

namespace LinguaSpace.Words.UI
{
    class FileUtils : LinguaSpace.Common.IO.FileUtils
    {
        static public String DocumentsFolder
        {
            get
            {
				return ApplicationFolder;
			}
		}

		static public String VocabulariesFolder
		{
			get
			{
				String path = DocumentsFolder;
				path = Path.Combine(path, "Vocabularies");
				Directory.CreateDirectory(path);
				return path;
			}
		}

		static public String ProfilesFolder
		{
			get
			{
				String path = DocumentsFolder;
				path = Path.Combine(path, "Profiles");
				Directory.CreateDirectory(path);
				return path;
			}
		}

		static public String LicenseFolder
		{
			get
			{
				String path = DocumentsFolder;
				path = Path.Combine(path, "License");
				Directory.CreateDirectory(path);
				return path;
			}
		}
	}
}

