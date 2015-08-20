namespace PVM.Core.Runtime
{
    public class InvalidExecutionStateException : System.Exception
    {
        public InvalidExecutionStateException(string message) : base(message)
        {
        }

        public InvalidExecutionStateException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}