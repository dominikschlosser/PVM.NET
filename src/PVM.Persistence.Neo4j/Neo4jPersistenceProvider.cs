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
using System.Collections.Generic;
using System.Linq;
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
            using (var client = CreateGraphClient())
            {
                client.Cypher
                    .Merge("(wf:WorkflowDefinition {Identifier: {wfId}})")
                    .Merge("(initialNode:Node {Identifier: {id}})")
                    .Merge("(wf)-[:STARTS_AT]->(initialNode)")
                    .Set("initialNode = {initialNode}")
                    .WithParams(new
                    {
                        initialNode =
                            new NodeModel()
                            {
                                Identifier = workflowDefinition.InitialNode.Identifier,
                                OperationType = workflowDefinition.InitialNode.Operation.AssemblyQualifiedName
                            },
                        id = workflowDefinition.InitialNode.Identifier,
                        wfId = workflowDefinition.Identifier
                    })
                    .ExecuteWithoutResults();

                PersistNode(workflowDefinition.InitialNode, workflowDefinition, client);
            }
        }

        private void PersistNode(INode node, IWorkflowDefinition definition, GraphClient client)
        {
            foreach (var outgoingTransition in node.OutgoingTransitions)
            {
                client.Cypher.Match("(source:Node {Identifier: {sourceId}})")
                      .Merge("(dest:Node {Identifier: {destId}})")
                      .Merge("(source)-[t:TRANSITION]->(dest)")
                      .Set("source = {source}")
                      .Set("dest = {dest}")
                      .Set("t = {transition}")
                      .WithParams(new
                      {
                          source =
                              new NodeModel()
                              {
                                  Identifier = node.Identifier,
                                  OperationType = node.Operation.AssemblyQualifiedName,
                                  IsEndNode = definition.EndNodes.Contains(node)
                              },
                          dest = new NodeModel()
                          {
                              Identifier = outgoingTransition.Destination.Identifier,
                              OperationType = outgoingTransition.Destination.Operation.AssemblyQualifiedName,
                              IsEndNode = definition.EndNodes.Contains(outgoingTransition.Destination)
                          },
                          sourceId = node.Identifier,
                          destId = outgoingTransition.Destination.Identifier,
                          transition = new TransitionModel(){Identifier = outgoingTransition.Identifier, IsDefault = outgoingTransition.IsDefault}
                      })
                      .ExecuteWithoutResults();
                PersistNode(outgoingTransition.Destination, definition, client);
            }
        }

        public IWorkflowDefinition LoadWorkflowDefinition(string workflowDefinitionIdentifier)
        {
            using (var client = CreateGraphClient())
            {
                var result = client.Cypher.Match("(wf:WorkflowDefinition {Identifier: {wfId}})")
                      .Match("(wf)-[:STARTS_AT]->(initialNode:Node)")
                      .Match("(source)-[transition:TRANSITION]->(destination)")
                      .WithParams(new
                      {
                          wfId = workflowDefinitionIdentifier
                      })
                      .ReturnDistinct((source, transition, destination) => new
                      {
                          Source = source.As<NodeModel>(),
                          Transition = transition.As<TransitionModel>(),
                          Destination = destination.As<NodeModel>()
                      });

                List<INode> allNodes = new List<INode>();
                List<INode> endNodes = new List<INode>();

                Node initialNode = null;
                foreach (var r in result.Results)
                {
                    if (initialNode == null)
                    {
                        initialNode = new Node(r.Source.Identifier, Type.GetType(r.Source.OperationType));
                        allNodes.Add(initialNode);
                        if (r.Source.IsEndNode)
                        {
                            endNodes.Add(initialNode);
                        }
                    }

                    Node destination = new Node(r.Destination.Identifier, Type.GetType(r.Destination.OperationType));
                    if (allNodes.Contains(destination))
                    {
                        continue;
                    }
                    allNodes.Add(destination);
                    if (r.Destination.IsEndNode)
                    {
                        endNodes.Add(destination);
                    }

                    var transition = new Transition(r.Transition.Identifier, r.Transition.IsDefault, initialNode,
                        destination);
                    initialNode.AddOutgoingTransition(transition);
                    destination.AddIncomingTransition(transition);

                    PopulateNode(destination, allNodes, endNodes, client);
                }

                return new WorkflowDefinition(workflowDefinitionIdentifier, allNodes, endNodes, initialNode);
            }

        }

        private void PopulateNode(Node node, List<INode> allNodes, List<INode> endNodes, GraphClient client)
        {
            var result = client.Cypher.Match("(source:Node {Identifier: {sourceId}})")
                      .Match("(source)-[transition:TRANSITION]->(destination)")
                      .WithParams(new
                      {
                          sourceId = node.Identifier
                      })
                      .ReturnDistinct((source, transition, destination) => new
                      {
                          Source = source.As<NodeModel>(),
                          Transition = transition.As<TransitionModel>(),
                          Destination = destination.As<NodeModel>()
                      });

            foreach (var r in result.Results)
            {
                Node destination = new Node(r.Destination.Identifier, Type.GetType(r.Destination.OperationType));
                if (allNodes.Contains(destination))
                {
                    continue;
                }
                allNodes.Add(destination);
                if (r.Destination.IsEndNode)
                {
                    endNodes.Add(destination);
                }

                var transition = new Transition(r.Transition.Identifier, r.Transition.IsDefault, node,
                    destination);
                node.AddOutgoingTransition(transition);
                destination.AddIncomingTransition(transition);

                PopulateNode(destination, allNodes, endNodes, client);
            }
        }
        private static GraphClient CreateGraphClient()
        {
            var graphClient = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "dkschlos");
            graphClient.Connect();
            return graphClient;
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