using System;

namespace PVM.Core.Utils
{
    public class ContractException : Exception
    {
        public ContractException(string message) : base(message)
        {
        }

        public ContractException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}