namespace KingsCafe_V2.ViewModel
{
    using KingsCafe_V2.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderDetail_VM
    {
        public Order OrderFID { get; set; }
        public OrderDetail OrderDetails { get; set; }
        public Category CatFID { get; set; }
        public FoodItem ItemFID { get; set; }
    }
}
