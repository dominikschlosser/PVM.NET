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
using Node = PVM.Core.Definition.Node;

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
                IDictionary<string, INode> allNodes = new Dictionary<string, INode>();
                List<INode> endNodes = new List<INode>();

                var initialNodeResult = client.Cypher.Match("(wf:WorkflowDefinition {Identifier: {wfId}})")
                                                             .Match("(wf)-[:STARTS_AT]->(initialNode:Node)")
                                                             .WithParams(new
                                                             {
                                                                 wfId = workflowDefinitionIdentifier
                                                             })
                                                             .ReturnDistinct(initialNode => initialNode.As<NodeModel>());

                var initialNodeModel = initialNodeResult.Results.First();
                INode initial = new Node(initialNodeModel.Identifier, Type.GetType(initialNodeModel.OperationType));
                allNodes.Add(initial.Identifier, initial);
                if (initialNodeModel.IsEndNode)
                {
                    endNodes.Add(initial);
                }

                var result = client.Cypher
                      .Match("(source:Node {Identifier: {initialNodeId}})-[transition:TRANSITION*]->(destination:Node)")
                      .WithParams(new
                      {
                          initialNodeId = initialNodeModel.Identifier
                      })
                      .ReturnDistinct((source, transition, destination) => new
                      {
                          Source = source.As<NodeModel>(),
                          Transition = transition.As<List<TransitionModel>>(),
                          Destination = destination.As<NodeModel>()
                      });



                foreach (var r in result.Results)
                {
                    INode source;

                    if (allNodes.ContainsKey(r.Source.Identifier))
                    {
                        source = allNodes[r.Source.Identifier];
                    }
                    else
                    {
                        source = new Node(r.Source.Identifier, Type.GetType(r.Source.OperationType));
                        allNodes.Add(source.Identifier, source);

                        if (r.Source.IsEndNode && !endNodes.Contains(source))
                        {
                            endNodes.Add(source);
                        }
                    }


                    if (r.Transition.Count > 1)
                    {
                        for (int i = 0; i < r.Transition.Count - 1; i++)
                        {
                            var transitionToTake =
                                source.OutgoingTransitions.First(t => t.Identifier == r.Transition[i].Identifier);
                            source = transitionToTake.Destination;
                        }
                    }

                    INode destination;

                    if (allNodes.ContainsKey(r.Destination.Identifier))
                    {
                        destination = allNodes[r.Destination.Identifier];
                    }
                    else
                    {
                        destination = new Node(r.Destination.Identifier, Type.GetType(r.Destination.OperationType));
                        allNodes.Add(destination.Identifier, destination);

                        if (r.Destination.IsEndNode && !endNodes.Contains(destination))
                        {
                            endNodes.Add(destination);
                        }
                    }


                    var lastTransition = r.Transition.Last();
                    var transition = new Transition(lastTransition.Identifier, lastTransition.IsDefault, source,
                        destination);
                    source.AddOutgoingTransition(transition);
                    destination.AddIncomingTransition(transition);
                }

                return new WorkflowDefinition(workflowDefinitionIdentifier, new List<INode>(allNodes.Values), endNodes, initial);
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