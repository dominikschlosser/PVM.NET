#region License

// -------------------------------------------------------------------------------
//  <copyright file="SimpleWorkflowPersistenceTest.cs" company="PVM.NET Project Contributors">
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

using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Core.Data.Attributes;
using PVM.Core.Runtime;
using PVM.Core.Runtime.Operations.Base;
using PVM.Persistence.Sql.Model;

namespace PVM.Persistence.Sql.Test
{
    [TestFixture]
    public class SimpleWorkflowPersistenceDbTest : DbTestBase
    {
        private class TestOperation : IOperation
        {
            public void Execute(IExecution execution)
            {
                execution.Wait();
            }
        }

        [WorkflowData]
        public class TestData
        {
            public TestData()
            {
                Counter = 0;
                Data = new NestedTestClass { Name = "bla", Value = 42.3f };
            }

            [In, Out]
            public virtual int Counter { get; set; }

            [In, Out]
            public virtual NestedTestClass Data { get; set; }
        }

        public class NestedTestClass
        {
            public string Name { get; set; }
            public float Value { get; set; }
        }

        [Test]
        public void PersistSingleExecution()
        {
            var builder = new WorkflowDefinitionBuilder();

            var workflowDefinition = builder
                .WithIdentifier("testWorkflowDefinition")
                .AddNode()
                    .WithName("start")
                    .WithOperation<TestOperation>()
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
                                           .ImportModule(new SqlPersistenceTestModule(SessionFactory))
                                           .Build()
                                           .StartNewInstance(workflowDefinition, new TestData());

            using (var session = SessionFactory.OpenSession())
                Assert.That(session.QueryOver<WorkflowDefinitionModel>().Where(d => d.Identifier == workflowDefinition.Identifier).SingleOrDefault(), Is.Not.Null);

            Assert.False(instance.IsFinished);
        }
    }
}