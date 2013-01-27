using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Diagnostics;

namespace LinguaSpace.Common.UI
{
    public static class DragDropUtils
    {
        public const String FILE_DROP = "FileDrop";
        public const String FILE_NAME_W = "FileNameW";
        public const String FILE_NAME = "FileName";

        public static String GetFileName(IDataObject data)
        {
            Debug.Assert(data != null);
            String fileName = String.Empty;
            if (data.GetDataPresent(FILE_DROP))
            {
                String[] fileNames = (String[])data.GetData(FILE_DROP);
                if (fileNames.Length > 0)
                {
                    fileName = fileNames[0];
                }
            }
            else if (data.GetDataPresent(FILE_NAME_W))
            {
                fileName = (String)data.GetData(FILE_NAME_W);
            }
            else if (data.GetDataPresent(FILE_NAME))
            {
                fileName = (String)data.GetData(FILE_NAME);
            }
            return fileName;
        }

        public static void PutFileName(IDataObject data, String fileName)
        {
            Debug.Assert(data != null);
            Debug.Assert(fileName != null);
            data.SetData(FILE_DROP, fileName);
            data.SetData(FILE_NAME_W, fileName);
            data.SetData(FILE_NAME, fileName);
        }
    }
}
