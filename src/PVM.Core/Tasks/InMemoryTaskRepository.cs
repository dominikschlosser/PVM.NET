#region License

// -------------------------------------------------------------------------------
//  <copyright file="InMemoryTaskRepository.cs" company="PVM.NET Project Contributors">
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

namespace PVM.Core.Tasks
{
    public class InMemoryTaskRepository : ITaskRepository
    {
        private readonly IDictionary<string, IList<UserTask>> tasks = new Dictionary<string, IList<UserTask>>();

        public void Add(UserTask userTask)
        {
            IList<UserTask> userTasks;
            if (tasks.TryGetValue(userTask.WorkflowInstanceIdentifier, out userTasks))
            {
                userTasks.Add(userTask);
            }
            else
            {
                userTasks = new List<UserTask>();
                userTasks.Add(userTask);
                tasks.Add(userTask.WorkflowInstanceIdentifier, userTasks);
            }
        }

        public UserTask FindTask(string taskid, string workflowInstanceIdentifier)
        {
            if (!tasks.ContainsKey(workflowInstanceIdentifier))
            {
                return null;
            }
            return tasks[workflowInstanceIdentifier].SingleOrDefault(t => t.TaskIdentifier.Equals(taskid));
        }

        public void Remove(UserTask userTask)
        {
            if (!tasks.ContainsKey(userTask.WorkflowInstanceIdentifier))
            {
                return;
            }

            tasks[userTask.WorkflowInstanceIdentifier].Remove(userTask);
        }
    }
}