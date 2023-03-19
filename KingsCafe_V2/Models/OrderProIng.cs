
using System;
using System.Collections.Generic;
using System.Text;

namespace KingsCafeApp.Models
{
    public partial class OrderProIng
    {
        public int OrderProIngID { get; set; }
        public int OrderDetailFID { get; set; }
        public int IngredientFID { get; set; }

    }
}
