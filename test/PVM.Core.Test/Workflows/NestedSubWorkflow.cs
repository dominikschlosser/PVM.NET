// -------------------------------------------------------------------------------
//  <copyright file="NestedSubWorkflow.cs" company="PVM.NET Project Contributors">
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

using NUnit.Framework;
using PVM.Core.Builder;

namespace PVM.Core.Test.Workflows
{
    [TestFixture]
    public class NestedSubWorkflow
    {
        [Test]
        public void Executes()
        {
            var builder = new WorkflowDefinitionBuilder();
            bool executed = false;

            var workflowDefinition = builder
                .AddNode()
                .WithName("start")
                .IsStartNode()
                .AddTransition()
                .WithName("transition")
                .To("startSub")
                .BuildTransition()
                .BuildNode()
                .AddNode()
                .WithName("startSub")
                .AddTransition()
                .WithName("toEnd")
                .To("end")
                .BuildTransition()
                .BuildSubWorkflow(new WorkflowDefinitionBuilder()
                    .AddNode()
                    .WithName("subStart")
                    .IsStartNode()
                    .AddTransition()
                    .WithName("subToEnd")
                    .To("subEnd")
                    .BuildTransition()
                    .BuildNode()
                    .AddNode()
                    .WithName("subEnd")
                    .IsEndNode()
                    .BuildNode()
                    .AsDefinitionBuilder())
                .AddNode()
                .WithName("end")
                .IsEndNode()
                .BuildMockNode(e => executed = true)
                .BuildWorkflow();

            var instance = new WorkflowEngineBuilder().Build().CreateNewInstance(workflowDefinition);
            instance.Start();

            Assert.That(executed);
            Assert.That(instance.IsFinished);
        }
    }
}