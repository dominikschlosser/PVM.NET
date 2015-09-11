#region License

// -------------------------------------------------------------------------------
//  <copyright file="ParallelWorkflow.cs" company="PVM.NET Project Contributors">
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

namespace PVM.Core.Test.Workflows
{
    [TestFixture]
    public class ParallelWorkflow
    {
        [Test]
        public void SingleBranch_ExecutesNodeAfterJoin()
        {
            var builder = new WorkflowDefinitionBuilder();
            bool executed = false;

            var workflowDefinition = builder
                .AddNode()
                    .WithName("split")
                    .IsStartNode()
                 .AddTransition()
                        .WithName("transition1")
                        .To("subNode1")
                    .BuildTransition()
                    .AddTransition()
                        .WithName("transition2")
                        .To("subNode2")
                    .BuildTransition()
                .BuildParallelGateway()
                .AddNode()
                    .WithName("subNode1")
                    .AddTransition()
                        .WithName("subNodeToEnd")
                        .To("join")
                    .BuildTransition()
                .BuildNode()
                .AddNode()
                    .WithName("subNode2")
                    .AddTransition()
                        .WithName("subNode2ToEnd")
                        .To("join")
                    .BuildTransition()
                .BuildNode()
                .AddNode()
                    .WithName("join")
                    .AddTransition()
                        .WithName("joinToEnd")
                        .To("end")
                    .BuildTransition()
                .BuildParallelGateway()
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                .BuildMockNode(e => executed = e)
                .BuildWorkflow();

            var instance = new WorkflowEngineBuilder().Build().StartNewInstance(workflowDefinition);

            Assert.That(executed);
            Assert.That(instance.IsFinished);
        }

        [Test]
        public void WithExplicitSplitAndJoinOperations()
        {
            var builder = new WorkflowDefinitionBuilder();
            bool executed = false;

            var workflowDefinition = builder
                .AddNode()
                    .WithName("split")
                    .IsStartNode()
                 .AddTransition()
                        .WithName("transition1")
                        .To("subNode1")
                    .BuildTransition()
                    .AddTransition()
                        .WithName("transition2")
                        .To("subNode2")
                    .BuildTransition()
                .BuildParallelGateway()
                .AddNode()
                    .WithName("subNode1")
                    .AddTransition()
                        .WithName("subNodeToEnd")
                        .To("join")
                    .BuildTransition()
                .BuildNode()
                .AddNode()
                    .WithName("subNode2")
                    .AddTransition()
                        .WithName("subNode2ToEnd")
                        .To("join")
                    .BuildTransition()
                .BuildNode()
                .AddNode()
                    .WithName("join")
                    .AddTransition()
                        .WithName("joinToEnd")
                        .To("end")
                    .BuildTransition()
                .BuildParallelGateway()
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                .BuildMockNode(e => executed = e)
                .BuildWorkflow();

            var instance = new WorkflowEngineBuilder().Build().StartNewInstance(workflowDefinition);

            Assert.That(executed);
            Assert.That(instance.IsFinished);
        }
    }
}