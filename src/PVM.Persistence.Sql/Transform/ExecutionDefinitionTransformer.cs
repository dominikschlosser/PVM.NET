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

using System.Linq;
using PVM.Core.Runtime;
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

        public ExecutionModel Transform(IExecution execution)
        {
            return new ExecutionModel()
            {
                Identifier = execution.Identifier,
                IsActive = execution.IsActive,
                CurrentNodeIdentifier = execution.CurrentNode.Identifier,
                Parent = execution.Parent != null ? Transform(execution.Parent) : null,
                Children = execution.Children.Select(Transform).ToList(),
                Variables = execution.Data.Select(entry => new ExecutionVariableModel()
                {
                    Key = entry.Key,
                    SerializedValue = serializer.Serialize(entry.Value),
                    ValueType = entry.Value.GetType().AssemblyQualifiedName
                }).ToList()
            };
        }
    }
}