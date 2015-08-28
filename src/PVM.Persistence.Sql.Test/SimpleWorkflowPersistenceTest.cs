// -------------------------------------------------------------------------------
//  <copyright file="SimpleWorkflowPersistenceTest.cs" company="PVM.NET Project Contributors">
//    Copyright (c) 2015 PVM.NET Project Contributors
//    Authors: Dominik Schlosser (dominik.schlosser@gmail.com)
//            
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//    	http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -------------------------------------------------------------------------------

using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Core.Data.Attributes;
using PVM.Core.Plan.Operations.Base;
using PVM.Core.Runtime;

namespace PVM.Persistence.Sql.Test
{
    [TestFixture]
    public class SimpleWorkflowPersistenceTest
    {
        [Test]
        public void PersistSingleExecution()
        {
            var builder = new WorkflowDefinitionBuilder();

            var workflowDefinition = builder
                .AddNode()
                .WithName("start")
                .WithOperation(new TestOperation())
                .IsStartNode()
                .AddTransition()
                .WithName("transition")
                .To("end")
                .BuildTransition()
                .BuildNode()
                .AddNode()
                .WithName("end")
                .IsEndNode()
                .BuildNode()
                .BuildWorkflow<TestData>();

            var instance =
                new WorkflowEngineBuilder().ConfigureServiceLocator()
                    .OverridePersistenceProvider<SqlPersistenceProvider>()
                    .Build()
                    .CreateNewInstance(workflowDefinition);

            instance.Start(new TestData());

            Assert.False(instance.IsFinished);
        }

        private class TestOperation : IOperation
        {
            public void Execute(IExecution execution)
            {
                execution.Wait("signal");
            }
        }

        [WorkflowData]
        private class TestData
        {
            public TestData()
            {
                Counter = 0;
                Data = new NestedTestClass() {Name = "bla", Value = 42.3f};
            }

            public virtual int Counter { get; set; }
            public virtual NestedTestClass Data { get; set; }
        }

        public class NestedTestClass
        {
            public string Name { get; set; }
            public float Value { get; set; }
        }
    }
}