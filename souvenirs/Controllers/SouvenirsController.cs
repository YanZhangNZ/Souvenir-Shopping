using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using souvenirs.Data;
using souvenirs.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace souvenirs.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SouvenirsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;

        public SouvenirsController(ApplicationDbContext context,IHostingEnvironment hEnv)
        {

            _context = context;
            _hostingEnv = hEnv;
        }

        // GET: Souvenirs
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            
            
            if(searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var souvenirs = from s in _context.Souvenirs
                            select s;

            souvenirs = souvenirs.Include(s=>s.Category).Include(s=>s.Supplier);

            if (!String.IsNullOrEmpty(searchString))
            {
                souvenirs = souvenirs.Where(s => s.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    souvenirs = souvenirs.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    souvenirs = souvenirs.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    souvenirs = souvenirs.OrderByDescending(s => s.Price);
                    break;
                default:
                    souvenirs = souvenirs.OrderBy(s => s.Name);
                    break;
            }
            int pageSize = 3;
            return View(await PaginatedList<Souvenir>.CreateAsync(souvenirs.AsNoTracking(), page ?? 1, pageSize));
            //return View(await souvenirs.AsNoTracking().ToListAsync());
        }

        /*public async Task<IActionResult> CustomerIndex(int? categoryId, string currentFilter,string searchString,int? page,decimal? minPrice,decimal? maxPrice)
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

            
            if(categoryId.HasValue)
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
        }*/

        // GET: Souvenirs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var souvenir = await _context.Souvenirs
                .SingleOrDefaultAsync(m => m.ID == id);
            if (souvenir == null)
            {
                return NotFound();
            }

            return View(souvenir);
        }

        // GET: Souvenirs/Create
        public IActionResult Create()
        {
            PopulateCategoriesDropDownList();
            PopulateSuppliersDropDownList();
            return View();
        }

        // POST: Souvenirs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        /*public async Task<IActionResult> Create([Bind("ID,Name,Price,Image,Description,CategoryID,SupplierID")] Souvenir souvenir)
        {
            if (ModelState.IsValid)
            {
                _context.Add(souvenir);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(souvenir);
        }*/

        public async Task<IActionResult> Create([Bind("Name, Price, Description, CategoryID, SupplierID")] Souvenir souvenir,IList<IFormFile> _files)
        {
            var relativeName = "";
            var fileName = "";

            if(_files.Count < 1)
            {
                relativeName = "/Images/Souvenir/Default.jpg";
            }
            else
            {
                foreach (var file in _files)
                {
                    fileName = ContentDispositionHeaderValue
                                .Parse(file.ContentDisposition)
                                .FileName
                                .Trim('"');
                    relativeName = "/Images/Souvenir/" + DateTime.Now.ToString("ddMMyyyy-HHmmssffffff") + fileName;

                    using (FileStream fs =
                        System.IO.File.Create(_hostingEnv.WebRootPath + relativeName))
                    {
                        await file.CopyToAsync(fs);
                        fs.Flush();
                    }
                }
            }
            souvenir.Image = relativeName;
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(souvenir);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("","Unable to save changes." + "Try again and if the problem persists" + "see your system administrator.");
            }
            PopulateCategoriesDropDownList(souvenir.CategoryID);
            PopulateSuppliersDropDownList(souvenir.SupplierID);
            return View(souvenir);
        }

        // GET: Souvenirs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var souvenir = await _context.Souvenirs.SingleOrDefaultAsync(m => m.ID == id);
            var souvenir = await _context.Souvenirs.AsNoTracking().SingleOrDefaultAsync(m => m.ID == id);

            if (souvenir == null)
            {
                return NotFound();
            }
            PopulateCategoriesDropDownList(souvenir.CategoryID);
            PopulateSuppliersDropDownList(souvenir.SupplierID);
            return View(souvenir);
        }

        // POST: Souvenirs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Price,Description,CategoryID,SupplierID")] Souvenir souvenir, IList<IFormFile> _files)
        //public async Task<IActionResult> EditPost(int? id)
        {
            var relativeName = "";
            var fileName = "";
            if(id == null)
            {
                return NotFound();
            }
            var souvenirToUpdate = await _context.Souvenirs
                .SingleOrDefaultAsync(s => s.ID == id);
            if(await TryUpdateModelAsync<Souvenir>(souvenirToUpdate,"", s => s.CategoryID, s=>s.SupplierID, s=>s.Name, s=>s.Price,s=>s.Description))
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes." + "Try again, and if the problem persists," + "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
     
            }
            if (id != souvenir.ID)
            {
                return NotFound();
            }
            if (_files.Count < 1)
            {
                relativeName = "/Images/Souvenir/Default.jpg";
            }
            else
            {
                foreach (var file in _files)
                {
                    fileName = ContentDispositionHeaderValue
                                .Parse(file.ContentDisposition)
                                .FileName
                                .Trim('"');
                    relativeName = "/Images/Souvenir/" + DateTime.Now.ToString("ddMMyyyy-HHmmssffffff") + fileName;

                    using (FileStream fs =
                        System.IO.File.Create(_hostingEnv.WebRootPath + relativeName))
                    {
                        await file.CopyToAsync(fs);
                        fs.Flush();
                    }
                }
            }
            souvenir.Image = relativeName;

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
            PopulateCategoriesDropDownList(souvenirToUpdate.CategoryID);
            PopulateSuppliersDropDownList(souvenirToUpdate.SupplierID);
            return View(souvenirToUpdate);
        }
        private void PopulateSuppliersDropDownList(object selectedSupplier = null)
        {
            var suppliersQuery = from s in _context.Suppliers
                                 orderby s.SupplierName
                                 select s;
            ViewBag.SupplierID = new SelectList(suppliersQuery.AsNoTracking(), "ID", "SupplierName",
                selectedSupplier);

        }

        private void PopulateCategoriesDropDownList(object selectedCategory = null)
        {
            var categoriesQuery = from c in _context.Categories
                                  orderby c.ID
                                  select c;
            ViewBag.CategoryID = new SelectList(categoriesQuery.AsNoTracking(), "ID", "CategoryName",
                selectedCategory);
        }
        // GET: Souvenirs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var souvenir = await _context.Souvenirs
                .SingleOrDefaultAsync(m => m.ID == id);
            if (souvenir == null)
            {
                return NotFound();
            }

            return View(souvenir);
        }

        // POST: Souvenirs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var souvenir = await _context.Souvenirs.SingleOrDefaultAsync(m => m.ID == id);
            _context.Souvenirs.Remove(souvenir);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException s)
            {
                TempData["OrderUsed"] = "The souvenir being delete has been used in previous orders. Delete those orders before trying again.";
                return RedirectToAction("Delete");
            }
            return RedirectToAction("Index");
            
        }

        private bool SouvenirExists(int id)
        {
            return _context.Souvenirs.Any(e => e.ID == id);
        }






    }
}
