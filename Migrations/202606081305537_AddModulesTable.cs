namespace StudentManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModulesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Modules",
                c => new
                    {
                        ModuleId = c.Int(nullable: false, identity: true),
                        ModuleCode = c.String(nullable: false),
                        ModuleName = c.String(nullable: false),
                        Credits = c.Int(nullable: false),
                        LecturerId = c.Int(),
                    })
                .PrimaryKey(t => t.ModuleId)
                .ForeignKey("dbo.Lecturers", t => t.LecturerId)
                .Index(t => t.LecturerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Modules", "LecturerId", "dbo.Lecturers");
            DropIndex("dbo.Modules", new[] { "LecturerId" });
            DropTable("dbo.Modules");
        }
    }
}
