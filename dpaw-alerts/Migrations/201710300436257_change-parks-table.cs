namespace dpaw_alerts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeparkstable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Parks", "Longitude", c => c.Single(nullable: false));
            AlterColumn("dbo.Parks", "Latitude", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Parks", "Latitude", c => c.Double(nullable: false));
            AlterColumn("dbo.Parks", "Longitude", c => c.Double(nullable: false));
        }
    }
}
