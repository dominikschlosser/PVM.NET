using System;

namespace PVM.Core.Definition
{
    public class WorkflowValidationException : Exception
    {
        public WorkflowValidationException(string message) : base(message)
        {
        }

        public WorkflowValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}