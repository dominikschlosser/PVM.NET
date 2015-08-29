#region License

// -------------------------------------------------------------------------------
//  <copyright file="NinjectServiceLocatorBuilder.cs" company="PVM.NET Project Contributors">
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

using Ninject;
using Ninject.Modules;
using PVM.Core.Inject;
using PVM.Core.Persistence;
using PVM.Core.Runtime;
using PVM.Core.Serialization;

namespace PVM.Core.Builder
{
    public class NinjectServiceLocatorBuilder
    {
        private readonly IKernel kernel = new StandardKernel(new PvmModule());

        public NinjectServiceLocatorBuilder OverridePersistenceProvider<T>() where T : IPersistenceProvider
        {
            kernel.Rebind<IPersistenceProvider>().To<T>();
            return this;
        }

        public NinjectServiceLocatorBuilder OverrideObjectSerializer<T>() where T : IObjectSerializer
        {
            kernel.Rebind<IObjectSerializer>().To<T>();
            return this;
        }

        public NinjectServiceLocatorBuilder ImportModule(NinjectModule module)
        {
            kernel.Load(module);
            return this;
        }

        public WorkflowEngine Build()
        {
            return new WorkflowEngine(new NinjectServiceLocator(kernel));
        }
    }
}