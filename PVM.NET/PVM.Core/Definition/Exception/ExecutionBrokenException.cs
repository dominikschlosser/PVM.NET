namespace PVM.Core.Definition.Exception
{
    public class ExecutionBrokenException : System.Exception
    {
        public ExecutionBrokenException(string message) : base(message)
        {
        }

        public ExecutionBrokenException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}