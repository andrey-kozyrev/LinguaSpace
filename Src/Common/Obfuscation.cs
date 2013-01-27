using System;

namespace System.Reflection
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
    public sealed class ObfuscationAttribute : Attribute
    {
        public ObfuscationAttribute()
        {
        }

        public bool Exclude 
        {
            get
            {
                return false;
            }
            set
            {
                ;
            }
        }

    }
}