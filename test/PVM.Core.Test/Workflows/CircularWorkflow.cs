﻿using log4net;
using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Core.Plan;
using System.Collections.Generic;
using PVM.Core.Data.Attributes;
using PVM.Core.Plan.Operations;
using PVM.Core.Runtime;

namespace PVM.Core.Test.Workflows
{
    [TestFixture]
    public class CircularWorkflow
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (CircularWorkflow));

        [Test]
        public void Executes()
        {
            var builder = new WorkflowDefinitionBuilder();
            bool executed = false;

            var workflowDefinition = builder.AddNode()
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
               .BuildWorkflow();

            var instance = new WorkflowInstance(workflowDefinition);

            // todo: how to define process in/output?
            var testdata = new Dictionary<string, object>();
            testdata.Add("counter", 0);
            instance.Start(testdata);

            Assert.That(executed);
            Assert.That(instance.IsFinished);
        }

        private class CounterGateway : IOperation<TestData>
        {
            public void Execute(IExecution e, TestData dataContext)
            {
                if (dataContext.Counter == 1)
                {
                    Logger.Info("COUNTER == 1");
                    e.Proceed("intermediateToJoin", dataContext);
                }
                else
                {
                    Logger.Info("COUNTER == 0");
                    dataContext.Counter = 1;
                    e.Proceed("intermediateToStart", dataContext);
                }
            }

            // ugly api is ugly, introduce base class or something
            public void Execute(IExecution execution)
            {
                throw new System.NotImplementedException();
            }
        }

        private class TestData
        {
            [In]
            [Out]
            public int Counter { get; set; }
        }
    }
}