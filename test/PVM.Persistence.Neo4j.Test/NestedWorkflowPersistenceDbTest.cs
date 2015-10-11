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

using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Core.Runtime;
using PVM.Core.Runtime.Operations.Base;
using PVM.Persistence.Sql.Test;

namespace PVM.Persistence.Neo4j.Test
{
    public class NestedWorkflowPersistenceDbTest : Neo4jTestBase
    {
        public class TestOperation : IOperation
        {
            public void Execute(IExecution execution)
            {
                execution.Wait();
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
                    .WithOperation<TestOperation>()
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
                        .WithOperation<TestOperation>()
                    .BuildNode()
                    .AsDefinitionBuilder())
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                .BuildNode()
                .BuildWorkflow();

            var instance =
                new WorkflowEngineBuilder().ConfigureServiceLocator()
                                           .ImportModule(new Neo4jPersistenceTestModule(GraphClient))
                                           .Build()
                                           .StartNewInstance(workflowDefinition);


        }
    }
}