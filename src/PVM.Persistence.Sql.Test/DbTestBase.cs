#region License

// -------------------------------------------------------------------------------
//  <copyright file="DbTestBase.cs" company="PVM.NET Project Contributors">
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

using System.Configuration;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using PVM.Persistence.Sql.Mapping;

namespace PVM.Persistence.Sql.Test
{
    public abstract class DbTestBase
    {
        protected ISessionFactory SessionFactory { get; private set; }

        [SetUp]
        public void TestSetUp()
        {
            CreateDatabase(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }

        [TearDown]
        public void TestTearDown()
        {
            SessionFactory.Dispose();
            SessionFactory = null;
        }

        private void CreateDatabase(string connectionString)
        {
            var configuration = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connectionString))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ExecutionMap>())
                .BuildConfiguration();

            var exporter = new SchemaExport(configuration);
            exporter.Execute(true, true, false);

            SessionFactory = configuration.BuildSessionFactory();
        }
    }
}