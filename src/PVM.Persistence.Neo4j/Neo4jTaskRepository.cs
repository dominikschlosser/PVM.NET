#region License
// -------------------------------------------------------------------------------
//  <copyright file="Neo4jTaskRepository.cs" company="PVM.NET Project Contributors">
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
using Neo4jClient;
using PVM.Core.Tasks;

namespace PVM.Persistence.Neo4j
{
    public class Neo4jTaskRepository : ITaskRepository
    {
        private readonly IGraphClient graphClient;

        public Neo4jTaskRepository(IGraphClient graphClient)
        {
            this.graphClient = graphClient;
        }

        public void Add(UserTask userTask)
        {
            
            graphClient.Cypher.Match("(e:Execution {Identifier: {id}})")
                .Merge("(e)-[:HAS_TASK]->(t:Task)")
                .Set("t = {task}")
                .WithParams(new
                {
                    id = userTask.ExecutionIdentifier,
                    task = userTask
                })
                .ExecuteWithoutResults();
        }

        public UserTask FindTask(string taskName)
        {
            return graphClient.Cypher.Match("(t:Task {TaskIdentifier: {name}})")
                       .WithParam("name", taskName)
                       .Return(t => t.As<UserTask>()).Limit(1).Results.FirstOrDefault();
        }
    }
}