#region License

// -------------------------------------------------------------------------------
//  <copyright file="IExecutionPlan.cs" company="PVM.NET Project Contributors">
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

using PVM.Core.Definition;
using PVM.Core.Plan.Operations.Base;
using PVM.Core.Runtime;

namespace PVM.Core.Plan
{
    public interface IExecutionPlan
    {
        void Proceed(IExecution execution, INode node);
        void OnExecutionStarting(IExecution execution);
        void OnExecutionStopped(IExecution execution);
        void OnOutgoingTransitionIsNull(IExecution execution, string transitionIdentifier);
        void OnExecutionResuming(IExecution execution);
        void OnExecutionReachesWaitState(IExecution execution);
        void OnExecutionSignaled(IExecution execution);
        IWorkflowDefinition WorkflowDefinition { get; }
    }
}