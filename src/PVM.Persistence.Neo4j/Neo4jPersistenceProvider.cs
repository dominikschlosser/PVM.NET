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
using PVM.Core.Runtime;
using PVM.Core.Runtime.Algorithms;
using PVM.Core.Runtime.Plan;
using PVM.Core.Serialization;
using PVM.Persistence.Neo4j.Model;

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

        public void Persist(IExecution execution)
        {
            var findRoot = FindRoot(execution);

            var query = graphClient.Cypher
                                   .Match("(e:Execution {Identifier: {id}})")
                                   .OptionalMatch("(e)-[t:EXECUTES]->()")
                                   .Delete("t");


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
                         new ExecutionModel
                         {
                             Identifier = findRoot.Identifier,
                             WorkflowInstanceIdentifier = findRoot.WorkflowInstanceIdentifier,
                             IsActive = findRoot.IsActive,
                             IsFinished = findRoot.IsFinished,
                             Data = objectSerializer.Serialize(execution.Data),
                             IncomingTransition = findRoot.IncomingTransition
                         },
                     id = findRoot.Identifier,
                     currentNodeId = execution.CurrentNode == null ? null : execution.CurrentNode.Identifier
                 })
                 .ExecuteWithoutResults();

            graphClient.Cypher
                       .Match("(e:Execution {Identifier: {id}})")
                       .Match("(wf:WorkflowDefinition {Identifier: {wfId}})")
                       .Merge("(e)-[:REFERENCES]->(wf)")
                       .WithParams(new
                       {
                           id = findRoot.Identifier,
                           wfId = findRoot.WorkflowDefinition.Identifier
                       }).ExecuteWithoutResults();

            PersistChildExecutions(findRoot, graphClient);
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
                               new NodeModel
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
                                    .Match(
                                        "(source:Node {Identifier: {initialNodeId}})-[transition:TRANSITION*]->(destination:Node)")
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

            var workflowDefinition = new WorkflowDefinition(workflowDefinitionIdentifier,
                new List<INode>(allNodes.Values), endNodes,
                initial);

            // TODO: smells
            workflowDefinition.AddStartTransition();

            return workflowDefinition;
        }

        public IExecution LoadExecution(string executionIdentifier, IExecutionPlan executionPlan)
        {
            var rootQuery = graphClient.Cypher
                                       .Match("(e:Execution {Identifier: {id}})")
                                       .OptionalMatch("(r:Execution)-[:PARENT_OF*]->(e)")
                                       .OptionalMatch("(r)-[:EXECUTES]->(currentNode:Node)")
                                       .Where("NOT (:Execution)-[:PARENT_OF*]->(r)")
                                       .WithParam("id", executionIdentifier)
                                       .Return((r, e, currentNode) => new
                                       {
                                           Root = r.As<ExecutionModel>(),
                                           Current = e.As<ExecutionModel>(),
                                           CurrentNode = currentNode.As<NodeModel>()
                                       });

            var executionModel = rootQuery.Results.Last();

            var root = executionModel.Root ?? executionModel.Current;


            var wfQuery = graphClient.Cypher
                                     .Match("(e:Execution {Identifier: {id}})-[*]->(n:Node)")
                                     .Match("(wf:WorkflowDefinition)-[*]->(n)")
                                     .OptionalMatch(
                                         "(e:Execution {Identifier: {id}})-[:REFERENCES]->(wf:WorkflowDefinition)")
                                     .WithParam("id", root.Identifier)
                                     .Return(wf => wf.As<WorkflowDefinitionModel>());

            var workflowDefinition = LoadWorkflowDefinition(wfQuery.Results.First().Identifier);


            Dictionary<string, object> data =
                (Dictionary<string, object>)
                    objectSerializer.Deserialize(root.Data, typeof (Dictionary<string, object>));
            var children = new List<IExecution>();

            var rootExecution = new Execution(null,
                executionModel.CurrentNode == null
                    ? null
                    : workflowDefinition.Nodes.First(n => n.Identifier == executionModel.CurrentNode.Identifier),
                root.IsActive, root.IsFinished, data, root.IncomingTransition,
                root.Identifier, root.WorkflowInstanceIdentifier,
                executionPlan, children,
                workflowDefinition);

            FillChildren(rootExecution, children, executionPlan, workflowDefinition);

            var collector = new ExecutionCollector(e => e.Identifier == executionIdentifier);
            rootExecution.Accept(collector);

            return collector.Result.First();
        }

        private void PersistChildExecutions(IExecution execution, IGraphClient client)
        {
            foreach (var child in execution.Children)
            {
                if (child.IsFinished)
                {
                    client.Cypher.Match("(child:Execution {Identifier: {childId}})")
                          .OptionalMatch("()-[from]->(child)")
                          .OptionalMatch("(child)-[to]->()")
                          .OptionalMatch("(child)-[]->(e:Execution)")
                          .OptionalMatch("(e)-[t]->()")
                          .Delete("child, from, to, t, e")
                          .WithParam("childId", child.Identifier)
                          .ExecuteWithoutResults();

                    continue;
                }

                var query = client.Cypher
                                  .Merge("(parent:Execution {Identifier: {parentId}})")
                                  .Merge("(parent)-[:PARENT_OF]->(child:Execution {Identifier: {childId}})");

                query.Set("child = {execution}")
                     .WithParams(new
                     {
                         execution =
                             new ExecutionModel
                             {
                                 Identifier = child.Identifier,
                                 WorkflowInstanceIdentifier = child.WorkflowInstanceIdentifier,
                                 IsActive = child.IsActive,
                                 IsFinished = child.IsFinished,
                                 Data = objectSerializer.Serialize(execution.Data),
                                 IncomingTransition = child.IncomingTransition
                             },
                         parentId = execution.Identifier,
                         childId = child.Identifier
                     })
                     .ExecuteWithoutResults();

                client.Cypher
                      .Match("(child:Execution {Identifier: {childId}})")
                      .OptionalMatch("(child)-[t:EXECUTES]->()")
                      .WithParam("childId", child.Identifier)
                      .Delete("t").ExecuteWithoutResults();

                if (child.CurrentNode != null)
                {
                    client.Cypher
                          .Match("(child:Execution {Identifier: {childId}})")
                          .Match("(n:Node {Identifier: {currentNodeId}})")
                          .Merge("(child)-[:EXECUTES]->(n)")
                          .WithParam("childId", child.Identifier)
                          .WithParam("currentNodeId", child.CurrentNode.Identifier)
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
                              new NodeModel
                              {
                                  Identifier = node.Identifier,
                                  OperationType = node.Operation.AssemblyQualifiedName,
                                  IsEndNode = definition.EndNodes.Contains(node)
                              },
                          dest = new NodeModel
                          {
                              Identifier = outgoingTransition.Destination.Identifier,
                              OperationType = outgoingTransition.Destination.Operation.AssemblyQualifiedName,
                              IsEndNode = definition.EndNodes.Contains(outgoingTransition.Destination)
                          },
                          sourceId = node.Identifier,
                          destId = outgoingTransition.Destination.Identifier,
                          transition =
                              new TransitionModel
                              {
                                  Identifier = outgoingTransition.Identifier,
                                  IsDefault = outgoingTransition.IsDefault
                              }
                      })
                      .ExecuteWithoutResults();
                PersistNode(outgoingTransition.Destination, definition, client);
            }
        }

        private void FillChildren(IExecution parent, List<IExecution> children,
            IExecutionPlan executionPlan, IWorkflowDefinition workflowDefinition)
        {
            var result = graphClient.Cypher
                                    .Match("(root:Execution {Identifier: {id}})")
                                    .Match("(root)-[:PARENT_OF]->(child:Execution)")
                                    .OptionalMatch("(child)-[:EXECUTES]->(currentNode:Node)")
                                    .WithParam("id", parent.Identifier)
                                    .Return((child, currentNode) => new
                                    {
                                        Child = child.As<ExecutionModel>(),
                                        CurrentNode = currentNode.As<NodeModel>()
                                    });

            foreach (var r in result.Results)
            {
                Dictionary<string, object> data =
                    (Dictionary<string, object>)
                        objectSerializer.Deserialize(r.Child.Data, typeof (Dictionary<string, object>));

                var childExecutions = new List<IExecution>();
                var e = new Execution(parent,
                    r.CurrentNode == null
                        ? null
                        : workflowDefinition.Nodes.First(n => n.Identifier == r.CurrentNode.Identifier),
                    r.Child.IsActive, r.Child.IsFinished, data, r.Child.IncomingTransition, r.Child.Identifier,
                    r.Child.WorkflowInstanceIdentifier, executionPlan,
                    childExecutions, workflowDefinition);
                children.Add(e);

                FillChildren(e, childExecutions, executionPlan, workflowDefinition);
            }
        }
    }
}