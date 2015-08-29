#region License

// -------------------------------------------------------------------------------
//  <copyright file="ExecutionModel.cs" company="PVM.NET Project Contributors">
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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PVM.Core.Runtime;
using PVM.Core.Serialization;

namespace PVM.Persistence.Sql.Model
{
    public class ExecutionModel
    {
        [Key]
        public string Identifier { get; set; }

        public virtual ExecutionModel Parent { get; set; }
        public virtual IList<ExecutionModel> Children { get; set; }
        public virtual string CurrentNodeIdentifier { get; set; }
        public bool IsActive { get; set; }
        public virtual IList<ExecutionVariableModel> Variables { get; set; }

        public static ExecutionModel FromExecution(IExecution execution, IObjectSerializer serializer)
        {
            var variables =
                execution.Data.Select(
                    entry =>
                        new ExecutionVariableModel
                        {
                            Key = entry.Key,
                            SerializedValue = serializer.Serialize(entry.Value),
                            ValueType = entry.Value.GetType().FullName
                        }).ToList();
            return new ExecutionModel
            {
                Identifier = execution.Identifier,
                IsActive = execution.IsActive,
                CurrentNodeIdentifier = execution.CurrentNode.Identifier,
                Parent = execution.Parent == null ? null : FromExecution(execution.Parent, serializer),
                Children = execution.Children.Select(c => FromExecution(c, serializer)).ToList(),
                Variables = variables
            };
        }
    }
}