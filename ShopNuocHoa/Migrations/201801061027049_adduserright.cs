namespace ShopNuocHoa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adduserright : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "UserRight", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "UserRight");
        }
    }
}
