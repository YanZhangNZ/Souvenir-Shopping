using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace souvenirs.Models
{
    public class Customer
    {
        public int ID { get; set; }

        [Required]
        [StringLength(10)]
        public string CustomerName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }


        [Required]
        public string EmailAddress { get; set; }
    }
}
