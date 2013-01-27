 using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace LinguaSpace.Common.IO
{
	public class FileUtils
	{
		protected FileUtils()
		{
		}

		static public void DeleteFiles(string folder, string wildcard)
		{
			string[] files = System.IO.Directory.GetFiles(folder, wildcard);
			int count = files.Length;
			for (int i = 0; i < count; ++i)
			{
				string path = files[i];
				System.IO.File.Delete(path);
			}
		}

		static public void CopyFiles(string folderSrc, string folderDst, string wildcard)
		{
			string[] files = System.IO.Directory.GetFiles(folderSrc, wildcard);
			int count = files.Length;
			for (int i = 0; i < count; ++i)
			{
				string pathSrc = files[i];
				string fileSrc = System.IO.Path.GetFileName(pathSrc);
				string pathDst = System.IO.Path.Combine(folderDst, fileSrc);
				System.IO.File.Copy(pathSrc, pathDst);
			}
		}

		static public byte[] ReadBytes(String path)
		{
			byte[] bytes = null;
			using (Stream s = File.OpenRead(path))
			{
				using (BinaryReader br = new BinaryReader(s))
				{
					bytes = br.ReadBytes((int)s.Length);
				}
			}
			return bytes;
		}

		static public void WriteBytes(String path, byte[] bytes)
		{
			using (Stream s = File.OpenWrite(path))
			{
				using (BinaryWriter bw = new BinaryWriter(s))
				{
					bw.Write(bytes);
				}
			}
		}
        
        public static String GetLastUsedFile(String folder, String mask)
        {
            String file = null;
            DateTime time = DateTime.MinValue;

            String[] files = Directory.GetFiles(folder, mask);

            foreach (String f in files)
            {
                DateTime t = File.GetLastWriteTime(f);
                if (t > time)
                {
                    file = f;
                    time = t;
                }
            }

            return file;
        }

        public static String ApplicationFolder 
        {
            get
            {
                Uri uriCodeBase = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
                String pathCodeBase = uriCodeBase.LocalPath;
                return Path.GetDirectoryName(pathCodeBase);
            }
        }

#if PLATFORM_DESKTOP
		static public String HelpFolder
		{
			get
			{
				String path = Path.Combine(ApplicationFolder, "Help");
				Debug.Assert(Directory.Exists(path));
				return path;
			}
		}
#endif

#if PLATFORM_DESKTOP
		static public String HelpFile
		{
			get
			{
				String path = Path.Combine(FileUtils.HelpFolder, "help-quickstart.html");
				Debug.Assert(File.Exists(path));
				return path;
			}
		}
#endif

	}
}
