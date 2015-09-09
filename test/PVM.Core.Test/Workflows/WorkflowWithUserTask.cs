#region License
// -------------------------------------------------------------------------------
//  <copyright file="WorkflowWithUserTask.cs" company="PVM.NET Project Contributors">
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
    public class WorkflowWithUserTask
    {
        [Test]
        public void SingleTask()
        {
            var builder = new WorkflowDefinitionBuilder();
            bool executed = false;

            var workflowDefinition = builder
                .AddNode()
                    .WithName("myTask")
                    .IsStartNode()                    
                    .AddTransition()
                        .WithName("transition")
                        .To("end")
                    .BuildTransition()
                .BuildUserTask()
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                .BuildMockNode(e => executed = e)
                .BuildWorkflow();

            var workflowEngine = new WorkflowEngineBuilder().Build();
            var instance = workflowEngine.StartNewInstance(workflowDefinition);

            Assert.False(executed);

            var userTask = workflowEngine.FindTask("myTask");
            workflowEngine.Complete(userTask);

            Assert.That(executed);
            Assert.That(instance.IsFinished);
        }

        [Test]
        public void TwoParallelTasks()
        {
            var builder = new WorkflowDefinitionBuilder();
            bool executed = false;

            var workflowDefinition = builder
                .AddNode()
                    .WithName("start")
                    .IsStartNode()
                    .AddTransition()
                        .To("task1")
                   .BuildTransition()
                   .AddTransition()
                        .To("task2")
                    .BuildTransition()
                .BuildParallelGateway()
                .AddNode()
                    .WithName("task1")
                    .AddTransition()
                        .To("join")
                    .BuildTransition()
                .BuildUserTask()
                .AddNode()
                    .WithName("task2")
                    .AddTransition()
                        .To("join")
                    .BuildTransition()
                .BuildUserTask()
                .AddNode()
                    .WithName("join")
                    .AddTransition()
                        .To("end")
                    .BuildTransition()
                .BuildParallelGateway()
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                .BuildMockNode(e => executed = e)
                .BuildWorkflow();

            var workflowEngine = new WorkflowEngineBuilder().Build();
            var instance = workflowEngine.StartNewInstance(workflowDefinition);

            Assert.False(executed);

            var userTask = workflowEngine.FindTask("task1");
            workflowEngine.Complete(userTask);

            Assert.False(executed);

            var userTask2 = workflowEngine.FindTask("task2");
            workflowEngine.Complete(userTask2);

            Assert.That(executed);
            Assert.That(instance.IsFinished);
        }
    }
}