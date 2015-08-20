namespace PVM.Core.Utils
{
    public static class ContractExtensions
    {
        public static void RequireNotNull<T>(this T obj, string message)
        {
            if (obj == null)
            {
                throw new ContractException(message);
            }
        }
    }
}