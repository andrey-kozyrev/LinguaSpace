using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace System.Collections.ObjectModel
{
    public class ObservableCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public ObservableCollection()
        {
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            throw new NotImplementedException();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
