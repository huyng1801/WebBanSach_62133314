using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebBanSach_62133314.Models;

namespace WebBanSach_62133314.Controllers
{
    public class Categories_62133314Controller : Controller
    {
        private WebBanSach_62133314Entities _context = new WebBanSach_62133314Entities();

        public async Task<ActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Category category)
        {
            try
            {
             
                if (_context.Categories.Any(c => c.NameCate == category.NameCate))
                {
                    ModelState.AddModelError("NameCate", "Tên thể loại đã tồn tại.");
                }

                if (ModelState.IsValid)
                {
                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(category);
                }
            }
            catch (Exception)
            {
                return View(category);
            }
        }


        public async Task<ActionResult> Details(int id)
        {
            var categoryWithProducts = await _context.Categories
                .Include(c => c.Products)  
                .FirstOrDefaultAsync(c => c.IDCate == id);

            if (categoryWithProducts == null)
            {
                return RedirectToAction("Index");
            }

            return View(categoryWithProducts);
        }


        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.IDCate == id);
            return View(category);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, Category category)
        {
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.IDCate == id);
            if (category == null)
            {
                return Json(new { success = false, message = "Loại sách không tồn tại." });
            }
            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = "Không thể xóa loại sách có liên kết." });
            }
           
        }

       
    }
}
