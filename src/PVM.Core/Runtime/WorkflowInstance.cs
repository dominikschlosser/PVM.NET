#region License

// -------------------------------------------------------------------------------
//  <copyright file="WorkflowInstance.cs" company="PVM.NET Project Contributors">
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
using System.Collections.Generic;
using log4net;
using Microsoft.Practices.ServiceLocation;
using PVM.Core.Data.Proxy;
using PVM.Core.Definition;
using PVM.Core.Plan;

namespace PVM.Core.Runtime
{
    public class WorkflowInstance
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (WorkflowInstance));
        private readonly IExecution rootExecution;

        public WorkflowInstance(IWorkflowDefinition definition, IServiceLocator serviceLocator)
            : this(
                Guid.NewGuid().ToString(),
                new Execution(Guid.NewGuid() + "_" + definition.InitialNode.Identifier,
                    new ExecutionPlan(definition, serviceLocator)))
        {
        }

        public WorkflowInstance(String identifier, IExecution rootExecution)
        {
            Identifier = identifier;
            this.rootExecution = rootExecution;
        }

        public string Identifier { get; private set; }

        public bool IsFinished
        {
            get { return rootExecution.Plan.IsFinished; }
        }

        public void Start<T>(T data) where T : class
        {
            rootExecution.Start(rootExecution.Plan.Definition.InitialNode, DataMapper.ExtractData(data));
        }

        public void Start()
        {
            rootExecution.Start(rootExecution.Plan.Definition.InitialNode, new Dictionary<string, object>());
        }
    }
}