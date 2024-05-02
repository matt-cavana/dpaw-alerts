namespace dpaw_alerts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeparktable2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Locations", "Longitude", c => c.Double(nullable: false));
            AlterColumn("dbo.Locations", "Latitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Locations", "Latitude", c => c.Single(nullable: false));
            AlterColumn("dbo.Locations", "Longitude", c => c.Single(nullable: false));
        }
    }
}
