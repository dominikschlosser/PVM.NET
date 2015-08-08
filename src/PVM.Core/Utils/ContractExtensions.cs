using System.Runtime.InteropServices.ComTypes;
using PVM.Core.Definition;
using PVM.Core.Definition.Exception;

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

        public static void RequireActive(this IExecution execution)
        {
            if (!execution.IsActive)
            {
                throw new ExecutionInactiveException($"Execution '{execution.Identifier}' is inactive.");
            }
        }
    }
}