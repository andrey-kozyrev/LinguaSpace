using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using LinguaSpace.Common.IO;
using LinguaSpace.Common.Resources;

namespace LinguaSpace.Words.IO
{
	public class FileUtils : LinguaSpace.Common.IO.FileUtils
    {
        [DllImport("shfolder.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);
        private const int MAX_PATH = 260;
        private const int CSIDL_COMMON_DOCUMENTS = 0x002E;

        static public String KeyCodeFile
        {
            get
            {
                return Path.Combine(LicenseFolder, "Key.dat");
            }
        }

        static public String DocumentsFolder
        {
            get
            {
                StringBuilder sbPath = new StringBuilder(MAX_PATH);
                SHGetFolderPath(IntPtr.Zero, CSIDL_COMMON_DOCUMENTS, IntPtr.Zero, 0, sbPath);
                String path = sbPath.ToString();
				path = Path.Combine(path, CommonStrings.COMPANY);
				path = Path.Combine(path, CommonStrings.PRODUCT);
				path = Path.Combine(path, CommonStrings.VERSION_SHORT);
                return path;
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

        static public String GetProfilePath(String name)
        {
			return System.IO.Path.Combine(FileUtils.ProfilesFolder, System.IO.Path.ChangeExtension(name, "lsp"));
        }

        static public String GetVocabularyPath(String name)
        {
			return System.IO.Path.Combine(FileUtils.VocabulariesFolder, System.IO.Path.ChangeExtension(name, "lsp"));
        }
        
        static public bool Exists(String path)
        {
			return File.Exists(path);
        }
    }
}
