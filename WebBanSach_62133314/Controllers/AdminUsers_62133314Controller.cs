using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks; 
using System.Web;
using System.Web.Mvc;
using WebBanSach_62133314.Models;

namespace WebBanSach_62133314.Controllers
{
    public class AdminUsers_62133314Controller : Controller
    {
        private WebBanSach_62133314Entities _context = new WebBanSach_62133314Entities();

        public ActionResult Index()
        {
            List<AdminUser> adminUsers = _context.AdminUsers.ToList();
            return View(adminUsers);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminUser adminUser)
        {
            // Check for duplicate NameUser
            if (_context.AdminUsers.Any(u => u.NameUser == adminUser.NameUser))
            {
                ModelState.AddModelError("NameUser", "Tên người dùng đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                _context.AdminUsers.Add(adminUser);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(adminUser);
        }


        public ActionResult Edit(int id)
        {
            AdminUser adminUser = _context.AdminUsers.Find(id);
            return View(adminUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminUser adminUser)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.AdminUsers.Find(adminUser.ID);

                if (existingUser != null)
                {
                    existingUser.NameUser = adminUser.NameUser;
                    existingUser.RoleUser = adminUser.RoleUser;
                    if (adminUser.PasswordUser != null)
                        existingUser.PasswordUser = adminUser.PasswordUser;
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(adminUser);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var adminUser = await _context.AdminUsers.FirstOrDefaultAsync(c => c.ID == id);
            if (adminUser == null)
            {
                return Json(new { success = false, message = "Tài khoản quản lý không tồn tài." });
            }
            try
            {
                _context.AdminUsers.Remove(adminUser);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = "Không thể xóa tài khoản quản lý." });
            }

        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AdminUser model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.AdminUsers.FirstOrDefault(u => u.NameUser == model.NameUser && u.PasswordUser.Trim() == model.PasswordUser.Trim());

                if (user != null)
                {
                    return RedirectToAction("Index", "AdminUsers_62133314");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                }
            }

            return View(model);
        }

    }
}
