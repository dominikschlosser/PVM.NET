#region License

// -------------------------------------------------------------------------------
//  <copyright file="Node.cs" company="PVM.NET Project Contributors">
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
using PVM.Core.Plan;
using PVM.Core.Plan.Operations.Base;
using PVM.Core.Runtime;

namespace PVM.Core.Definition
{
    public interface INode
    {
        string OperationType { get; }
        IEnumerable<Transition> IncomingTransitions { get; }
        IEnumerable<Transition> OutgoingTransitions { get; }
        string Identifier { get; }
        void AddOutgoingTransition(Transition transition);
        void AddIncomingTransition(Transition transition);
        void Execute(IExecution execution, IExecutionPlan executionPlan);
    }


    public class Node : INode
    {
        private readonly List<Transition> incomingTransitions = new List<Transition>();
        private readonly string operationType;
        private readonly List<Transition> outgoingTransitions = new List<Transition>();

        public Node(string identifier) : this(identifier, new TakeDefaultTransitionOperation())
        {
        }

        public Node(string identifier, IOperation operation)
        {
            operationType = operation.GetType().AssemblyQualifiedName;
            Identifier = identifier;
        }

        public string OperationType
        {
            get { return operationType; }
        }

        public IEnumerable<Transition> IncomingTransitions
        {
            get { return incomingTransitions; }
        }

        public IEnumerable<Transition> OutgoingTransitions
        {
            get { return outgoingTransitions; }
        }

        public virtual void AddOutgoingTransition(Transition transition)
        {
            outgoingTransitions.Add(transition);
        }

        public virtual void AddIncomingTransition(Transition transition)
        {
            incomingTransitions.Add(transition);
        }

        public string Identifier { get; private set; }

        public virtual void Execute(IExecution execution, IExecutionPlan executionPlan)
        {
            executionPlan.Proceed(execution, operationType);
        }

        protected bool Equals(Node other)
        {
            return string.Equals(Identifier, other.Identifier);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Node) obj);
        }

        public override int GetHashCode()
        {
            return (Identifier != null ? Identifier.GetHashCode() : 0);
        }
    }
}