using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace souvenirs.Models
{
    public class Order
    {
        [DisplayName("Order Number")]
        public int OrderID { get; set; }

        [DisplayName("Order Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}",ApplyFormatInEditMode =true)]
        public System.DateTime OrderDate { get; set; }

        
        public string Status { get; set; }
        public decimal GST { get; set; }
        public decimal Subtotal { get; set; }
        public decimal GrandTotal { get; set; }
        public int CustomerID { get; set; }
        public string UserID { get; set; }



        public List<OrderItem> OrderItems { get; set; }
        public ApplicationUser User { get; set; }

    }
}
