using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach_62133314.Models;

namespace WebBanSach_62133314.Controllers
{
    public class Customer_62133314Controller : Controller
    {
        private WebBanSach_62133314Entities _context = new WebBanSach_62133314Entities();
        public ActionResult Index()
        {
            var customers = _context.Customers.ToList();
            return View(customers);
        }


        public ActionResult Details(int id)
        {

            Customer customer = _context.Customers.Find(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            var customerOrders = _context.OrderProes.Where(o => o.IDCus == id).ToList();
            ViewBag.Customer = customer;
            return View(customerOrders);
        }
    }
}