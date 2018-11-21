using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace souvenirs.Models
{
    public class Category
    {
        public int ID { get; set; }

        
        public string CategoryName { get; set; }

        public ICollection<Souvenir> Souvenirs { get; set; }
    }
}
