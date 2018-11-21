using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using souvenirs.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace souvenirs.Models
{
    public class ShoppingCart
    {
        public string ShoppingCartID { get; set; }
        public const string CartSessionKey = "CartID";
        public static ShoppingCart GetCart(HttpContext context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartID = cart.GetCartID(context);
            return cart;
        }

        public void AddToCart(Souvenir souvenir, ApplicationDbContext db)
        {
            var cartItem = db.CartItem.SingleOrDefault(
                c => c.ShoppingCartID == ShoppingCartID && c.Souvenir.ID == souvenir.ID);
            if(cartItem == null)
            {
                cartItem = new CartItem
                {
                    Souvenir = souvenir,
                    ShoppingCartID = ShoppingCartID,
                    Quantity = 1,
                    CartDate = DateTime.Now
                };
                db.CartItem.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
            }
            db.SaveChanges();
        }

        public int RemoveFromCart(int id,ApplicationDbContext db)
        {
            var cartItem = db.CartItem.SingleOrDefault(
                cart => cart.ShoppingCartID == ShoppingCartID && cart.Souvenir.ID == id);
            int itemCount = 0;
            if(cartItem != null)
            {
                if(cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                    itemCount = cartItem.Quantity;
                }
                else
                {
                    db.CartItem.Remove(cartItem);
                }
                db.SaveChanges();
            }
            return itemCount;
        }

        public void EmptyCart(ApplicationDbContext db)
        {
            var cartItems = db.CartItem.Where(cart => cart.ShoppingCartID == ShoppingCartID);
            foreach(var cartItem in cartItems)
            {
                db.CartItem.Remove(cartItem);
            }
            db.SaveChanges();
        }

        public List<CartItem> GetCartItems(ApplicationDbContext db)
        {
            List<CartItem> cartItems = db.CartItem.Include(
                i => i.Souvenir).ThenInclude(i=>i.Category).Where(CartItem => CartItem.ShoppingCartID == ShoppingCartID).ToList();
            return cartItems;
        }

        public int GetCount(ApplicationDbContext db)
        {
            int? count =
                (from cartItems in db.CartItem where cartItems.ShoppingCartID == ShoppingCartID select (int?)cartItems.Quantity).Sum();
            return count ?? 0;
        }

        public decimal GetTotal(ApplicationDbContext db)
        {
            decimal? total = (from cartItems in db.CartItem
                              where cartItems.ShoppingCartID == ShoppingCartID
                              select (int?)cartItems.Quantity * cartItems.Souvenir.Price).Sum();
            return total ?? decimal.Zero;
        }

        public string GetCartID(HttpContext context)
        {
            if(context.Session.GetString(CartSessionKey) == null)
            {
                Guid tempCartID = Guid.NewGuid();
                context.Session.SetString(CartSessionKey, tempCartID.ToString());
            }
            return context.Session.GetString(CartSessionKey).ToString();
        }
    }
}
