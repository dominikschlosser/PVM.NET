// -------------------------------------------------------------------------------
//  <copyright file="201508261443435_InitialCreate.cs" company="PVM.NET Project Contributors">
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

using System.Data.Entity.Migrations;

namespace PVM.Persistence.Sql.Migrations
{
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExecutionModels",
                c => new
                {
                    Identifier = c.String(nullable: false, maxLength: 128),
                    CurrentNodeIdentifier = c.String(),
                    IsActive = c.Boolean(nullable: false),
                    Parent_Identifier = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Identifier)
                .ForeignKey("dbo.ExecutionModels", t => t.Parent_Identifier)
                .Index(t => t.Parent_Identifier);

            CreateTable(
                "dbo.ExecutionVariableModels",
                c => new
                {
                    Identifier = c.Int(nullable: false, identity: true),
                    Key = c.String(),
                    SerializedValue = c.String(),
                    ValueType = c.String(),
                    ExecutionModel_Identifier = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Identifier)
                .ForeignKey("dbo.ExecutionModels", t => t.ExecutionModel_Identifier)
                .Index(t => t.ExecutionModel_Identifier);

            CreateTable(
                "dbo.NodeModels",
                c => new
                {
                    Identifier = c.String(nullable: false, maxLength: 128),
                    OperationType = c.String(),
                    Discriminator = c.String(nullable: false, maxLength: 128),
                    WorkflowDefinitionModel_Identifier = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Identifier)
                .ForeignKey("dbo.NodeModels", t => t.WorkflowDefinitionModel_Identifier)
                .Index(t => t.WorkflowDefinitionModel_Identifier);

            CreateTable(
                "dbo.TransitionModels",
                c => new
                {
                    Identifier = c.String(nullable: false, maxLength: 128),
                    Source = c.String(),
                    Destination = c.String(),
                    Executed = c.Boolean(nullable: false),
                    IsDefault = c.Boolean(nullable: false),
                    NodeModel_Identifier = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Identifier)
                .ForeignKey("dbo.NodeModels", t => t.NodeModel_Identifier)
                .Index(t => t.NodeModel_Identifier);
        }

        public override void Down()
        {
            DropForeignKey("dbo.NodeModels", "WorkflowDefinitionModel_Identifier1", "dbo.NodeModels");
            DropForeignKey("dbo.NodeModels", "WorkflowDefinitionModel_Identifier", "dbo.NodeModels");
            DropForeignKey("dbo.TransitionModels", "NodeModel_Identifier1", "dbo.NodeModels");
            DropForeignKey("dbo.TransitionModels", "NodeModel_Identifier", "dbo.NodeModels");
            DropForeignKey("dbo.ExecutionVariableModels", "ExecutionModel_Identifier", "dbo.ExecutionModels");
            DropForeignKey("dbo.ExecutionModels", "Parent_Identifier", "dbo.ExecutionModels");
            DropIndex("dbo.TransitionModels", new[] {"NodeModel_Identifier"});
            DropIndex("dbo.NodeModels", new[] {"WorkflowDefinitionModel_Identifier"});
            DropIndex("dbo.ExecutionVariableModels", new[] {"ExecutionModel_Identifier"});
            DropIndex("dbo.ExecutionModels", new[] {"Parent_Identifier"});
            DropTable("dbo.TransitionModels");
            DropTable("dbo.NodeModels");
            DropTable("dbo.ExecutionVariableModels");
            DropTable("dbo.ExecutionModels");
        }
    }
}