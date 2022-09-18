namespace KingsCafe_V2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    public class Category
    {
        public int CatID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }
    }
}
