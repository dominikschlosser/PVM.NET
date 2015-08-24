using System.Reflection;
using Castle.Core.Internal;

namespace PVM.Core.Data.Attributes
{
    public static class AttributeExtensions
    {
        public static string GetInMappingName(this PropertyInfo propertyInfo)
        {
            string name = propertyInfo.GetCustomAttribute<InAttribute>(true).Name ?? propertyInfo.Name;

            return name.ToLower();
        }

        public static string GetOutMappingName(this PropertyInfo propertyInfo)
        {
            string name = propertyInfo.GetCustomAttribute<OutAttribute>(true).Name ?? propertyInfo.Name;

            return name.ToLower();
        }

        public static bool HasInMapping(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetAttribute<InAttribute>() != null;
        }

        public static bool HasOutMapping(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetAttribute<OutAttribute>() != null;
        }
    }
}