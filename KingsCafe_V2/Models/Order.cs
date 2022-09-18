namespace KingsCafe_V2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Order
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public TimeSpan? OrderTime { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public String Name { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public String Address { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
    }
}
