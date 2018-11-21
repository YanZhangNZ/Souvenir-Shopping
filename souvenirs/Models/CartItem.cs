using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace souvenirs.Models
{
    public class CartItem
    {
        public int CartItemID { get; set; }
        public string ShoppingCartID { get; set; }
        public int SouvenirID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CartDate { get; set; }
        public int Quantity { get; set; }
        

        public Souvenir Souvenir { get; set; }
    }
}
