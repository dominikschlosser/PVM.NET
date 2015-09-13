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
                        CurrentNodeIdentifier = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsFinished = c.Boolean(nullable: false),
                        IncomingTransition = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Parent_Identifier = c.String(maxLength: 128),
                        WorkflowDefinition_Identifier = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Identifier)
                .ForeignKey("dbo.ExecutionModels", t => t.Parent_Identifier)
                .ForeignKey("dbo.NodeModels", t => t.WorkflowDefinition_Identifier)
                .Index(t => t.Parent_Identifier)
                .Index(t => t.WorkflowDefinition_Identifier);
            
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
                        IsEndNode = c.Boolean(nullable: false),
                        IsInitialNode = c.Boolean(nullable: false),
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
                        IsDefault = c.Boolean(nullable: false),
                        NodeModel_Identifier = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Identifier)
                .ForeignKey("dbo.NodeModels", t => t.NodeModel_Identifier)
                .Index(t => t.NodeModel_Identifier);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExecutionModels", "WorkflowDefinition_Identifier", "dbo.NodeModels");
            DropForeignKey("dbo.NodeModels", "WorkflowDefinitionModel_Identifier", "dbo.NodeModels");
            DropForeignKey("dbo.TransitionModels", "NodeModel_Identifier", "dbo.NodeModels");
            DropForeignKey("dbo.ExecutionVariableModels", "ExecutionModel_Identifier", "dbo.ExecutionModels");
            DropForeignKey("dbo.ExecutionModels", "Parent_Identifier", "dbo.ExecutionModels");
            DropIndex("dbo.TransitionModels", new[] { "NodeModel_Identifier" });
            DropIndex("dbo.NodeModels", new[] { "WorkflowDefinitionModel_Identifier" });
            DropIndex("dbo.ExecutionVariableModels", new[] { "ExecutionModel_Identifier" });
            DropIndex("dbo.ExecutionModels", new[] { "WorkflowDefinition_Identifier" });
            DropIndex("dbo.ExecutionModels", new[] { "Parent_Identifier" });
            DropTable("dbo.TransitionModels");
            DropTable("dbo.NodeModels");
            DropTable("dbo.ExecutionVariableModels");
            DropTable("dbo.ExecutionModels");
        }
    }
}
