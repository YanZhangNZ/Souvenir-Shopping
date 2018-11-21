using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using souvenirs.Data;
using souvenirs.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace souvenirs.Controllers
{
    [AllowAnonymous]
    [Authorize(Roles = "Member")]

    public class MemberSouvenirController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemberSouvenirController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MemberSouvenir
        public async Task<IActionResult> Index(int? categoryId, string currentFilter, string searchString, int? page, decimal? minPrice, decimal? maxPrice)
        {

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryId;//put category id into current filter
            ViewData["minPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;

            var souvenirs = from s in _context.Souvenirs
                            select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                souvenirs = souvenirs.Where(s => s.Name.Contains(searchString));
            }

            //return View(await _context.Souvenirs.ToListAsync());
            if (categoryId.HasValue)
            {
                souvenirs = souvenirs.Where(s => s.CategoryID == categoryId);
            }
            if (minPrice.HasValue)
            {
                souvenirs = souvenirs.Where(s => s.Price >= minPrice);
            }
            if (maxPrice.HasValue)
            {
                souvenirs = souvenirs.Where(s => s.Price <= maxPrice);
            }
            var categories = _context.Categories
                .AsNoTracking();

            ViewBag.Categories = categories;

            int pageSize = 3;
            return View(await PaginatedList<Souvenir>.CreateAsync(souvenirs.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: MemberSouvenir/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var souvenir = await _context.Souvenirs
                .Include(s => s.Category)
                .Include(s => s.Supplier)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (souvenir == null)
            {
                return NotFound();
            }

            return View(souvenir);
        }

        // GET: MemberSouvenir/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "ID");
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "ID", "ID");
            return View();
        }

        // POST: MemberSouvenir/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Price,Image,Description,CategoryID,SupplierID")] Souvenir souvenir)
        {
            if (ModelState.IsValid)
            {
                _context.Add(souvenir);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "ID", souvenir.CategoryID);
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "ID", "ID", souvenir.SupplierID);
            return View(souvenir);
        }

        // GET: MemberSouvenir/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var souvenir = await _context.Souvenirs.SingleOrDefaultAsync(m => m.ID == id);
            if (souvenir == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "ID", souvenir.CategoryID);
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "ID", "ID", souvenir.SupplierID);
            return View(souvenir);
        }

        // POST: MemberSouvenir/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Price,Image,Description,CategoryID,SupplierID")] Souvenir souvenir)
        {
            if (id != souvenir.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(souvenir);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SouvenirExists(souvenir.ID))
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
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "ID", souvenir.CategoryID);
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "ID", "ID", souvenir.SupplierID);
            return View(souvenir);
        }

        // GET: MemberSouvenir/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var souvenir = await _context.Souvenirs
                .Include(s => s.Category)
                .Include(s => s.Supplier)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (souvenir == null)
            {
                return NotFound();
            }

            return View(souvenir);
        }

        // POST: MemberSouvenir/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var souvenir = await _context.Souvenirs.SingleOrDefaultAsync(m => m.ID == id);
            _context.Souvenirs.Remove(souvenir);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SouvenirExists(int id)
        {
            return _context.Souvenirs.Any(e => e.ID == id);
        }
    }
}
