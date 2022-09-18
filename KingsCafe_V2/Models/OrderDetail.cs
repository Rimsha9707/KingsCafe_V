namespace KingsCafe_V2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderDetail
    {
        public int DetailsID { get; set; }
        public int OrderFID { get; set; }
        public int ItemFID { get; set; }
        public int Quantity { get; set; }
    }
}
