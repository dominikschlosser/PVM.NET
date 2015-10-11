#region License
// -------------------------------------------------------------------------------
//  <copyright file="TransformBackTest.cs" company="PVM.NET Project Contributors">
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

using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PVM.Core.Definition;
using PVM.Core.Runtime;
using PVM.Core.Serialization;
using PVM.Persistence.Sql.Model;
using PVM.Persistence.Sql.Transform;

namespace PVM.Persistence.Sql.Test.ExecutionDefinitionTransformer
{
    [TestFixture]
    public class TransformBackTest
    {
        private class TestContext
        {
            private readonly IList<INode> workflowDefinitionNodes = new List<INode>();
            private string identifier;
            private string currentNode;
            private readonly Mock<IWorkflowDefinitionTransformer> workflowDefinitionTransformer = new Mock<IWorkflowDefinitionTransformer>();
            private readonly Mock<IWorkflowDefinition> workflowDefinition = new Mock<IWorkflowDefinition>();
            private string incomingTransition = "incomingTransition";
            private bool finished;
            private bool active;
            private Mock<IExecutionPlan> plan = new Mock<IExecutionPlan>();
            private readonly IList<ExecutionVariableModel> variables = new List<ExecutionVariableModel>();
            private IObjectSerializer objectSerializer = Mock.Of<IObjectSerializer>();

            public TestContext()
            {
                plan.SetupGet(p => p.WorkflowDefinition).Returns(workflowDefinition.Object);
                workflowDefinition.SetupGet(w => w.Nodes).Returns(workflowDefinitionNodes);
                workflowDefinitionTransformer.Setup(t => t.TransformBack(It.IsAny<WorkflowDefinitionModel>()))
                                        .Returns(workflowDefinition.Object);
            }

            public IExecution ExecuteTransform()
            {
                return ExecuteTransform(BuildWorkflowInstanceModel());
            }

            public IExecution ExecuteTransform(ExecutionModel executionModel)
            {
                var transformer = new Transform.ExecutionDefinitionTransformer(objectSerializer);

                return transformer.TransformBack(executionModel, plan.Object);
            }

            private WorkflowInstanceModel BuildWorkflowInstanceModel()
            {
                return new WorkflowInstanceModel()
                {
                    Identifier = identifier,
                    CurrentNodeIdentifier = currentNode,
                    Variables = variables,
                    Children = new List<ExecutionModel>(),
                    IncomingTransition = incomingTransition,
                    IsFinished = finished,
                    IsActive = active
                };
            }

            public TestContext WithExecutionIdentifier(string identifier)
            {
                this.identifier = identifier;
                return this;
            }
            public TestContext WithCurrentNode(string identifier)
            {
                this.currentNode = identifier;
                return this;
            }
            public TestContext WithExistingNode(INode node)
            {
                workflowDefinitionNodes.Add(node);
                return this;
            }


            public TestContext WithIncomingTransition(string transitionId)
            {
                incomingTransition = transitionId;
                return this;
            }

            public TestContext Finished()
            {
                finished = true;
                return this;
            }
            public TestContext Active()
            {
                active = true;
                return this;
            }

            public TestContext WithVariable(ExecutionVariableModel variable)
            {
                variables.Add(variable);
                return this;
            }

            public TestContext WithObjectSerializer(IObjectSerializer serializer)
            {
                this.objectSerializer = serializer;
                return this;
            }

        }
        [Test]
        public void SetsIdentifier()
        {
            IExecution result = new TestContext().WithExecutionIdentifier("myIdentifier").ExecuteTransform();

            Assert.That(result.Identifier, Is.EqualTo("myIdentifier"));
        }

        [Test]
        public void SetsIncomingTransition()
        {
            IExecution result = new TestContext().WithIncomingTransition("myIdentifier").ExecuteTransform();

            Assert.That(result.IncomingTransition, Is.EqualTo("myIdentifier"));
        }

        [Test]
        public void SetsFinished()
        {
            IExecution result = new TestContext().Finished().ExecuteTransform();

            Assert.That(result.IsFinished);
        }
        
        [Test]
        public void SetsActive()
        {
            IExecution result = new TestContext().Active().ExecuteTransform();

            Assert.That(result.IsActive);
        }

        [Test]
        public void AddsData()
        {
            var type = typeof (string);
            var objectSerializer = new Mock<IObjectSerializer>();
            objectSerializer.Setup(s => s.Deserialize("Val", type)).Returns("DeserializedVal");
            IExecution result = new TestContext()
                .WithObjectSerializer(objectSerializer.Object)
                .WithVariable(new ExecutionVariableModel(){VariableKey = "myVar", SerializedValue = "Val", ValueType = type.AssemblyQualifiedName})
                .ExecuteTransform();

            Assert.That(result.Data["myVar"], Is.SameAs("DeserializedVal"));
        }

        [Test]
        public void AddsParent()
        {
            WorkflowInstanceModel parent = new WorkflowInstanceModel();
            var child = new ExecutionModel()
            {
                Children = new List<ExecutionModel>(),
                Variables = new List<ExecutionVariableModel>(),
                Parent = parent,
                Identifier = "child"
            };

            parent.Children = new List<ExecutionModel>() {child};
            parent.Variables = new List<ExecutionVariableModel>();
            parent.Identifier = "parentId";

            IExecution result = new TestContext()
                .ExecuteTransform(child);

            Assert.That(result.Parent.Identifier, Is.EqualTo("parentId"));
        }


        [Test]
        public void SetsCurrentNode()
        {
            var currentNode = new Node("myCurrentNode");

            IExecution result =
                new TestContext().WithCurrentNode("myCurrentNode").WithExistingNode(currentNode).ExecuteTransform();

            Assert.That(result.CurrentNode, Is.SameAs(currentNode));
        }
    }
}