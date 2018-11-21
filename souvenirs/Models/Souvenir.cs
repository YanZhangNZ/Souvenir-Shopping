using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace souvenirs.Models
{
    public class Souvenir
    {
        public int ID { get; set; }

        public string Name { get; set; }


        public decimal Price { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public int SupplierID { get; set; }

        public Category Category { get; set; }
        public Supplier Supplier { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

    }
}
