using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSpace.Data
{
    public abstract class DataException : System.ApplicationException
    {
        public DataException(string sMessage)
            : base(sMessage)
        {
        }
    }

	public class BlankStringException : DataException
	{
		internal BlankStringException()
			: base("Text cannot be blank")
		{
		}
	}

    public abstract class DuplicateItemExceptionBase : DataException
    {
        internal DuplicateItemExceptionBase(String format, String text)
            : base(String.Format(format, text))
        {
        }
    }

    public class DuplicateStringException : DuplicateItemExceptionBase
    {
        internal DuplicateStringException(String str)
            : base("duplicate item '{0}'", str)
        {
        }
    }

    public class DuplicateWordException : DuplicateItemExceptionBase
    {
        internal DuplicateWordException(IWord word)
            : base("duplicate word '{0}'", word.Text)
        {
        }
    }

    public class CircularWordException : DataException
    {
        internal CircularWordException()
            : base("You cannot specify the same word")
        {
        }
    }

    public class InternalDataException : DataException
    {
        internal InternalDataException()
            : base("Internal error")
        {
        }
    }

    public class FileFormatException : DataException
    {
        internal FileFormatException(String path, String error)
            : base(String.Format("Error loading file '{0}': {1}'", path, error))
        {
        }
    }
}
