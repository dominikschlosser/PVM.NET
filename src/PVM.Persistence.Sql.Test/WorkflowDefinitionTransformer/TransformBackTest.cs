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

using System.Linq;
using NUnit.Framework;
using PVM.Core.Definition;
using PVM.Core.Runtime;
using PVM.Core.Runtime.Operations.Base;
using PVM.Persistence.Sql.Model;

namespace PVM.Persistence.Sql.Test.WorkflowDefinitionTransformer
{
    [TestFixture]
    public class TransformBackTest
    {
        private class TestOperation : IOperation
        {
            public void Execute(IExecution execution)
            {
            }
        }

        [Test]
        public void AddsIncomingTransitionOnNode()
        {
            var model = new WorkflowDefinitionModel();
            model.Nodes.Add(new NodeModel
            {
                Identifier = "node",
                OutgoingTransitions = new[]
                {
                    new TransitionModel
                    {
                        Identifier = "transitionId",
                        Source = "node",
                        Destination = "destNode"
                    }
                }
            });
            model.Nodes.Add(new NodeModel
            {
                Identifier = "destNode"
            });
            var transformer = new Transform.WorkflowDefinitionTransformer();

            IWorkflowDefinition definition = transformer.TransformBack(model);

            var destNode = definition.Nodes.First(n => n.Identifier == "destNode");
            Assert.That(destNode.IncomingTransitions.Count(), Is.EqualTo(1));
            var transition = destNode.IncomingTransitions.First();
            Assert.That(transition.Identifier, Is.EqualTo("transitionId"));
            Assert.That(transition.Source.Identifier, Is.EqualTo("node"));
        }

        [Test]
        [Ignore("TODO")]
        public void AddsNestedWorkflow()
        {
            var model = new WorkflowDefinitionModel();
            model.Nodes.Add(new NodeModel
            {
                Identifier = "node",
                OutgoingTransitions = new[]
                {
                    new TransitionModel
                    {
                        Identifier = "transitionId",
                        Source = "node",
                        Destination = "nested"
                    }
                }
            });
            model.Nodes.Add(new WorkflowDefinitionModel
            {
                Identifier = "nested",
                OperationType = typeof (TakeDefaultTransitionOperation).AssemblyQualifiedName
            });
            var transformer = new Transform.WorkflowDefinitionTransformer();

            IWorkflowDefinition definition = transformer.TransformBack(model);

            var nestedWorkflow = definition.Nodes.FirstOrDefault(n => n.Identifier == "nested");
            Assert.That(nestedWorkflow, Is.Not.Null);
            Assert.That(nestedWorkflow, Is.InstanceOf<IWorkflowDefinition>());
        }

        [Test]
        public void AddsNode()
        {
            var model = new WorkflowDefinitionModel();
            model.Nodes.Add(new NodeModel
            {
                Identifier = "node"
            });
            var transformer = new Transform.WorkflowDefinitionTransformer();

            IWorkflowDefinition definition = transformer.TransformBack(model);

            Assert.That(definition.Nodes.Count, Is.EqualTo(1));
            Assert.That(definition.Nodes.First().Identifier, Is.EqualTo("node"));
        }

        [Test]
        public void AddsOperationToNode()
        {
            var model = new WorkflowDefinitionModel();
            model.Nodes.Add(new NodeModel
            {
                Identifier = "node",
                OperationType = typeof (TestOperation).AssemblyQualifiedName
            });
            var transformer = new Transform.WorkflowDefinitionTransformer();

            IWorkflowDefinition definition = transformer.TransformBack(model);

            Assert.That(definition.Nodes.First().Operation, Is.EqualTo(typeof (TestOperation)));
        }

        [Test]
        public void AddsOutgoingTransitionOnNode()
        {
            var model = new WorkflowDefinitionModel();
            model.Nodes.Add(new NodeModel
            {
                Identifier = "node",
                OutgoingTransitions = new[]
                {
                    new TransitionModel
                    {
                        Identifier = "transitionId",
                        Source = "node",
                        Destination = "destNode"
                    }
                }
            });
            model.Nodes.Add(new NodeModel
            {
                Identifier = "destNode"
            });
            var transformer = new Transform.WorkflowDefinitionTransformer();

            IWorkflowDefinition definition = transformer.TransformBack(model);

            var node = definition.Nodes.First(n => n.Identifier == "node");
            Assert.That(node.OutgoingTransitions.Count(), Is.EqualTo(1));
            var transition = node.OutgoingTransitions.First();
            Assert.That(transition.Identifier, Is.EqualTo("transitionId"));
            Assert.That(transition.Destination.Identifier, Is.EqualTo("destNode"));
        }

        [Test]
        public void ReturnsEmptyWorkflowDefinitionIfModelIsEmpty()
        {
            var model = new WorkflowDefinitionModel();
            var transformer = new Transform.WorkflowDefinitionTransformer();

            IWorkflowDefinition definition = transformer.TransformBack(model);

            Assert.That(definition, Is.Not.Null);
            Assert.That(definition.Nodes, Is.Empty);
            Assert.That(definition.OutgoingTransitions, Is.Empty);
            Assert.That(definition.IncomingTransitions, Is.Empty);
        }

        [Test]
        public void SetsDefaultPropertyOnTransition()
        {
            var model = new WorkflowDefinitionModel();
            model.Nodes.Add(new NodeModel
            {
                Identifier = "node",
                OutgoingTransitions = new[]
                {
                    new TransitionModel
                    {
                        Identifier = "transitionId",
                        IsDefault = true,
                        Source = "node",
                        Destination = "destNode"
                    }
                }
            });
            model.Nodes.Add(new NodeModel
            {
                Identifier = "destNode"
            });
            var transformer = new Transform.WorkflowDefinitionTransformer();

            IWorkflowDefinition definition = transformer.TransformBack(model);

            var transition = definition.Nodes.First(n => n.Identifier == "node").OutgoingTransitions.First();
            Assert.That(transition.IsDefault);
        }

        [Test]
        public void SetsEndNodePropertyOnNode()
        {
            var model = new WorkflowDefinitionModel();
            model.Nodes.Add(new NodeModel
            {
                Identifier = "node",
                IsEndNode = true
            });
            var transformer = new Transform.WorkflowDefinitionTransformer();

            IWorkflowDefinition definition = transformer.TransformBack(model);

            var node = definition.Nodes.First();
            Assert.That(definition.EndNodes.Contains(node));
        }

        [Test]
        public void SetsStartNodePropertyOnNode()
        {
            var model = new WorkflowDefinitionModel();
            model.Nodes.Add(new NodeModel
            {
                Identifier = "node",
                IsInitialNode = true
            });
            var transformer = new Transform.WorkflowDefinitionTransformer();

            IWorkflowDefinition definition = transformer.TransformBack(model);

            var node = definition.Nodes.First();
            Assert.That(definition.InitialNode, Is.EqualTo(node));
        }

        [Test]
        public void SetsWorkflowIdentifier()
        {
            var model = new WorkflowDefinitionModel {Identifier = "identifier"};
            model.Nodes.Add(new NodeModel
            {
                Identifier = "node"
            });
            var transformer = new Transform.WorkflowDefinitionTransformer();

            IWorkflowDefinition definition = transformer.TransformBack(model);

            Assert.That(definition.Identifier, Is.EqualTo("identifier"));
        }
    }
}