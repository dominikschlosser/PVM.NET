using System.Reflection;

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
    }
}