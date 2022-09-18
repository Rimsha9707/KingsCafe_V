namespace KingsCafe_V2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Ingredient
    {
        public int IngredientID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
    }
}
