#region License
// -------------------------------------------------------------------------------
//  <copyright file="ExecutionMapping.cs" company="PVM.NET Project Contributors">
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

using FluentNHibernate.Mapping;
using PVM.Persistence.Sql.Model;

namespace PVM.Persistence.Sql.Mapping
{
    public class ExecutionMap : ClassMap<ExecutionModel>
    {
        public ExecutionMap()
        {
            Id(m => m.Identifier).GeneratedBy.Assigned();
            HasMany(m => m.Children).Cascade.All();
            HasMany(m => m.Variables).Cascade.All();
            Map(m => m.CurrentNodeIdentifier);
            Map(m => m.IncomingTransition);
            Map(m => m.IsActive);
            Map(m => m.IsFinished);
            References(m => m.Parent);

            DiscriminateSubClassesOnColumn("Discriminator");
        }     
    }
}