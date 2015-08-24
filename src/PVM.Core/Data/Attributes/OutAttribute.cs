using System;

namespace PVM.Core.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class OutAttribute : Attribute
    {
        public string Name { get; private set; }

        public OutAttribute()
        {
            
        }

        public OutAttribute(string name)
        {
            Name = name;
        }
    }
}