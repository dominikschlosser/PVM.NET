#region License

// -------------------------------------------------------------------------------
//  <copyright file="UserTask.cs" company="PVM.NET Project Contributors">
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

namespace PVM.Core.Tasks
{
    public class UserTask
    {
        private readonly string executionIdentifier;
        private readonly string taskIdentifier;
        private readonly string workflowInstanceIdentifier;

        public UserTask()
        {
        }

        public UserTask(string taskIdentifier, string executionIdentifier, string workflowInstanceIdentifier)
        {
            this.taskIdentifier = taskIdentifier;
            this.executionIdentifier = executionIdentifier;
            this.workflowInstanceIdentifier = workflowInstanceIdentifier;
        }

        public string ExecutionIdentifier
        {
            get { return executionIdentifier; }
        }

        public string TaskIdentifier
        {
            get { return taskIdentifier; }
        }

        public string WorkflowInstanceIdentifier
        {
            get { return workflowInstanceIdentifier; }
        }

        protected bool Equals(UserTask other)
        {
            return string.Equals(taskIdentifier, other.taskIdentifier) &&
                   string.Equals(workflowInstanceIdentifier, other.workflowInstanceIdentifier);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UserTask) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((taskIdentifier != null ? taskIdentifier.GetHashCode() : 0)*397) ^
                       (workflowInstanceIdentifier != null ? workflowInstanceIdentifier.GetHashCode() : 0);
            }
        }
    }
}