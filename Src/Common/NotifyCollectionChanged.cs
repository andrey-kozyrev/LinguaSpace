using System;
using System.Diagnostics;

namespace System.Collections.Specialized
{
    public enum NotifyCollectionChangedAction
    {
        Add, Move, Remove, Replace, Reset
    }

    public class NotifyCollectionChangedEventArgs : EventArgs
    {
        public NotifyCollectionChangedEventArgs()
        {
        }

        public NotifyCollectionChangedAction Action
        {
            get
            {
                Debug.Assert(false);
                throw new NotImplementedException();
                return NotifyCollectionChangedAction.Reset;
            }
        }

        public IList NewItems
        {
            get
            {
                Debug.Assert(false);
                throw new NotImplementedException();
                return null;
            }
        }

        public IList OldItems
        {
            get
            {
                Debug.Assert(false);
                throw new NotImplementedException();
                return null;
            }
        }
    }

    public delegate void NotifyCollectionChangedEventHandler(Object sender, NotifyCollectionChangedEventArgs e);

    public interface INotifyCollectionChanged
    {
        event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}