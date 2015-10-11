// -------------------------------------------------------------------------------
//  <copyright file="Neo4jPersistenceProvider.cs" company="PVM.NET Project Contributors">
//    Copyright (c) 2015 PVM.NET Project Contributors
//    Authors: Dominik Schlosser (dominik.schlosser@gmail.com)
//            
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//    	http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Neo4jClient;
using PVM.Core.Definition;
using PVM.Core.Persistence;
using PVM.Core.Runtime;
using PVM.Core.Runtime.Algorithms;
using PVM.Core.Serialization;
using PVM.Persistence.Neo4j.Model;
using Node = PVM.Core.Definition.Node;

namespace PVM.Persistence.Neo4j
{
    public class Neo4jPersistenceProvider : IPersistenceProvider
    {
        private readonly IGraphClient graphClient;
        private readonly IObjectSerializer objectSerializer;

        public Neo4jPersistenceProvider(IGraphClient graphClient, IObjectSerializer objectSerializer)
        {
            this.graphClient = graphClient;
            this.objectSerializer = objectSerializer;
        }

        public void Persist(IExecution execution, IWorkflowDefinition definition)
        {
            var findRoot = FindRoot(execution);

            var query = graphClient.Cypher
                                   .Match("(e:Execution {Identifier: {id}})");


            if (execution.CurrentNode != null)
            {
                query.Match("(n:Node {Identifier: {currentNodeId}})")
                     .Merge("(e)-[:EXECUTES]->(n)");
            }
            else
            {
                query.Merge("(e:Execution {Identifier: {id}})");
            }

            query.Set("e = {execution}")
                .WithParams(new
                {
                    execution =
                        new ExecutionModel()
                        {
                            Identifier = findRoot.Identifier,
                            IsActive = findRoot.IsActive,
                            Data = objectSerializer.Serialize(execution.Data),
                            IncomingTransition = findRoot.IncomingTransition
                        },
                    id = findRoot.Identifier,
                    currentNodeId = execution.CurrentNode == null ? null : execution.CurrentNode.Identifier
                })
                .ExecuteWithoutResults();

            PersistChildExecutions(findRoot, graphClient);
        }

        private void PersistChildExecutions(IExecution execution, IGraphClient client)
        {
            foreach (var child in execution.Children)
            {
                var query = client.Cypher
                    .Merge("(parent:Execution {Identifier: {parentId}})")
                    .Merge("(parent)-[:PARENT_OF]->(child:Execution {Identifier: {childId}})");

                query.Set("child = {execution}")
                    .WithParams(new
                    {
                        execution =
                            new ExecutionModel()
                            {
                                Identifier = child.Identifier,
                                IsActive = child.IsActive
                            },
                        parentId = execution.Identifier,
                        childId = child.Identifier
                    })
                    .ExecuteWithoutResults();

                if (child.CurrentNode != null)
                {
                    client.Cypher.Match("(n:Node {Identifier: {currentNodeId}})")
                        .Match("(child:Execution {Identifier: {childId}})")
                        .Merge("(child)-[:EXECUTES]->(n)")
                         .WithParams(new
                         {
                             currentNodeId = child.CurrentNode.Identifier,
                             childId = child.Identifier
                         })
                        .ExecuteWithoutResults();
                }

                PersistChildExecutions(child, client);
            }
        }

        private IExecution FindRoot(IExecution execution)
        {
            if (execution.Parent == null)
            {
                return execution;
            }

            return FindRoot(execution.Parent);
        }

        public void Persist(IWorkflowDefinition workflowDefinition)
        {
            graphClient.Cypher
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

            PersistNode(workflowDefinition.InitialNode, workflowDefinition, graphClient);
        }

        private void PersistNode(INode node, IWorkflowDefinition definition, IGraphClient client)
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
                        transition =
                            new TransitionModel()
                            {
                                Identifier = outgoingTransition.Identifier,
                                IsDefault = outgoingTransition.IsDefault
                            }
                    })
                    .ExecuteWithoutResults();
                PersistNode(outgoingTransition.Destination, definition, client);
            }
        }

        public IWorkflowDefinition LoadWorkflowDefinition(string workflowDefinitionIdentifier)
        {
            IDictionary<string, INode> allNodes = new Dictionary<string, INode>();
            List<INode> endNodes = new List<INode>();

            var initialNodeResult = graphClient.Cypher.Match("(wf:WorkflowDefinition {Identifier: {wfId}})")
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

            var result = graphClient.Cypher
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

            var workflowDefinition = new WorkflowDefinition(workflowDefinitionIdentifier, new List<INode>(allNodes.Values), endNodes,
                initial);

            // TODO: smells
            workflowDefinition.AddStartTransition();

            return workflowDefinition;
        }

        public IExecution LoadExecution(string executionIdentifier, IExecutionPlan executionPlan)
        {
            var root = graphClient.Cypher
                                  .Match("(e:Execution {Identifier: {id}})")
                                  .Match("(root:Execution)-[:PARENT_OF*]->(e)")
                                  .Match("(root)-[:EXECUTES]->(currentNode:Node)")
                                  .Where("NOT (:Execution)-[:PARENT_OF*]->(root)")
                                  .Limit(1)
                                  .WithParam("id", executionIdentifier)
                                  .Return((r, currentNode) => new
                                  {
                                      Root = r.As<ExecutionModel>(),
                                      CurrentNode = currentNode.As<NodeModel>()
                                  });

            var executionModel = root.Results.Single();

            Dictionary<string, object> data = (Dictionary<string, object>) objectSerializer.Deserialize(executionModel.Root.Data, typeof(Dictionary<string, object>));
            var children = new List<IExecution>();

            var rootExecution = new Execution(null, 
                executionPlan.WorkflowDefinition.Nodes.First(n => n.Identifier == executionModel.CurrentNode.Identifier), 
                executionModel.Root.IsActive, false, data, executionModel.Root.IncomingTransition, executionModel.Root.Identifier, executionPlan, children);

            FillChildren(executionModel.Root.Identifier, children, executionPlan);

            var collector = new ExecutionCollector(e => e.Identifier == executionIdentifier);
            rootExecution.Accept(collector);

            return collector.Result.First();
        }

        private void FillChildren(string executionId, List<IExecution> children, IExecutionPlan executionPlan)
        {
            var result = graphClient.Cypher
                .Match("(root:Execution {Identifier: {id}})")
                .Match("(root:Execution)-[:PARENT_OF]->(child:Execution)")
                .Match("(child)-[:EXECUTES]->(currentNode:Node)")
                .WithParam("id", executionId)
                .Return((child, currentNode) => new
                {
                    Child = child.As<ExecutionModel>(),
                    CurrentNode = currentNode.As<NodeModel>()
                });

            foreach (var r in result.Results)
            {
                Dictionary<string, object> data = (Dictionary<string, object>)objectSerializer.Deserialize(r.Child.Data, typeof(Dictionary<string, object>));

                var childExecutions = new List<IExecution>();
                var e = new Execution(null,
                    executionPlan.WorkflowDefinition.Nodes.First(n => n.Identifier == r.CurrentNode.Identifier),
                    r.Child.IsActive, false, data, r.Child.IncomingTransition, r.Child.Identifier, executionPlan,
                    childExecutions);
                children.Add(e);

                FillChildren(r.Child.Identifier, childExecutions, executionPlan);
            }
        }

        public IWorkflowInstance LoadWorkflowInstance(string identifier,
            Func<IWorkflowDefinition, IExecutionPlan> executionPlanCreatorCallback)
        {
            throw new NotImplementedException();
        }
    }
}