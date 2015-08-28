// -------------------------------------------------------------------------------
//  <copyright file="LinearWorkflow.cs" company="PVM.NET Project Contributors">
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
    public class LinearWorkflow
    {
        [Test]
        public void JustOneNode()
        {
            var builder = new WorkflowDefinitionBuilder();
            bool executed = false;

            var workflowDefinition =
                builder.AddNode()
                    .WithName("start")
                    .IsStartNode()
                    .IsEndNode()
                    .BuildMockNode(e => executed = e)
                    .BuildWorkflow();

            var instance = new WorkflowEngineBuilder().Build().CreateNewInstance(workflowDefinition);
            instance.Start();

            Assert.That(executed);
            Assert.That(instance.IsFinished);
        }

        [Test]
        public void SingleStartAndEndNode()
        {
            var builder = new WorkflowDefinitionBuilder();
            bool executed = false;

            var workflowDefinition = builder
                .AddNode()
                .WithName("start")
                .IsStartNode()
                .AddTransition()
                .WithName("transition")
                .To("end")
                .BuildTransition()
                .BuildMockNode(e => executed = e)
                .AddNode()
                .WithName("end")
                .IsEndNode()
                .BuildNode()
                .BuildWorkflow();

            var instance = new WorkflowEngineBuilder().Build().CreateNewInstance(workflowDefinition);
            instance.Start();

            Assert.That(executed);
            Assert.That(instance.IsFinished);
        }
    }
}