namespace EstiwTest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "CustomersId", c => c.Int(nullable: false));
            CreateIndex("dbo.Products", "CustomersId");
            AddForeignKey("dbo.Products", "CustomersId", "dbo.Customers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "CustomersId", "dbo.Customers");
            DropIndex("dbo.Products", new[] { "CustomersId" });
            DropColumn("dbo.Products", "CustomersId");
        }
    }
}
