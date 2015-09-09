#region License

// -------------------------------------------------------------------------------
//  <copyright file="NodeBuilder.cs" company="PVM.NET Project Contributors">
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

using System;
using System.Collections.Generic;
using PVM.Core.Definition;
using PVM.Core.Plan.Operations;
using PVM.Core.Plan.Operations.Base;

namespace PVM.Core.Builder
{
    public class NodeBuilder
    {
        private readonly WorkflowDefinitionBuilder parentWorkflowBuilder;
        private readonly List<TransitionData> transitions = new List<TransitionData>();
        private bool isEndNode;
        private bool isStartNode;
        private string name = Guid.NewGuid().ToString();
        private Type operation = typeof(TakeDefaultTransitionOperation);

        public NodeBuilder(WorkflowDefinitionBuilder parentWorkflowBuilder)
        {
            this.parentWorkflowBuilder = parentWorkflowBuilder;
        }

        public string Name
        {
            get { return name; }
        }

        public NodeBuilder WithOperation<T>() where T : IOperation
        {
            this.operation = typeof(T);

            return this;
        }

        public NodeBuilder WithOperation(Type type)
        {
            if (!typeof (IOperation).IsAssignableFrom(type))
            {
                throw new ArgumentException(string.Format("Type '{0}' is not an operation"));
            }

            operation = type;
            return this;
        }
        public NodeBuilder WithName(string name)
        {
            this.name = name;

            foreach (var transition in transitions)
            {
                transition.Source = name;
            }
            return this;
        }

        public TransitionBuilder AddTransition()
        {
            return new TransitionBuilder(this, name);
        }

        internal void AddTransition(TransitionData data)
        {
            if (!transitions.Contains(data))
            {
                transitions.Add(data);
            }
        }

        public NodeBuilder IsStartNode()
        {
            isStartNode = true;

            return this;
        }

        public NodeBuilder IsEndNode()
        {
            isEndNode = true;

            return this;
        }

        public IWorkflowPathBuilder BuildNode(Func<string, INode> nodeFactory)
        {
            parentWorkflowBuilder.AddNode(nodeFactory(name), isStartNode, isEndNode, transitions);

            return parentWorkflowBuilder;
        }

        public IWorkflowPathBuilder BuildNode()
        {
            return BuildNode(n => new Node(n, operation));
        }

        public IWorkflowPathBuilder BuildSubWorkflow<T>(WorkflowDefinitionBuilder subWorkflowDefinition) where T : class
        {
            return BuildNode(n => subWorkflowDefinition.WithIdentifier(n).BuildWorkflow<T>());
        }

        public IWorkflowPathBuilder BuildSubWorkflow(WorkflowDefinitionBuilder subWorkflowDefinition)
        {
            return BuildNode(n => subWorkflowDefinition.WithIdentifier(n).BuildWorkflow());
        }

        public IWorkflowPathBuilder BuildParallelGateway()
        {
            return BuildNode(n => new Node(n, typeof(ParallelGatewayOperation)));
        }

        public IWorkflowPathBuilder BuildParallelSplit()
        {
            return BuildNode(n => new Node(n, typeof(ParallelSplitOperation)));
        }

        public IWorkflowPathBuilder BuildParallelJoin()
        {
            return BuildNode(n => new Node(n, typeof(ParallelJoinOperation)));
        }
    }
}