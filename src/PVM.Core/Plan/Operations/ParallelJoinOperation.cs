#region License

// -------------------------------------------------------------------------------
//  <copyright file="ParallelJoinOperation.cs" company="PVM.NET Project Contributors">
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
using PVM.Core.Plan.Operations.Base;
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class ParallelJoinOperation : IOperation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ParallelJoinOperation));

        public void Execute(IExecution execution)
        {
            if (execution.Parent != null)
            {
                execution.Kill();

                foreach (var incomingExecution in execution.Parent.Children)
                {
                    if (!incomingExecution.Identifier.Equals(execution.Identifier) && !incomingExecution.IsFinished)
                    {
                        Logger.InfoFormat("Transition from node '{0}' in execution '{1}' not taken yet. Waiting...",
                            incomingExecution.CurrentNode.Identifier, incomingExecution.Identifier);
                        return;
                    }
                }

                execution.Parent.Resume(execution.CurrentNode);
            }
            else
            {
                // If we are here this operation is not actually used to join execution paths. TODO: Maybe throw?
                execution.Proceed();
            }

            
        }
    }
}