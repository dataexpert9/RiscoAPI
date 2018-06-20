namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asd3 : DbMigration
    {
        public override void Up()
        {
            //DropColumn("dbo.StoreDeliveryHours", "Store_Id");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.StoreDeliveryHours", "Store_Id", c => c.Int(nullable: false));
        }
    }
}
