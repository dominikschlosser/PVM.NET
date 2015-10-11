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
using PVM.Core.Runtime.Algorithms;
using PVM.Core.Runtime.Operations.Base;
using PVM.Core.Utils;

namespace PVM.Core.Runtime.Operations
{
    public class ParallelGatewayOperation : IOperation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ParallelGatewayOperation));

        public void Execute(IExecution execution)
        {
            execution.Stop();

            int incomingTransitionCount = execution.CurrentNode.IncomingTransitions.Count();

            if (incomingTransitionCount > 1)
            {
                var root = execution.GetConcurrentRoot();
                var executionCollector =
                    new ExecutionCollector(e => !e.IsActive && e.CurrentNode == execution.CurrentNode);
                root.Accept(executionCollector);
                int joinedTransitionCount = executionCollector.Result.DistinctBy(e => e.IncomingTransition).Count();

                if (incomingTransitionCount > joinedTransitionCount)
                {
                    Logger.InfoFormat("Cannot join in node '{0}' yet. Joined: {1}, To join: {2}. Waiting...",
                        execution.CurrentNode.Identifier, joinedTransitionCount, incomingTransitionCount);
                    execution.Wait();
                    return;
                }

                root.Split(execution.CurrentNode);
            }
            else
            {
                execution.Split(execution.CurrentNode);
            }

        }

    }
}