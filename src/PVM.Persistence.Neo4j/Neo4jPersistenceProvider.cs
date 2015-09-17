#region License
// -------------------------------------------------------------------------------
//  <copyright file="Neo4jPersistenceProvider.cs" company="PVM.NET Project Contributors">
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
using Neo4jClient;
using PVM.Core.Definition;
using PVM.Core.Persistence;
using PVM.Core.Plan;
using PVM.Core.Runtime;
using PVM.Persistence.Neo4j.Model;

namespace PVM.Persistence.Neo4j
{
    public class Neo4jPersistenceProvider : IPersistenceProvider
    {
        public void Persist(IExecution execution, IWorkflowDefinition definition)
        {
            throw new NotImplementedException();
        }

        public void Persist(IWorkflowDefinition workflowDefinition)
        {
            using (var client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "dkschlos"))
            {
                client.Connect();

                client.Cypher.Merge("(initialNode:Node {Identifier: {id}})")
                    .Set("initialNode = {initialNode}")
                    .WithParams(new
                    {
                        initialNode =
                            new NodeModel()
                            {
                                Identifier = workflowDefinition.InitialNode.Identifier,
                                OperationType = workflowDefinition.InitialNode.Operation.AssemblyQualifiedName
                            },
                        id = workflowDefinition.InitialNode.Identifier
                    })
                    .ExecuteWithoutResults();

                PersistNode(workflowDefinition.InitialNode, client);
            }
        }

        private void PersistNode(INode node, GraphClient client)
        {
            foreach (var outgoingTransition in node.OutgoingTransitions)
            {
                client.Cypher.Match("(source:Node {Identifier: {sourceId}})")
                      .Merge("(dest:Node {Identifier: {destId}})")
                      .Merge("(source)-[:TRANSITION]->(dest)")
                      .Set("source = {source}")
                      .Set("dest = {dest}")
                      .WithParams(new
                      {
                          source =
                              new NodeModel()
                              {
                                  Identifier = node.Identifier,
                                  OperationType = node.Operation.AssemblyQualifiedName
                              },
                          dest = new NodeModel()
                          {
                              Identifier = outgoingTransition.Destination.Identifier,
                              OperationType = outgoingTransition.Destination.Operation.AssemblyQualifiedName
                          },
                          sourceId = node.Identifier,
                          destId = outgoingTransition.Destination.Identifier
                      })
                      .ExecuteWithoutResults();
                PersistNode(outgoingTransition.Destination, client);
            }
        }

        public IWorkflowDefinition LoadWorkflowDefinition(string workflowDefinitionIdentifier)
        {
            throw new NotImplementedException();
        }

        public IExecution LoadExecution(string executionIdentifier, IExecutionPlan executionPlan)
        {
            throw new NotImplementedException();
        }

        public IWorkflowInstance LoadWorkflowInstance(string identifier, Func<IWorkflowDefinition, IExecutionPlan> executionPlanCreatorCallback)
        {
            throw new NotImplementedException();
        }
    }
}