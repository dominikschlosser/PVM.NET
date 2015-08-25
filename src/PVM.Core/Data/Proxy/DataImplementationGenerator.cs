using System;
using System.Collections.Generic;
using Castle.DynamicProxy;

namespace PVM.Core.Data.Proxy
{
    public class DataImplementationGenerator
    {
        public static object CreateInstanceFor(Type type, IDictionary<string, object> data)
        {
            var generator = new ProxyGenerator();
            return generator.CreateInterfaceProxyWithoutTarget(type, new DataInterceptor(data));
        } 
    }
}