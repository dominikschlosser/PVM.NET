// -------------------------------------------------------------------------------
//  <copyright file="DataInterceptor.cs" company="PVM.NET Project Contributors">
//    Copyright (c) 2015 PVM.NET Project Contributors
//    Authors: Dominik Schlosser (dominik.schlosser@gmail.com)
//            
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//    	http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
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

            if (setter != null && setter.HasAttribute<OutAttribute>())
            {
                string mappingName = setter.GetOutMappingName();

                invocation.Proceed();
                if (setter.GetGetMethod(true) != null)
                {
                    data[mappingName] = setter.GetValue(ProxyUtil.GetUnproxiedInstance(invocation.InvocationTarget));
                }
                else
                {
                    data[mappingName] = invocation.GetArgumentValue(0);
                }
            }

            PropertyInfo getter =
                invocation.Method.DeclaringType.GetProperties()
                    .FirstOrDefault(p => p.GetGetMethod() == invocation.Method);

            if (getter != null && getter.HasAttribute<InAttribute>())
            {
                string mappingName = getter.GetInMappingName();

                invocation.ReturnValue = data[mappingName];
            }
        }
    }
}