#region License

// -------------------------------------------------------------------------------
//  <copyright file="SqlPersistenceProvider.cs" company="PVM.NET Project Contributors">
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
using PVM.Core.Definition;
using PVM.Core.Persistence;
using PVM.Core.Runtime;
using PVM.Core.Serialization;
using PVM.Persistence.Sql.Model;

namespace PVM.Persistence.Sql
{
    public class SqlPersistenceProvider : IPersistenceProvider
    {
        private readonly IObjectSerializer objectSerializer;

        public SqlPersistenceProvider(IObjectSerializer objectSerializer)
        {
            this.objectSerializer = objectSerializer;
        }

        public void Persist(IExecution execution)
        {
            using (var db = new PvmContext())
            {
                var entity = ExecutionModel.FromExecution(execution, objectSerializer);

                if (db.Executions.Any(e => e.Identifier == execution.Identifier))
                {
                    db.Executions.Attach(entity);
                }
                else
                {
                    db.Executions.Add(entity);
                }

                db.SaveChanges();
            }
        }

        public void Persist(IWorkflowDefinition workflowDefinition)
        {
            using (var db = new PvmContext())
            {
                var entity = WorkflowDefinitionModel.FromWorkflowDefinition(workflowDefinition);

                if (db.WorkflowDefinitions.Any(d => d.Identifier == workflowDefinition.Identifier))
                {
                    db.WorkflowDefinitions.Attach(entity);
                }
                else
                {
                    db.WorkflowDefinitions.Add(entity);
                }

                db.SaveChanges();
            }
        }

        public IWorkflowDefinition LoadWorkflowDefinition(string workflowDefinitionIdentifier)
        {
            using (var db = new PvmContext())
            {
                var model = db.WorkflowDefinitions.FirstOrDefault(w => w.Identifier == workflowDefinitionIdentifier);

                if (model == null)
                {
                    return null;
                }

                var allNodes = MapNodes(model);

                return new WorkflowDefinition.Builder().WithIdentifier(model.Identifier)
                                                       .WithInitialNode(allNodes[model.Nodes.First(n => n.IsInitialNode).Identifier])
                                                       .WithNodes(allNodes.Select(n => n.Value).ToList())
                                                       .WithEndNodes(
                                                           model.Nodes.Where(n => n.IsEndNode)
                                                                .Select(n => allNodes[n.Identifier])
                                                                .ToList())
                                                       .Build();
            }
        }

        private static IDictionary<string, INode> MapNodes(WorkflowDefinitionModel model)
        {
            IDictionary<string, INode> allNodes = new Dictionary<string, INode>();

            foreach (var nodeModel in model.Nodes)
            {
                allNodes.Add(nodeModel.Identifier, new Node(nodeModel.Identifier, nodeModel.OperationType));
            }

            foreach (var nodeModel in model.Nodes)
            {
                INode node = allNodes[nodeModel.Identifier];

                foreach (var outgoingTransition in nodeModel.OutgoingTransitions)
                {
                    INode dest = allNodes[outgoingTransition.Destination];
                    node.AddOutgoingTransition(new Transition(outgoingTransition.Identifier,
                        outgoingTransition.IsDefault, outgoingTransition.Executed, node, dest));
                }
            }

            foreach (var node in allNodes.Values)
            {
                foreach (var outgoingTransition in node.OutgoingTransitions)
                {
                    INode dest = outgoingTransition.Destination;
                    if (dest != null)
                    {
                        dest.AddIncomingTransition(outgoingTransition);
                    }
                }
            }
            return allNodes;
        }
    }
}