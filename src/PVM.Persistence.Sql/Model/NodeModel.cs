#region License

// -------------------------------------------------------------------------------
//  <copyright file="NodeModel.cs" company="PVM.NET Project Contributors">
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
using PVM.Core.Definition;

namespace PVM.Persistence.Sql.Model
{
    public class NodeModel
    {
        [Key]
        public string Identifier { get; set; }

        public string OperationType { get; set; }
        public bool IsEndNode { get; set; }
        public bool IsInitialNode { get; set; }
        public virtual IList<TransitionModel> OutgoingTransitions { get; set; }

        protected bool Equals(NodeModel other)
        {
            return string.Equals(Identifier, other.Identifier);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NodeModel) obj);
        }

        public override int GetHashCode()
        {
            return (Identifier != null ? Identifier.GetHashCode() : 0);
        }

        public static NodeModel FromNode(INode node, bool isEndNode, bool isInitialNode)
        {
            return new NodeModel
            {
                Identifier = node.Identifier,
                OperationType = node.OperationType,
                OutgoingTransitions = node.OutgoingTransitions.Select(TransitionModel.FromTransition).ToList(),
                IsEndNode = isEndNode,
                IsInitialNode = isInitialNode
            };
        }
    }
}