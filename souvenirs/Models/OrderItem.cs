using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace souvenirs.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }
       public int SouvenirID { get; set; }
       public int OrderID { get; set; }
        public decimal OrderitemPrice { get; set; }
        public int Quantity { get; set; }

        public Souvenir Souvenir { get; set; }
        public Order Order { get; set; }
    }
}
