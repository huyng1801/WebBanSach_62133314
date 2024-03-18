using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebBanSach_62133314.Models;

namespace WebBanSach_62133314.Controllers
{
    public class Order_62133314Controller : Controller
    {
        private WebBanSach_62133314Entities _context = new WebBanSach_62133314Entities();

        // GET: Order
        public async Task<ActionResult> Index()
        {
            var orders = await _context.OrderProes.ToListAsync();
            return View(orders);
        }

        public async Task<ActionResult> Details(int id)
        {
            var order = await _context.OrderProes
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.ID == id);

            if (order == null)
            {
                return RedirectToAction("Index");
            }

            return View(order);
        }
    }
}
