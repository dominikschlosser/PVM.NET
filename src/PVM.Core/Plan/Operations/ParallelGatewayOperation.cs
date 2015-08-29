#region License

// -------------------------------------------------------------------------------
//  <copyright file="ParallelGatewayOperation.cs" company="PVM.NET Project Contributors">
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
using log4net;
using PVM.Core.Plan.Operations.Base;
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class ParallelGatewayOperation : IOperation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ParallelGatewayOperation));

        public void Execute(IExecution execution)
        {
            execution.Stop();

            foreach (var incomingTransition in execution.CurrentNode.IncomingTransitions)
            {
                if (!incomingTransition.Executed)
                {
                    Logger.InfoFormat("Transition '{0}' not taken yet. Waiting...", incomingTransition.Identifier);
                    return;
                }
            }

            var owningExecution = execution.Parent ?? execution;

            if (execution.CurrentNode.OutgoingTransitions.Count() == 1)
            {
                owningExecution.Resume(execution.CurrentNode);
            }
            else
            {
                foreach (var outgoingTransition in execution.CurrentNode.OutgoingTransitions)
                {
                    outgoingTransition.Executed = true;
                    Logger.InfoFormat("Split to '{0}'", outgoingTransition.Identifier);
                    owningExecution.CreateChild(outgoingTransition.Destination);
                }
            }
        }
    }
}