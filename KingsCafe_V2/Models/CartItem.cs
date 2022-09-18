using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KingsCafe_V2.Models
{
    public class CartItem
    {
        public FoodItem foodItem {get; set;}
        public int quantity { get; set; }
    }
}