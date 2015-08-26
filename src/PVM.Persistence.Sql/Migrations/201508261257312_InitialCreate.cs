namespace PVM.Persistence.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExecutionModels",
                c => new
                    {
                        Identifier = c.String(nullable: false, maxLength: 128),
                        IsActive = c.Boolean(nullable: false),
                        Parent_Identifier = c.String(maxLength: 128),
                        CurrentNode_Identifier = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Identifier)
                .ForeignKey("dbo.ExecutionModels", t => t.Parent_Identifier)
                .ForeignKey("dbo.NodeModels", t => t.CurrentNode_Identifier)
                .Index(t => t.Parent_Identifier)
                .Index(t => t.CurrentNode_Identifier);
            
            CreateTable(
                "dbo.NodeModels",
                c => new
                    {
                        Identifier = c.String(nullable: false, maxLength: 128),
                        OperationType = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        WorkflowDefinitionModel_Identifier = c.String(maxLength: 128),
                        WorkflowDefinitionModel_Identifier1 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Identifier)
                .ForeignKey("dbo.NodeModels", t => t.WorkflowDefinitionModel_Identifier)
                .ForeignKey("dbo.NodeModels", t => t.WorkflowDefinitionModel_Identifier1)
                .Index(t => t.WorkflowDefinitionModel_Identifier)
                .Index(t => t.WorkflowDefinitionModel_Identifier1);
            
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
                        NodeModel_Identifier1 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Identifier)
                .ForeignKey("dbo.NodeModels", t => t.NodeModel_Identifier)
                .ForeignKey("dbo.NodeModels", t => t.NodeModel_Identifier1)
                .Index(t => t.NodeModel_Identifier)
                .Index(t => t.NodeModel_Identifier1);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExecutionModels", "CurrentNode_Identifier", "dbo.NodeModels");
            DropForeignKey("dbo.NodeModels", "WorkflowDefinitionModel_Identifier1", "dbo.NodeModels");
            DropForeignKey("dbo.NodeModels", "WorkflowDefinitionModel_Identifier", "dbo.NodeModels");
            DropForeignKey("dbo.TransitionModels", "NodeModel_Identifier1", "dbo.NodeModels");
            DropForeignKey("dbo.TransitionModels", "NodeModel_Identifier", "dbo.NodeModels");
            DropForeignKey("dbo.ExecutionModels", "Parent_Identifier", "dbo.ExecutionModels");
            DropIndex("dbo.TransitionModels", new[] { "NodeModel_Identifier1" });
            DropIndex("dbo.TransitionModels", new[] { "NodeModel_Identifier" });
            DropIndex("dbo.NodeModels", new[] { "WorkflowDefinitionModel_Identifier1" });
            DropIndex("dbo.NodeModels", new[] { "WorkflowDefinitionModel_Identifier" });
            DropIndex("dbo.ExecutionModels", new[] { "CurrentNode_Identifier" });
            DropIndex("dbo.ExecutionModels", new[] { "Parent_Identifier" });
            DropTable("dbo.TransitionModels");
            DropTable("dbo.NodeModels");
            DropTable("dbo.ExecutionModels");
        }
    }
}
