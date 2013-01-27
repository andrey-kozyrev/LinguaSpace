using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace LinguaSpace.Words.UI
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class CheckListBox : ListBox
    {
        public static readonly DependencyProperty SelectedItemsSourceProperty =
            DependencyProperty.Register("SelectedItemsSource", 
                                        typeof(IEnumerable),
                                        typeof(CheckListBox),
                                        new FrameworkPropertyMetadata(null, new PropertyChangedCallback(CheckListBox.OnSelectedItemsSourceChanged)));

        private static void OnSelectedItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckListBox control = (CheckListBox)d;
            IEnumerable oldValue = (IEnumerable)e.OldValue;
            IEnumerable newValue = (IEnumerable)e.NewValue;
            if ((e.NewValue == null) && !BindingOperations.IsDataBound(d, ItemsSourceProperty))
            {
                control.ClearSelectedItemsSource();
            }
            else
            {
                control.SetSelectedItemsSource(newValue);
            }
            control.OnSelectedItemsSourceChanged(oldValue, newValue);
        }

        protected virtual void OnSelectedItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            ;
        }

        private void ClearSelectedItemsSource()
        {
            this.SelectionChanged -= new SelectionChangedEventHandler(OnSelectionChanged);
            this.UnselectAll();
        }

        private void SetSelectedItemsSource(IEnumerable selectedItemsSource)
        {
            ClearSelectedItemsSource();
            this.SetSelectedItems(selectedItemsSource);
            this.SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);
        }

        public void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IList list = this.SelectedItemsSource as IList;
            if (list != null)
            {
                foreach (Object obj in e.RemovedItems)
                {
                    list.Remove(obj);
                }
                foreach (Object obj in e.AddedItems)
                {
                    list.Add(obj);
                }
            }
        }

        public IEnumerable SelectedItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(SelectedItemsSourceProperty);
            }
            set
            {
                if (value == null)
                {
                    ClearValue(SelectedItemsSourceProperty);
                }
                else
                {
                    SetValue(SelectedItemsSourceProperty, value);
                }
            }
        }
    }
}

