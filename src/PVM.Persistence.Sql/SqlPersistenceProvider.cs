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
using PVM.Core.Utils;
using PVM.Persistence.Sql.Model;
using PVM.Persistence.Sql.Transform;

namespace PVM.Persistence.Sql
{
    public class SqlPersistenceProvider : IPersistenceProvider
    {
        private readonly IObjectSerializer objectSerializer;
        private readonly WorkflowDefinitionTransformer transformer;

        public SqlPersistenceProvider(IObjectSerializer objectSerializer, IOperationResolver operationResolver)
        {
            this.objectSerializer = objectSerializer;
            transformer = new WorkflowDefinitionTransformer(operationResolver);
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
                var entity = transformer.Transform(workflowDefinition);

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

                return transformer.TransformBack(model);
            }
        }
    }
}