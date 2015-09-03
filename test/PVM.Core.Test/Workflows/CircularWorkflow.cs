#region License

// -------------------------------------------------------------------------------
//  <copyright file="CircularWorkflow.cs" company="PVM.NET Project Contributors">
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

using log4net;
using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Core.Data.Attributes;
using PVM.Core.Plan.Operations.Base;
using PVM.Core.Runtime;

namespace PVM.Core.Test.Workflows
{
    [TestFixture]
    public class CircularWorkflow
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (CircularWorkflow));

        private class CounterGateway : DataAwareOperation<TestData>
        {
            public override void Execute(IExecution e, TestData dataContext)
            {
                if (dataContext.Counter == 1)
                {
                    Logger.Info("COUNTER == 1");
                    e.Proceed("intermediateToJoin");
                }
                else
                {
                    Logger.Info("COUNTER == " + dataContext.Counter);
                    dataContext.Counter++;
                    e.Proceed("intermediateToStart");
                }
            }
        }

        [WorkflowData]
        public class StartData
        {
            [Out]
            public int Counter { get; set; }

            [Out]
            public string Text { get; set; }
        }

        [WorkflowData]
        public class TestData
        {
            private int counter;

            [In, Out]
            public virtual int Counter
            {
                get { return counter; }
                set { counter = value + 2; }
            }
        }

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
                        .WithName("toJoin")
                        .To("join")
                    .BuildTransition()
                    .AddTransition()
                        .WithName("toIntermediate")
                        .To("intermediate")
                    .BuildTransition()
                .BuildParallelSplit()
                .AddNode()
                    .WithName("intermediate")
                    .WithOperation(new CounterGateway())
                    .AddTransition()
                        .WithName("intermediateToJoin")
                        .To("join")
                    .BuildTransition()
                    .AddTransition()
                        .WithName("intermediateToStart")
                        .To("start")
                    .BuildTransition()
                .BuildNode()
                .AddNode()
                    .WithName("join")
                    .AddTransition()
                        .WithName("endTransition")
                        .To("end")
                    .BuildTransition()
                .BuildParallelJoin()
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                .BuildMockNode(e => executed = e)
                .BuildWorkflow<TestData>();

            var instance = new WorkflowEngineBuilder().Build().CreateNewInstance(workflowDefinition);
            instance.Start(new StartData {Counter = -5});

            Assert.That(executed);
            Assert.That(instance.IsFinished);
        }
    }
}