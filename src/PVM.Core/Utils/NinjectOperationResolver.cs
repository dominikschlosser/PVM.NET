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
using Ninject;
using PVM.Core.Plan.Operations.Base;

namespace PVM.Core.Utils
{
    public class NinjectOperationResolver : IOperationResolver
    {
        private readonly IKernel ninjectKernel;

        public NinjectOperationResolver(IKernel ninjectKernel)
        {
            this.ninjectKernel = ninjectKernel;
        }

        public IOperation Resolve(string name)
        {
            var operation = ninjectKernel.Get(Type.GetType(name)) as IOperation;
            if (operation == null)
            {
                throw new InvalidOperationException(string.Format("Type '{0}' is not an operation", name));
            }

            return operation;
        }
    }
}