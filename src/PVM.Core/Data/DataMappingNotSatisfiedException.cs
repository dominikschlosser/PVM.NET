using System;

namespace PVM.Core.Data
{
    public class DataMappingNotSatisfiedException : Exception
    {
        public DataMappingNotSatisfiedException(string message) : base(message)
        {
        }

        public DataMappingNotSatisfiedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}