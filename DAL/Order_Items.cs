namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Order_Items
    {
        public int Id { get; set; }

        [Required]
        public int Qty { get; set; }

        public int? Product_Id { get; set; }

        public int? Box_Id { get; set; }
        
        public int? Package_Id { get; set; }

        public int? Offer_Product_Id { get; set; }

        public int? Offer_Package_Id { get; set; }

        public int StoreOrder_Id { get; set; }

        public virtual StoreOrder StoreOrder { get; set; }

        [ForeignKey("Package_Id")]
        public virtual Package Package { get; set; }

        [ForeignKey("Product_Id")]
        public virtual Product Product { get; set; }

        [ForeignKey("Offer_Product_Id")]
        public virtual Offer_Products Offer_Product { get; set; }

        [ForeignKey("Offer_Package_Id")]
        public virtual Offer_Packages Offer_Package { get; set; }

        [ForeignKey("Box_Id")]
        public virtual Box Box { get; set; }

        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
    }
}
