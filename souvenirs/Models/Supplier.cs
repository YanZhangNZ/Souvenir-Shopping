using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace souvenirs.Models
{
    public class Supplier
    {
        public int ID { get; set;}
        public string SupplierName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }

        public ICollection<Souvenir> Souveniers { get; set; }
    }
}
