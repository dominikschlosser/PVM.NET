using System;

namespace PVM.Core.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class InAttribute : Attribute
    {
        public string Name { get; private set; }

        public InAttribute()
        {
            Name = null;
        }

        public InAttribute(string name)
        {
            Name = name;
        }
    }
}