using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBanSach_62133314.Models;

namespace WebBanSach_62133314.Controllers
{
    public class Products_62133314Controller : Controller
    {
        private WebBanSach_62133314Entities _context = new WebBanSach_62133314Entities();

        public async Task<ActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult Create()
        {
            ViewBag.Category = new SelectList(_context.Categories, "IDCate", "NameCate");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProductID,NamePro,DecriptionPro,IDCate,Price,ImagePro")] Product product, HttpPostedFileBase ImagePro)
        {

            if (ModelState.IsValid)
            {
                if (ImagePro != null)
                {
                    var fileName = Path.GetFileName(ImagePro.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    string directoryPath = Server.MapPath("~/Content/Images");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    product.ImagePro = fileName;
                    ImagePro.SaveAs(path);
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Category = new SelectList(_context.Categories, "IDCate", "NameCate", product.Category);
            return View(product);
        }
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Category = new SelectList(_context.Categories, "IDCate", "NameCate", product.Category);
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ProductID,NamePro,DecriptionPro,IDCate,Price,ImagePro")] Product product, HttpPostedFileBase ImagePro)
        {
            System.Diagnostics.Debug.WriteLine(product.IDCate.ToString());
            if (ModelState.IsValid)
            {
                var product_context = await _context.Products.FindAsync(product.ProductID);
                if (product_context != null)
                {
                    product_context.NamePro = product.NamePro;
                    product_context.DecriptionPro = product.DecriptionPro;
                    product_context.Price = product.Price;
                    product_context.IDCate = product.IDCate;
                    if (ImagePro != null)
                    {
                        var fileName = Path.GetFileName(ImagePro.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                        string directoryPath = Server.MapPath("~/Content/Images");
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        product_context.ImagePro = fileName;
                        ImagePro.SaveAs(path);
                    }

                    product_context.Category = product.Category;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Category = new SelectList(_context.Categories, "IDCate", "NameCate", product.Category);
            return View(product);
        }
        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(c => c.ProductID == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Loại sản phẩm không tồn tại." });
            }
            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = "Không thể xóa sản phẩm có liên kết." });
            }

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
