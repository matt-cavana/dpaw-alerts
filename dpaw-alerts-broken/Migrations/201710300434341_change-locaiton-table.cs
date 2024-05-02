namespace dpaw_alerts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changelocaitontable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Locations", "Longitude", c => c.Single(nullable: false));
            AlterColumn("dbo.Locations", "Latitude", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Locations", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Locations", "Longitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
