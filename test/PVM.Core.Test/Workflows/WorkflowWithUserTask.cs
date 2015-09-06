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
using PVM.Core.Plan.Operations;
using PVM.Core.Tasks;

namespace PVM.Core.Test.Workflows
{
    [TestFixture]
    public class WorkflowWithUserTask
    {
        [Test]
        public void Executes()
        {
            var builder = new WorkflowDefinitionBuilder();
            bool executed = false;
            var taskRepository = new InMemoryTaskRepository();

            var workflowDefinition = builder
                .AddNode()
                    .WithName("start")
                    .IsStartNode()
                    .WithOperation(new UserTaskOperation("taskId", taskRepository))
                    .AddTransition()
                        .WithName("transition")
                        .To("end")
                    .BuildTransition()
                .BuildNode()
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                .BuildMockNode(e => executed = e)
                .BuildWorkflow();

            var workflowEngine = new WorkflowEngineBuilder().Build();
            var instance = workflowEngine.CreateNewInstance(workflowDefinition);
            instance.Start();

            Assert.False(executed);

            var userTask = taskRepository.FindTask("taskId");
            workflowEngine.Complete(userTask);

            Assert.That(executed);
            Assert.That(instance.IsFinished);
        }
    }
}