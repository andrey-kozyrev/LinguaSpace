using System;
using System.Diagnostics;
using LinguaSpace.Common;
    
namespace System.ComponentModel
{
    public class PropertyChangingEventArgs : EventArgs
    {
        private String propertyName;

        public PropertyChangingEventArgs(String propertyName)
        {
            Debug.Assert(!StringUtils.IsEmpty(propertyName));
            this.propertyName = propertyName;
        }
        public virtual string PropertyName 
        {
            get
            {
                return this.propertyName;
            }
        }
    }

    public delegate void PropertyChangingEventHandler(Object sender, PropertyChangingEventArgs e);
    
    public interface INotifyPropertyChanging
    {
        event PropertyChangingEventHandler PropertyChanging;
    }
}