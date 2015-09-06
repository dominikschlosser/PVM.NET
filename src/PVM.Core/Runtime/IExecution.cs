#region License

// -------------------------------------------------------------------------------
//  <copyright file="IExecution.cs" company="PVM.NET Project Contributors">
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
using PVM.Core.Definition;
using PVM.Core.Plan;

namespace PVM.Core.Runtime
{
    public interface IExecution
    {
        string Identifier { get; }
        IExecution Parent { get; }
        IList<IExecution> Children { get; }
        INode CurrentNode { get; }
        IExecutionPlan Plan { get; }
        bool IsFinished { get; }
        bool IsActive { get; }
        bool IsPaused { get; }
        IDictionary<string, object> Data { get; }
        void Proceed();
        void Proceed(string transitionName);
        void Resume();
        void Resume(INode currentNode);
        void Stop();
        void Start(INode startNode, IDictionary<string, object> data);
        void CreateChildren(IEnumerable<INode> nodes);
        void Accept(IExecutionVisitor visitor);
        void Proceed(INode currentNode);
        void Wait();
        void Signal();
        void Kill();
    }
}