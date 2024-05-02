namespace dpaw_alerts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeparktable : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.AlertFiles",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            AlertId = c.Guid(nullable: false),
            //            FileTitle = c.String(nullable: false),
            //            FilePath = c.String(nullable: false),
            //            FileSize = c.String(),
            //            FileType = c.String(),
            //            CreateDate = c.DateTime(nullable: false),
            //            CreatedBy = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Alerts", t => t.AlertId, cascadeDelete: true)
            //    .Index(t => t.AlertId);
            
            //CreateTable(
            //    "dbo.Alerts",
            //    c => new
            //        {
            //            AlertId = c.Guid(nullable: false, identity: true),
            //            Title = c.String(nullable: false, maxLength: 160),
            //            Description = c.String(nullable: false),
            //            StartDate = c.DateTime(nullable: false),
            //            EndDate = c.DateTime(),
            //            SitesAffected = c.String(maxLength: 200),
            //            Facebook = c.String(maxLength: 10),
            //            SMS = c.String(maxLength: 10),
            //            Twitter = c.String(maxLength: 10),
            //            PushNotification = c.String(maxLength: 10),
            //            Email = c.String(maxLength: 10),
            //            PubImmediately = c.Boolean(nullable: false),
            //            Published = c.String(maxLength: 10),
            //            WebLink = c.String(maxLength: 200),
            //            CreateDate = c.DateTime(nullable: false),
            //            CreatedBy = c.String(),
            //            UpdateDate = c.DateTime(),
            //            UpdatedBy = c.String(maxLength: 160),
            //            AId = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.AlertId)
            //    .ForeignKey("dbo.AlertTypes", t => t.AId, cascadeDelete: true)
            //    .Index(t => t.AId);
            
            //CreateTable(
            //    "dbo.AlertTypes",
            //    c => new
            //        {
            //            AId = c.Int(nullable: false, identity: true),
            //            Name = c.String(nullable: false),
            //            Description = c.String(),
            //            Slug = c.String(maxLength: 60),
            //            Status = c.String(nullable: false, maxLength: 12),
            //            IconUrl = c.String(),
            //            CreateDate = c.DateTime(nullable: false),
            //            CreatedBy = c.String(),
            //        })
            //    .PrimaryKey(t => t.AId);
            
            //CreateTable(
            //    "dbo.UserAlertTypes",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            UserId = c.Guid(nullable: false),
            //            AId = c.Int(nullable: false),
            //            GrantDate = c.DateTime(nullable: false),
            //            GrantedBy = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.AlertTypes", t => t.AId, cascadeDelete: true)
            //    .Index(t => t.AId);
            
            //CreateTable(
            //    "dbo.Locations",
            //    c => new
            //        {
            //            LocId = c.Int(nullable: false, identity: true),
            //            Name = c.String(nullable: false, maxLength: 120),
            //            AlertId = c.Guid(nullable: false),
            //            RPrkId = c.Int(),
            //            Longitude = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            Latitude = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            Contact = c.String(),
            //            Email = c.String(maxLength: 120),
            //        })
            //    .PrimaryKey(t => t.LocId)
            //    .ForeignKey("dbo.Alerts", t => t.AlertId, cascadeDelete: true)
            //    .Index(t => t.AlertId);
            
            //CreateTable(
            //    "dbo.Apis",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            AId = c.Int(nullable: false),
            //            Description = c.String(nullable: false, maxLength: 200),
            //            Token = c.Guid(nullable: false),
            //            ExpiryDate = c.DateTime(nullable: false),
            //            CreateDate = c.DateTime(nullable: false),
            //            CreatedBy = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.AlertTypes", t => t.AId, cascadeDelete: true)
            //    .Index(t => t.AId);
            
            //CreateTable(
            //    "dbo.Contacts",
            //    c => new
            //        {
            //            ContactId = c.Int(nullable: false, identity: true),
            //            OfficeName = c.String(nullable: false),
            //            Email = c.String(nullable: false),
            //            Phone = c.String(),
            //            Address = c.String(nullable: false),
            //            OfficeHours = c.String(),
            //            CreateDate = c.DateTime(nullable: false),
            //            CreatedBy = c.String(),
            //            LastUpdate = c.DateTime(),
            //            UpdatedBy = c.String(),
            //        })
            //    .PrimaryKey(t => t.ContactId);
            
            //CreateTable(
            //    "dbo.EmailTemplates",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            From = c.String(nullable: false),
            //            HeaderSection = c.String(nullable: false),
            //            FooterSection = c.String(nullable: false),
            //            CreateDate = c.DateTime(nullable: false),
            //            CreatedBy = c.String(),
            //            UpdateDate = c.DateTime(),
            //            UpdatedBy = c.String(maxLength: 160),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.Errors",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Source = c.String(maxLength: 120),
            //            Description = c.String(),
            //            Date = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.Parks",
            //    c => new
            //        {
            //            ParkId = c.Int(nullable: false, identity: true),
            //            RPrkId = c.Int(nullable: false),
            //            Name = c.String(nullable: false, maxLength: 120),
            //            Region = c.String(),
            //            District = c.String(nullable: false),
            //            Description = c.String(nullable: false),
            //            Tenure = c.String(nullable: false),
            //            Longitude = c.Double(nullable: false),
            //            Latitude = c.Double(nullable: false),
            //            ContactId = c.Int(),
            //            Status = c.String(nullable: false),
            //            CreateDate = c.DateTime(nullable: false),
            //            CreatedBy = c.String(),
            //        })
            //    .PrimaryKey(t => t.ParkId)
            //    .ForeignKey("dbo.Contacts", t => t.ContactId)
            //    .Index(t => t.ContactId);
            
            //CreateTable(
            //    "dbo.AspNetRoles",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            Name = c.String(nullable: false, maxLength: 256),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            //CreateTable(
            //    "dbo.AspNetUserRoles",
            //    c => new
            //        {
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            RoleId = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => new { t.UserId, t.RoleId })
            //    .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.UserId)
            //    .Index(t => t.RoleId);
            
            //CreateTable(
            //    "dbo.ScheduleJobs",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            AlertId = c.Guid(nullable: false),
            //            Channel = c.String(maxLength: 60),
            //            ExecutionDate = c.DateTime(nullable: false),
            //            Status = c.String(maxLength: 20),
            //            CompletedDate = c.DateTime(),
            //            Response = c.String(),
            //            CreateDate = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Alerts", t => t.AlertId, cascadeDelete: true)
            //    .Index(t => t.AlertId);
            
            //CreateTable(
            //    "dbo.SocialNetworks",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Channel = c.String(),
            //            AppId = c.String(nullable: false),
            //            AppSecret = c.String(nullable: false),
            //            AccessToken = c.String(nullable: false),
            //            LongLifeAccessToken = c.String(),
            //            ExpiryDate = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.Subscribers",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            FirstName = c.String(nullable: false, maxLength: 60),
            //            LastName = c.String(nullable: false, maxLength: 60),
            //            Email = c.String(nullable: false, maxLength: 120),
            //            Mobile = c.String(maxLength: 9),
            //            SubscriptionType = c.String(nullable: false, maxLength: 120),
            //            SubscriptionDate = c.DateTime(),
            //            Status = c.String(),
            //            CreateDate = c.DateTime(nullable: false),
            //            CreatedBy = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.UserActivities",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Action = c.String(),
            //            UserName = c.String(),
            //            Message = c.String(),
            //            IpAddress = c.String(),
            //            MetaData = c.String(),
            //            ActionDate = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.AspNetUsers",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            FirstName = c.String(),
            //            LastName = c.String(),
            //            Email = c.String(maxLength: 256),
            //            EmailConfirmed = c.Boolean(nullable: false),
            //            PasswordHash = c.String(),
            //            SecurityStamp = c.String(),
            //            PhoneNumber = c.String(),
            //            PhoneNumberConfirmed = c.Boolean(nullable: false),
            //            TwoFactorEnabled = c.Boolean(nullable: false),
            //            LockoutEndDateUtc = c.DateTime(),
            //            LockoutEnabled = c.Boolean(nullable: false),
            //            AccessFailedCount = c.Int(nullable: false),
            //            UserName = c.String(nullable: false, maxLength: 256),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            //CreateTable(
            //    "dbo.AspNetUserClaims",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            ClaimType = c.String(),
            //            ClaimValue = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.UserId);
            
            //CreateTable(
            //    "dbo.AspNetUserLogins",
            //    c => new
            //        {
            //            LoginProvider = c.String(nullable: false, maxLength: 128),
            //            ProviderKey = c.String(nullable: false, maxLength: 128),
            //            UserId = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.ScheduleJobs", "AlertId", "dbo.Alerts");
            //DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            //DropForeignKey("dbo.Parks", "ContactId", "dbo.Contacts");
            //DropForeignKey("dbo.Apis", "AId", "dbo.AlertTypes");
            //DropForeignKey("dbo.Locations", "AlertId", "dbo.Alerts");
            //DropForeignKey("dbo.AlertFiles", "AlertId", "dbo.Alerts");
            //DropForeignKey("dbo.Alerts", "AId", "dbo.AlertTypes");
            //DropForeignKey("dbo.UserAlertTypes", "AId", "dbo.AlertTypes");
            //DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            //DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            //DropIndex("dbo.AspNetUsers", "UserNameIndex");
            //DropIndex("dbo.ScheduleJobs", new[] { "AlertId" });
            //DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            //DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            //DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            //DropIndex("dbo.Parks", new[] { "ContactId" });
            //DropIndex("dbo.Apis", new[] { "AId" });
            //DropIndex("dbo.Locations", new[] { "AlertId" });
            //DropIndex("dbo.UserAlertTypes", new[] { "AId" });
            //DropIndex("dbo.Alerts", new[] { "AId" });
            //DropIndex("dbo.AlertFiles", new[] { "AlertId" });
            //DropTable("dbo.AspNetUserLogins");
            //DropTable("dbo.AspNetUserClaims");
            //DropTable("dbo.AspNetUsers");
            //DropTable("dbo.UserActivities");
            //DropTable("dbo.Subscribers");
            //DropTable("dbo.SocialNetworks");
            //DropTable("dbo.ScheduleJobs");
            //DropTable("dbo.AspNetUserRoles");
            //DropTable("dbo.AspNetRoles");
            //DropTable("dbo.Parks");
            //DropTable("dbo.Errors");
            //DropTable("dbo.EmailTemplates");
            //DropTable("dbo.Contacts");
            //DropTable("dbo.Apis");
            //DropTable("dbo.Locations");
            //DropTable("dbo.UserAlertTypes");
            //DropTable("dbo.AlertTypes");
            //DropTable("dbo.Alerts");
            //DropTable("dbo.AlertFiles");
        }
    }
}
