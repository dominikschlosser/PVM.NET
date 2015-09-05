#region License
// -------------------------------------------------------------------------------
//  <copyright file="DefaultOperationResolver.cs" company="PVM.NET Project Contributors">
//    Copyright (c) 2015 PVM.NET Project Contributors
//    Authors: Dominik Schlosser (dominik.schlosser@gmail.com)
//            
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -------------------------------------------------------------------------------
#endregion

using System;
using PVM.Core.Plan.Operations.Base;

namespace PVM.Core.Utils
{
    /// <summary>
    /// Default implementation of IOperationResolver. Does not resolve operation dependencies.
    /// When using other service locators than the built in one you have to implement a resolver yourself
    /// </summary>
    public class DefaultOperationResolver : IOperationResolver
    {
        public IOperation Resolve(string name)
        {
            var type = Type.GetType(name);
            if (type == null)
            {
                throw new InvalidOperationException(string.Format("Type '{0}' not found", name));
            }

            var operation = Activator.CreateInstance(type) as IOperation;
            if (operation == null)
            {
                throw new InvalidOperationException(string.Format("Type '{0}' is not an operation", name));
            }

            return operation;
        }
    }
}