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

using System;
using NHibernate;
using PVM.Core.Definition;
using PVM.Core.Persistence;
using PVM.Core.Runtime;
using PVM.Core.Serialization;
using PVM.Persistence.Sql.Model;
using PVM.Persistence.Sql.Transform;

namespace PVM.Persistence.Sql
{
    public class SqlPersistenceProvider : IPersistenceProvider
    {
        private readonly ExecutionDefinitionTransformer executionTransformer;
        private readonly ISessionFactory sessionFactory;
        private readonly WorkflowDefinitionTransformer workflowDefinitionTransformer;

        public SqlPersistenceProvider(IObjectSerializer objectSerializer, ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
            workflowDefinitionTransformer = new WorkflowDefinitionTransformer();
            executionTransformer = new ExecutionDefinitionTransformer(objectSerializer);
        }

        public void Persist(IExecution execution, IWorkflowDefinition definition)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    var entity = executionTransformer.Transform(execution, definition);

                    session.SaveOrUpdate(entity);
                    session.Flush();
                    txn.Commit();
                }
            }
        }

        public void Persist(IWorkflowDefinition workflowDefinition)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    var entity = workflowDefinitionTransformer.Transform(workflowDefinition);

                    session.SaveOrUpdate(entity);
                    session.Flush();

                    txn.Commit();
                }
            }
        }

        public IWorkflowDefinition LoadWorkflowDefinition(string workflowDefinitionIdentifier)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var model =
                    session.QueryOver<WorkflowDefinitionModel>()
                           .Where(w => w.Identifier == workflowDefinitionIdentifier)
                           .SingleOrDefault();

                if (model == null)
                {
                    return null;
                }

                return workflowDefinitionTransformer.TransformBack(model);
            }
        }

        public IExecution LoadExecution(string executionIdentifier, IExecutionPlan executionPlan)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var model =
                    session.QueryOver<ExecutionModel>()
                           .Where(w => w.Identifier == executionIdentifier)
                           .SingleOrDefault();

                if (model == null)
                {
                    return null;
                }

                return executionTransformer.TransformBack(model, executionPlan);
            }
        }

        public IWorkflowInstance LoadWorkflowInstance(string identifier, Func<IWorkflowDefinition, IExecutionPlan> executionPlanCreatorCallback)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var model =
                    session.QueryOver<WorkflowInstanceModel>()
                           .Where(w => w.Identifier == identifier)
                           .SingleOrDefault();

                if (model == null)
                {
                    return null;
                }

                var workflowDefinition = LoadWorkflowDefinition(model.WorkflowDefinitionIdentifier);
                return executionTransformer.TransformBack(model, executionPlanCreatorCallback(workflowDefinition));
            }
        }
    }
}