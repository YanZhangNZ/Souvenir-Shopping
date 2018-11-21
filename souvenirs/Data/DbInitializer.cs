using souvenirs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace souvenirs.Data
{
    public class DbInitializer {


        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Categories.Any())
            {
                return;   // DB has been seeded
            }

            var categories = new Category[]
            {
                new Category
                {
                    CategoryName="Maori Gifts"
                },
                new Category
                {
                    CategoryName="T Shirts"
                },
                new Category
                {
                    CategoryName="Mugs"
                },
                new Category
                {
                    CategoryName="Wools"
                }

            };
            foreach (Category s in categories )
            {
                context.Categories.Add(s);
            }
            context.SaveChanges();

            var suppliers = new Supplier[]
            {
                new Supplier{SupplierName="AAA", PhoneNumber="1111", EmailAddress="a@a.com"},
                new Supplier{SupplierName="BBB", PhoneNumber="2222", EmailAddress="b@b.com"},
                new Supplier{SupplierName="CCC", PhoneNumber="3333", EmailAddress="c@c.com"},
                new Supplier{SupplierName="DDD", PhoneNumber="4444", EmailAddress="d@d.com"},
            };
            foreach(Supplier sp in suppliers)
            {
                context.Suppliers.Add(sp);            
            }
            context.SaveChanges();


           /* var customers = new Customer[]
            {
                new Customer{CustomerName="jjj",PhoneNumber="021233333",
                EmailAddress="aaaa@aaaa.com"},
                new Customer{CustomerName="qqq",PhoneNumber="021233333",
                EmailAddress="aabb@aabb.com"},
            };
            foreach(Customer cus in customers)
            {
                context.Customers.Add(cus);
            }
            context.SaveChanges();


            var souvenirs = new Souvenir[]
            {
                new Souvenir{Name="aMug",Price=11,
                CategoryID = categories.Single(c=>c.CategoryName=="Mugs").ID,
                SupplierID = suppliers.Single(s=>s.SupplierName=="AAA").ID},
                new Souvenir{Name="bMug",Price=7,
                CategoryID = categories.Single(c=>c.CategoryName=="Mugs").ID,
                SupplierID = suppliers.Single(s=>s.SupplierName=="BBB").ID},
                new Souvenir{Name="aT",Price=32,
                CategoryID = categories.Single(c=>c.CategoryName=="T Shirts").ID,
                SupplierID = suppliers.Single(s=>s.SupplierName=="CCC").ID},
                new Souvenir{Name="aMaori",Price=88,
                CategoryID = categories.Single(c=>c.CategoryName=="Maori Gifts").ID,
                SupplierID = suppliers.Single(s=>s.SupplierName=="DDD").ID},
            };
            foreach(Souvenir sv in souvenirs)
            {
                context.Souvenirs.Add(sv);
            }
            context.SaveChanges();


            var orders = new Order[]
            {
                new Order{Status="waiting",OrderDate=DateTime.Parse("2018-05-22"),
                CustomerID = customers.Single(i => i.CustomerName=="jjj").ID},
                new Order{Status="shipped",OrderDate=DateTime.Parse("2016-05-12"),
                CustomerID = customers.Single(i => i.CustomerName=="qqq").ID},
            };
            foreach(Order od in orders)
            {
                context.Orders.Add(od);
            }
            context.SaveChanges();*/

        }
    }

}

