namespace StudentManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLecturerTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Lecturers",
                c => new
                    {
                        LecturerId = c.Int(nullable: false, identity: true),
                        StaffNumber = c.String(nullable: false),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Phone = c.String(),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.LecturerId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lecturers", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Lecturers", new[] { "UserId" });
            DropTable("dbo.Lecturers");
        }
    }
}
