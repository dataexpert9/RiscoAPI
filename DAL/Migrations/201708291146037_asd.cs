namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asd : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Stores", "StoreDeliveryHours_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stores", "StoreDeliveryHours_Id", c => c.Int());
        }
    }
}
