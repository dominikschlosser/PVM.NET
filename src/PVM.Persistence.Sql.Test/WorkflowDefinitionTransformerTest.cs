#region License
// -------------------------------------------------------------------------------
//  <copyright file="WorkflowDefinitionTransformerTest.cs" company="PVM.NET Project Contributors">
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
using PVM.Persistence.Sql.Model;
using PVM.Persistence.Sql.Transform;

namespace PVM.Persistence.Sql.Test
{
    [TestFixture]
    public class WorkflowDefinitionTransformerTest
    {
        [Test]
        public void TransformsEmptyWorkflowDefinition()
        {
            var transformer = new WorkflowDefinitionTransformer();
            var emptyWorkflow = new WorkflowDefinitionBuilder().BuildWorkflow();

            WorkflowDefinitionModel result = transformer.Transform(emptyWorkflow);

            Assert.That(result, Is.Not.Null);
            Assert.That(!result.Nodes.Any());
        }

        [Test]
        public void TransformsSingleNodeWorkflowDefinition()
        {
            var transformer = new WorkflowDefinitionTransformer();
            var workflow = new WorkflowDefinitionBuilder()
                    .AddNode()
                        .WithName("node")
                    .BuildNode()
                .BuildWorkflow();

            WorkflowDefinitionModel result = transformer.Transform(workflow);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Nodes.Count, Is.EqualTo(1));
            Assert.That(result.Nodes.First().Identifier, Is.EqualTo("node"));
        }

        [Test]
        public void SetsInitialNodeProperty()
        {
            var transformer = new WorkflowDefinitionTransformer();
            var workflow = new WorkflowDefinitionBuilder()
                    .AddNode()
                        .WithName("node")
                        .IsStartNode()
                    .BuildNode()
                .BuildWorkflow();

            WorkflowDefinitionModel result = transformer.Transform(workflow);
            Assert.That(result.Nodes.First().IsInitialNode);
        }

        [Test]
        public void SetsWorkflowOperationTypeProperty()
        {
            var transformer = new WorkflowDefinitionTransformer();
            var workflow = new WorkflowDefinitionBuilder()
                    .AddNode()
                        .WithName("node")
                    .BuildNode()
                .BuildWorkflow();

            WorkflowDefinitionModel result = transformer.Transform(workflow);
            Assert.That(result.OperationType, Is.EqualTo(workflow.Operation.GetType().AssemblyQualifiedName));
        }

        [Test]
        public void SetsEndNodeProperty()
        {
            var transformer = new WorkflowDefinitionTransformer();
            var workflow = new WorkflowDefinitionBuilder()
                    .AddNode()
                        .WithName("node")
                        .IsEndNode()
                    .BuildNode()
                .BuildWorkflow();

            WorkflowDefinitionModel result = transformer.Transform(workflow);
            Assert.That(result.Nodes.First().IsEndNode);
        }

        [Test]
        public void SetsOperationTypeProperty()
        {
            var transformer = new WorkflowDefinitionTransformer();
            var workflow = new WorkflowDefinitionBuilder()
                    .AddNode()
                        .WithName("node")
                        .WithOperation(new TestOperation())
                    .BuildNode()
                .BuildWorkflow();

            WorkflowDefinitionModel result = transformer.Transform(workflow);
            Assert.That(result.Nodes.First().OperationType, Is.EqualTo(typeof(TestOperation).AssemblyQualifiedName));
        }

        [Test]
        public void SetsIdentifierProperty()
        {
            var transformer = new WorkflowDefinitionTransformer();
            var workflow = new WorkflowDefinitionBuilder()
                    .WithIdentifier("ident")
                    .AddNode()
                        .WithName("node")
                    .BuildNode()
                .BuildWorkflow();

            WorkflowDefinitionModel result = transformer.Transform(workflow);
            Assert.That(result.Identifier, Is.EqualTo("ident"));
        }

        [Test]
        public void AddsOutgoingTransition()
        {
            var transformer = new WorkflowDefinitionTransformer();
            var workflow = new WorkflowDefinitionBuilder()
                    .AddNode()
                        .WithName("node")
                        .AddTransition()
                            .WithName("toEnd")
                            .To("end")
                            .BuildTransition()
                    .BuildNode()
                    .AddNode()
                        .WithName("end")
                    .BuildNode()
                .BuildWorkflow();

            WorkflowDefinitionModel result = transformer.Transform(workflow);
            
            var startNode = result.Nodes.First(n => n.Identifier == "node");
            Assert.That(startNode.OutgoingTransitions.Count, Is.EqualTo(1));
            var transition = startNode.OutgoingTransitions.First();
            Assert.That(transition.Identifier, Is.EqualTo("toEnd"));
            Assert.That(transition.Source, Is.EqualTo("node"));
            Assert.That(transition.Destination, Is.EqualTo("end"));
            Assert.False(transition.IsDefault);
            Assert.False(transition.Executed);
        }

        [Test]
        public void SetDefaultPropertyOnTransition()
        {
            var transformer = new WorkflowDefinitionTransformer();
            var workflow = new WorkflowDefinitionBuilder()
                    .AddNode()
                        .WithName("node")
                        .AddTransition()
                            .WithName("toEnd")
                            .To("end")
                            .IsDefault()
                            .BuildTransition()
                    .BuildNode()
                    .AddNode()
                        .WithName("end")
                    .BuildNode()
                .BuildWorkflow();

            WorkflowDefinitionModel result = transformer.Transform(workflow);

            var startNode = result.Nodes.First(n => n.Identifier == "node");
            var transition = startNode.OutgoingTransitions.First();
            Assert.That(transition.IsDefault);
        }

        [Test]
        public void SetsExecutedPropertyOnTransition()
        {
            var transformer = new WorkflowDefinitionTransformer();
            var workflow = new WorkflowDefinitionBuilder()
                    .AddNode()
                        .WithName("node")
                        .IsStartNode()
                        .AddTransition()
                            .WithName("toEnd")
                            .To("end")
                            .BuildTransition()
                    .BuildNode()
                    .AddNode()
                        .WithName("end")
                        .IsEndNode()
                    .BuildNode()
                .BuildWorkflow();
            var workflowEngine = new WorkflowEngineBuilder().Build();
            workflowEngine.CreateNewInstance(workflow).Start();

            WorkflowDefinitionModel result = transformer.Transform(workflow);

            var startNode = result.Nodes.First(n => n.Identifier == "node");
            var transition = startNode.OutgoingTransitions.First();
            Assert.That(transition.Executed);
        }

        [Test]
        public void TransformsNestedWorkflow()
        {
            var transformer = new WorkflowDefinitionTransformer();
            var workflow = new WorkflowDefinitionBuilder()
                .WithIdentifier("testWorkflowDefinition")
                .AddNode()
                    .WithName("start")
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
                    .BuildNode()
                    .AsDefinitionBuilder())
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                .BuildNode()
                .BuildWorkflow();

            WorkflowDefinitionModel result = transformer.Transform(workflow);

            var nestedWorkflowNode = result.Nodes.First(n => n.Identifier == "nested");

            Assert.That(nestedWorkflowNode, Is.InstanceOf<WorkflowDefinitionModel>());
        }

        private class TestOperation : IOperation
        {
            public void Execute(IExecution execution)
            {
                execution.Proceed();
            }
        }
    }
}