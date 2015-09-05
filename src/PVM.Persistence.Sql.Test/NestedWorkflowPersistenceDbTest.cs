#region License
// -------------------------------------------------------------------------------
//  <copyright file="NestedProcessPersistenceTest.cs" company="PVM.NET Project Contributors">
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

using System.Linq;
using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Core.Plan.Operations.Base;
using PVM.Core.Runtime;

namespace PVM.Persistence.Sql.Test
{
    public class NestedWorkflowPersistenceDbTest : DbTestBase
    {
        public class TestOperation : IOperation
        {
            public void Execute(IExecution execution)
            {
                execution.Wait("signal");
            }
        }

        [Test]
        public void PersistNestedWorkflow()
        {
            var builder = new WorkflowDefinitionBuilder();

            var workflowDefinition = builder
                .WithIdentifier("testWorkflowDefinition")
                .AddNode()
                    .WithName("start")
                    .WithOperation(new TestOperation())
                    .IsStartNode()
                    .AddTransition()
                        .WithName("transition")
                        .To("nested")
                    .BuildTransition()
                .BuildNode()
                .AddNode()
                    .WithName("nested")
                    .AddTransition()
                        .WithName("nestedToEnd")
                        .To("end")
                    .BuildTransition()
                .BuildSubWorkflow(new WorkflowDefinitionBuilder()
                    .WithIdentifier("subWorkflowDefinition")
                    .AddNode()
                        .IsStartNode()
                        .IsEndNode()
                        .WithName("subWorkflowNode")
                        .WithOperation(new TestOperation())
                    .BuildNode()
                    .AsDefinitionBuilder())
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                .BuildNode()
                .BuildWorkflow();

            var instance =
                new WorkflowEngineBuilder().ConfigureServiceLocator()
                                           .OverridePersistenceProvider<SqlPersistenceProvider>()
                                           .Build()
                                           .CreateNewInstance(workflowDefinition);

            instance.Start();

            Assert.That(TestDbContext.WorkflowDefinitions.Any(d => d.Identifier == "nested"));
        }
    }
}