using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using PVM.Core.Data.Attributes;

namespace PVM.Core.Data.Proxy
{
    public class DataInterceptor : IInterceptor
    {
        private readonly IDictionary<String, object> data;

        public DataInterceptor(IDictionary<string, object> data)
        {
            this.data = data;
        }

        public void Intercept(IInvocation invocation)
        {
            PropertyInfo setter =
                invocation.Method.DeclaringType.GetProperties()
                    .FirstOrDefault(p => p.GetSetMethod() == invocation.Method);

            if (setter != null && setter.HasOutMapping())
            {
                string mappingName = setter.GetOutMappingName();

                data[mappingName] = invocation.GetArgumentValue(0);
            }

            PropertyInfo getter =
                invocation.Method.DeclaringType.GetProperties()
                    .FirstOrDefault(p => p.GetGetMethod() == invocation.Method);

            if (getter != null && getter.HasInMapping())
            {
                string mappingName = getter.GetInMappingName();

                invocation.ReturnValue = data[mappingName];
            }
        }
    }
}