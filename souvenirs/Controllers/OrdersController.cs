using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using souvenirs.Data;
using souvenirs.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace souvenirs.Controllers
{
    [Authorize(Roles = "Admin, Member")]

    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.Orders.Include(o => o.Customer);
            return View(await _context.Orders.Include(i => i.User).AsNoTracking().ToListAsync());
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> CustomerIndex()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            return View(await _context.Orders.Where(o=>o.UserID==user.Id).AsNoTracking().ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
         {
             if (id == null)
             {
                 return NotFound();
             }

             var order = await _context.Orders
                 .Include(o => o.User).
                 Include(o=>o.OrderItems).ThenInclude(o=>o.Souvenir)
                 .ThenInclude(o=>o.Category)
                 .SingleOrDefaultAsync(m => m.OrderID == id);
             if (order == null)
             {
                 return NotFound();
             }

             return View(order);
         }

        // GET: Orders/Create
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderID,OrderDate,Status,GST,Subtotal,GrandTotal,CustomerID")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "CustomerName", order.CustomerID);
            return View(order);
        }*/
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> CreatePost()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            Order order = new Order();

            ShoppingCart cart = ShoppingCart.GetCart(this.HttpContext);
            List<CartItem> items = cart.GetCartItems(_context);
            List<OrderItem> details = new List<OrderItem>();
            foreach (CartItem item in items)
            {

                OrderItem detail = CreateOrderDetailForThisItem(item);
                detail.Order = order;
                details.Add(detail);
                _context.Add(detail);

            }
            order.Status = "Waiting";
            order.User = user;
            order.OrderDate = DateTime.Now;
            order.Subtotal = ShoppingCart.GetCart(this.HttpContext).GetTotal(_context);
            order.GrandTotal = order.Subtotal * 1.15m;
            order.GST = order.Subtotal * 0.15m;
            order.OrderItems = details;
            _context.SaveChanges();


            return RedirectToAction("Purchased", new RouteValueDictionary(
            new { action = "Purchased", id = order.OrderID }));


            return View(order);
        }


        private OrderItem CreateOrderDetailForThisItem(CartItem item)
        {

            OrderItem detail = new OrderItem();

            detail.Quantity = item.Quantity;
            detail.Souvenir = item.Souvenir;
            detail.OrderitemPrice = item.Souvenir.Price;

            return detail;

        }
        public async Task<IActionResult> Purchased(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include(i => i.User).AsNoTracking().SingleOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            var details = _context.OrderItems.Where(detail => detail.Order.OrderID == order.OrderID).Include(detail => detail.Souvenir).ToList();

            order.OrderItems = details;
            ShoppingCart.GetCart(this.HttpContext).EmptyCart(_context);
            return View(order);
        }




        // GET: Orders/Edit/5
        /* public async Task<IActionResult> Edit(int? id)
         {
             if (id == null)
             {
                 return NotFound();
             }

             var order = await _context.Orders.SingleOrDefaultAsync(m => m.OrderID == id);
             if (order == null)
             {
                 return NotFound();
             }
             ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "CustomerName", order.CustomerID);
             return View(order);
         }*/

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /* [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> Edit(int id, [Bind("OrderID,OrderDate,Status,GST,Subtotal,GrandTotal,CustomerID")] Order order)
         {
             if (id != order.OrderID)
             {
                 return NotFound();
             }

             if (ModelState.IsValid)
             {
                 try
                 {
                     _context.Update(order);
                     await _context.SaveChangesAsync();
                 }
                 catch (DbUpdateConcurrencyException)
                 {
                     if (!OrderExists(order.OrderID))
                     {
                         return NotFound();
                     }
                     else
                     {
                         throw;
                     }
                 }
                 return RedirectToAction(nameof(Index));
             }
             ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "CustomerName", order.CustomerID);
             return View(order);
         }*/

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }
            var details = _context.OrderItems.Where(detail => detail.Order.OrderID == order.OrderID).Include(detail => detail.Souvenir).ToList();
            order.OrderItems = details;

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(m => m.OrderID == id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditOrderStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.SingleOrDefaultAsync(m => m.OrderID == id);
            if ( order== null)
            {
                return NotFound();
            }
            order.Status = "Shipped";
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
}
