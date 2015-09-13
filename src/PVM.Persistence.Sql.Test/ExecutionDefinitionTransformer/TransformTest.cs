#region License

// -------------------------------------------------------------------------------
//  <copyright file="TransformTest.cs" company="PVM.NET Project Contributors">
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
using System.Linq;
using Moq;
using NUnit.Framework;
using PVM.Core.Definition;
using PVM.Core.Runtime;
using PVM.Core.Serialization;
using PVM.Persistence.Sql.Model;

namespace PVM.Persistence.Sql.Test.ExecutionDefinitionTransformer
{
    [TestFixture]
    public class TransformTest
    {
        private class TestExecutionBuilder
        {
            private bool active;
            private INode currentNode = Mock.Of<INode>();
            private string executionIdentifier;
            private IExecution parent;
            private readonly IList<IExecution> children = new List<IExecution>(); 
            private IDictionary<string, object> variables = new Dictionary<string, object>();
            private bool finished;
            private string incomingTransition;

            public TestExecutionBuilder WithIdentifier(string identifier)
            {
                executionIdentifier = identifier;
                return this;
            }

            public TestExecutionBuilder WithCurrentNode(INode node)
            {
                currentNode = node;
                return this;
            }

            public IExecution BuildExecution()
            {
                var execution = new Mock<IExecution>();
                execution.Setup(e => e.CurrentNode).Returns(currentNode);
                execution.SetupGet(e => e.Identifier).Returns(executionIdentifier);
                execution.SetupGet(e => e.IsActive).Returns(active);
                execution.SetupGet(e => e.Parent).Returns(parent);
                execution.SetupGet(e => e.Children).Returns(children);
                execution.SetupGet(e => e.Data).Returns(variables);
                execution.SetupGet(e => e.IsFinished).Returns(true);
                execution.SetupGet(e => e.IncomingTransition).Returns(incomingTransition);

                return execution.Object;
            }

            public TestExecutionBuilder IsActive()
            {
                active = true;
                return this;
            }

            public TestExecutionBuilder WithParent(IExecution parent)
            {
                this.parent = parent;
                return this;
            }

            public TestExecutionBuilder WithChild(IExecution child)
            {
                children.Add(child);
                return this;
            }

            public TestExecutionBuilder WithVariables(Dictionary<string, object> variables)
            {
                this.variables = variables;
                return this;
                
            }

            public TestExecutionBuilder IsFinished()
            {
                finished = true;
                return this;
            }

            public TestExecutionBuilder WithIncomingTransition(string transitionName)
            {
                incomingTransition = transitionName;
                return this;
                
            }
        }

        [Test]
        public void TransformsActiveState()
        {
            var execution = new TestExecutionBuilder().IsActive().BuildExecution();
            var transformer = new Transform.ExecutionDefinitionTransformer(Mock.Of<IObjectSerializer>());

            ExecutionModel result = transformer.Transform(execution);

            Assert.That(result.IsActive, Is.True);
        }

        [Test]
        public void TransformsCurrentNode()
        {
            var execution = new TestExecutionBuilder().WithCurrentNode(new Node("nodeIdentifier")).BuildExecution();
            var transformer = new Transform.ExecutionDefinitionTransformer(Mock.Of<IObjectSerializer>());

            ExecutionModel result = transformer.Transform(execution);

            Assert.That(result.CurrentNodeIdentifier, Is.EqualTo("nodeIdentifier"));
        }

        [Test]
        public void TransformsIdentifier()
        {
            var identifier = "myExecution";
            var execution = new TestExecutionBuilder().WithIdentifier(identifier).BuildExecution();
            var transformer = new Transform.ExecutionDefinitionTransformer(Mock.Of<IObjectSerializer>());

            ExecutionModel result = transformer.Transform(execution);

            Assert.That(result.Identifier, Is.EqualTo(identifier));
        }

        [Test]
        public void AddsParentIfAvailable()
        {
            var parent = new TestExecutionBuilder().WithIdentifier("parent").BuildExecution();
            var execution = new TestExecutionBuilder().WithParent(parent).BuildExecution();
            var transformer = new Transform.ExecutionDefinitionTransformer(Mock.Of<IObjectSerializer>());

            ExecutionModel result = transformer.Transform(execution);

            Assert.That(result.Parent.Identifier, Is.EqualTo("parent"));
        }

        [Test]
        public void AddsChildrenIfAvailable()
        {
            var child1 = new TestExecutionBuilder().WithIdentifier("child1").BuildExecution();
            var child2 = new TestExecutionBuilder().WithIdentifier("child2").BuildExecution();
            var execution = new TestExecutionBuilder().WithChild(child1).WithChild(child2).BuildExecution();
            var transformer = new Transform.ExecutionDefinitionTransformer(Mock.Of<IObjectSerializer>());

            ExecutionModel result = transformer.Transform(execution);

            Assert.That(result.Children.Count, Is.EqualTo(2));
            Assert.That(result.Children.Any(c => c.Identifier.Equals("child1")));
            Assert.That(result.Children.Any(c => c.Identifier.Equals("child2")));
        }

        [Test]
        public void AddsVariables()
        {
            var variables = new Dictionary<string, object>();
            variables.Add("var1", new ComplexVariableType(){Identifier = "id1"});
            variables.Add("var2", new ComplexVariableType() { Identifier = "id2" });
            var serializer = new Mock<IObjectSerializer>();
            serializer.Setup(s => s.Serialize(It.IsAny<ComplexVariableType>()))
                      .Returns<ComplexVariableType>(v => v.Identifier);

            var execution = new TestExecutionBuilder().WithVariables(variables).BuildExecution();
            var transformer = new Transform.ExecutionDefinitionTransformer(serializer.Object);

            ExecutionModel result = transformer.Transform(execution);

            Assert.That(result.Variables.Count, Is.EqualTo(2));
            Assert.That(result.Variables.First(v => v.Key.Equals("var1")).SerializedValue, Is.EqualTo("id1"));
            Assert.That(result.Variables.First(v => v.Key.Equals("var2")).SerializedValue, Is.EqualTo("id2"));
        }

        [Test]
        public void SetsAssemblyQualifiedNameAsVariableType()
        {
            var variables = new Dictionary<string, object>();
            variables.Add("var1", new ComplexVariableType() { Identifier = "id1" });

            var execution = new TestExecutionBuilder().WithVariables(variables).BuildExecution();
            var transformer = new Transform.ExecutionDefinitionTransformer(Mock.Of<IObjectSerializer>());

            ExecutionModel result = transformer.Transform(execution);

            Assert.That(result.Variables.First().ValueType, Is.EqualTo(typeof(ComplexVariableType).AssemblyQualifiedName));
        }

        [Test]
        public void SetsIsFinishedProperty()
        {
            var execution = new TestExecutionBuilder().IsFinished().BuildExecution();
            var transformer = new Transform.ExecutionDefinitionTransformer(Mock.Of<IObjectSerializer>());

            ExecutionModel result = transformer.Transform(execution);

            Assert.That(result.IsFinished, Is.True);
        }

        [Test]
        public void SetsIncomingTransitionProperty()
        {
            string transitionName = "tansitionName";
            var execution = new TestExecutionBuilder().WithIncomingTransition(transitionName).BuildExecution();
            var transformer = new Transform.ExecutionDefinitionTransformer(Mock.Of<IObjectSerializer>());

            ExecutionModel result = transformer.Transform(execution);

            Assert.That(result.IncomingTransition, Is.EqualTo(transitionName));
        }
        private class ComplexVariableType
        {
            public string Identifier { get; set; }
        }
    }
}