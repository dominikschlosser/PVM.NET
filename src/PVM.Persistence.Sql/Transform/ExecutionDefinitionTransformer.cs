#region License

// -------------------------------------------------------------------------------
//  <copyright file="ExecutionDefinitionTransformer.cs" company="PVM.NET Project Contributors">
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
using PVM.Core.Definition;
using PVM.Core.Runtime;
using PVM.Core.Runtime.Algorithms;
using PVM.Core.Serialization;
using PVM.Persistence.Sql.Model;

namespace PVM.Persistence.Sql.Transform
{
    public class ExecutionDefinitionTransformer
    {
        private readonly IObjectSerializer serializer;

        public ExecutionDefinitionTransformer(IObjectSerializer serializer)
        {
            this.serializer = serializer;
        }

        public ExecutionModel Transform(IExecution execution, IWorkflowDefinition definition)
        {
            var root = FindRoot(execution);
            return (ExecutionModel)CreateExecutionModel((dynamic)root, null, definition);
        }

        private ExecutionModel CreateExecutionModel(IExecution execution, ExecutionModel transformedParent, IWorkflowDefinition definition)
        {
            var executionModel = new ExecutionModel();
            PopulateExecutionModel(executionModel, transformedParent, execution, definition);
            return executionModel;
        }

        private ExecutionModel CreateExecutionModel(IWorkflowInstance workflowInstance, ExecutionModel transformedParent, IWorkflowDefinition definition)
        {
            var workflowInstanceModel = new WorkflowInstanceModel
            {
                WorkflowDefinitionIdentifier = definition.Identifier
            };
            PopulateExecutionModel(workflowInstanceModel, transformedParent, workflowInstance, definition);
            return workflowInstanceModel;

        }

        private void PopulateExecutionModel(ExecutionModel model, ExecutionModel transformedParent, IExecution execution, IWorkflowDefinition definition)
        {
            model.Identifier = execution.Identifier;
            model.IsActive = execution.IsActive;
            model.IsFinished = execution.IsFinished;
            model.IncomingTransition = execution.IncomingTransition;
            model.CurrentNodeIdentifier = execution.CurrentNode == null ? null : execution.CurrentNode.Identifier;
            model.Parent = transformedParent;
            model.Children = execution.Children.Select(c => (ExecutionModel)CreateExecutionModel((dynamic)c, model, definition)).ToList();
            model.Variables = execution.Data.Select(entry => new ExecutionVariableModel
            {
                VariableKey = entry.Key,
                SerializedValue = serializer.Serialize(entry.Value),
                ValueType = entry.Value.GetType().AssemblyQualifiedName
            }).ToList();
        }

        public IWorkflowInstance TransformBack(WorkflowInstanceModel root, IExecutionPlan plan)
        {
            return TransformBack(root, null, plan);
        }

        public IExecution TransformBack(ExecutionModel model, IExecutionPlan plan)
        {
            if (model == null)
            {
                return null;
            }

            var root = FindRoot(model);
            var transformedRoot = TransformBack((dynamic)root, null, plan);

            var collector = new ExecutionCollector(e => e.Identifier == model.Identifier);
            transformedRoot.Accept(collector);
            return collector.Result.First();
        }

        private IExecution TransformBack(ExecutionModel model, IExecution parent, IExecutionPlan plan)
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            foreach (var variable in model.Variables)
            {
                data.Add(variable.VariableKey,
                    serializer.Deserialize(variable.SerializedValue, Type.GetType(variable.ValueType)));
            }

            IList<IExecution> children = new List<IExecution>();
            var execution = new Execution(parent, plan.WorkflowDefinition.Nodes.FirstOrDefault(n => n.Identifier == model.CurrentNodeIdentifier), model.IsActive, model.IsFinished, data, model.IncomingTransition, model.Identifier, plan,
                children);
            foreach (var child in model.Children)
            {
                children.Add(TransformBack(child, execution, plan));
            }

            return execution;
        }

        private IWorkflowInstance TransformBack(WorkflowInstanceModel model, IExecution parent, IExecutionPlan plan)
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            foreach (var variable in model.Variables)
            {
                data.Add(variable.VariableKey,
                    serializer.Deserialize(variable.SerializedValue, Type.GetType(variable.ValueType)));
            }

            IList<IExecution> children = new List<IExecution>();
            var execution = new WorkflowInstance(parent, plan.WorkflowDefinition.Nodes.FirstOrDefault(n => n.Identifier == model.CurrentNodeIdentifier), model.IsActive, model.IsFinished, data, model.IncomingTransition, model.Identifier, plan,
                children);
            foreach (var child in model.Children)
            {
                children.Add(TransformBack(child, execution, plan));
            }

            return execution;
        }

        private ExecutionModel FindRoot(ExecutionModel model)
        {
            if (model.Parent == null)
            {
                return model;
            }

            return FindRoot(model.Parent);
        }

        private ExecutionModel FindModel(ExecutionModel root, string identifier)
        {
            if (root.Identifier == identifier)
            {
                return root;
            }

            foreach (var child in root.Children)
            {
                var found = FindModel(child, identifier);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private IExecution FindRoot(IExecution execution)
        {
            if (execution.Parent == null)
            {
                return execution;
            }

            return FindRoot(execution.Parent);
        }
    }
}