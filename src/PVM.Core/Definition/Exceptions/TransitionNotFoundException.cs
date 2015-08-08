namespace PVM.Core.Definition.Exceptions
{
    public class TransitionNotFoundException : System.Exception
    {
        public TransitionNotFoundException(string message) : base(message)
        {
        }

        public TransitionNotFoundException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}