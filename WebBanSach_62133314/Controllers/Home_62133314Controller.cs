using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebBanSach_62133314.Models;

namespace WebBanSach_62133314.Controllers
{
    public class Home_62133314Controller : Controller
    {
        private WebBanSach_62133314Entities _context = new WebBanSach_62133314Entities();

      


        private int CalculateTotalQuantity()
        {
            if (Session["ShoppingCart"] != null)
            {
                var cart = Session["ShoppingCart"] as List<OrderDetail>;
                return cart.Count;
            }
            return 0;
        }
        public ActionResult Index()
        {
            ViewBag.CartTotalQuantity = CalculateTotalQuantity();
            var loggedInUser = Session["LoggedInUser"] as Customer;
            ViewBag.user = loggedInUser;
            List<Product> products = _context.Products.ToList();
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View(products);
        }
        [HttpPost]
        public ActionResult Search(string query)
        {
            ViewBag.CartTotalQuantity = CalculateTotalQuantity();
            var loggedInUser = Session["LoggedInUser"] as Customer;
            ViewBag.user = loggedInUser;
            if (!string.IsNullOrEmpty(query))
            {
                var searchResults = _context.Products
                    .Where(p => p.NamePro.Contains(query))
                    .ToList();

                var categories = _context.Categories.ToList();
                ViewBag.Categories = categories;
                ViewBag.Query = query;
                return View(searchResults);
            }

            return RedirectToAction("Index");
        }


        public ActionResult LienHe()
        {
            ViewBag.CartTotalQuantity = CalculateTotalQuantity();
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View();
        }
        public ActionResult TheLoai(string categoryName)
        {
            ViewBag.CategoryName = categoryName;
            ViewBag.CartTotalQuantity = CalculateTotalQuantity();
            var loggedInUser = Session["LoggedInUser"] as Customer;
            ViewBag.user = loggedInUser;
            var products = _context.Products
            .Include(p => p.Category)
            .Where(p => p.Category.NameCate == categoryName)
            .ToList();

            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View(products);
        }

        public ActionResult Login()
        {
            ViewBag.CartTotalQuantity = CalculateTotalQuantity();
            var loggedInUser = Session["LoggedInUser"] as Customer;
            ViewBag.user = loggedInUser;
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Customer model)
        {
            ViewBag.CartTotalQuantity = CalculateTotalQuantity();
            var loggedInUser = Session["LoggedInUser"] as Customer;
            ViewBag.user = loggedInUser;
            if (ModelState.IsValid)
            {
                string hashedPassword = HashPassword(model.PassCus);
                var user = _context.Customers.FirstOrDefault(c => c.EmailCus == model.EmailCus && c.PassCus == hashedPassword);

                if (user != null)
                {
                    Session["LoggedInUser"] = user;

                    return RedirectToAction("Index", "Home_62133314");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password");
                }
            }

            return View("Login", model);
        }

        public ActionResult Register()
        {
            ViewBag.CartTotalQuantity = CalculateTotalQuantity();
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            var model = new Customer();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Customer customer)
        {
            ViewBag.CartTotalQuantity = CalculateTotalQuantity();
            var loggedInUser = Session["LoggedInUser"] as Customer;
            ViewBag.user = loggedInUser;
            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.Customers.Any(c => c.EmailCus == customer.EmailCus))
                    {
                        ModelState.AddModelError("EmailCus", "Địa chỉ email đã tồn tại.");
                        var categories = _context.Categories.ToList();
                        ViewBag.Categories = categories;
                        return View("Register", customer);
                    }
                    customer.PassCus = HashPassword(customer.PassCus);

                    _context.Customers.Add(customer);
                    _context.SaveChanges();

                    return RedirectToAction("Login", "Home_62133314");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi đăng ký.");
                    var categories = _context.Categories.ToList();
                    ViewBag.Categories = categories;
                    return View("Register", customer);
                }
            }


            var categoriesList = _context.Categories.ToList();
            ViewBag.Categories = categoriesList;
            return View("Register", customer);
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        public ActionResult AdminQL()
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View();
        }

        public ActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _context.Products.Find(productId);
            var cart = Session["ShoppingCart"] as List<OrderDetail>;

            if (cart == null)
            {
                cart = new List<OrderDetail>();
                Session["ShoppingCart"] = cart;
            }

            var existingItem = cart.FirstOrDefault(item => item.Product.ProductID == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newItem = new OrderDetail
                {
                    Product = product,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };

                cart.Add(newItem);
            }

            return RedirectToAction("ViewCart");
        }

        public ActionResult ViewCart()
        {
            ViewBag.CartTotalQuantity = CalculateTotalQuantity();
            var loggedInUser = Session["LoggedInUser"] as Customer;
            ViewBag.user = loggedInUser;
            var cart = Session["ShoppingCart"] as List<OrderDetail>;
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            if (cart == null || !cart.Any())
            {
                ViewBag.Message = "Giỏ hàng trống";
                return View();
            }

            return View(cart);
        }
        public ActionResult RemoveFromCart(int productId)
        {
            var cart = Session["ShoppingCart"] as List<OrderDetail>;

            var itemToRemove = cart.FirstOrDefault(item => item.Product.ProductID == productId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
            }

            return RedirectToAction("ViewCart");
        }
        public ActionResult ThanhToan()
        {
            ViewBag.CartTotalQuantity = CalculateTotalQuantity();
            var loggedInUser = Session["LoggedInUser"] as Customer;
            ViewBag.user = loggedInUser;
            if (loggedInUser != null)
            {
                var cart = Session["ShoppingCart"] as List<OrderDetail>;

                if (cart != null && cart.Any())
                {
                    var newOrder = new OrderPro
                    {
                        DateOrder = DateTime.Now,
                        IDCus = loggedInUser.IDCus,
                        AddressDeliverry = loggedInUser.AddressCus

                    };
                    _context.OrderProes.Add(newOrder);
                    _context.SaveChanges();

                    // Add order details
                    foreach (var item in cart)
                    {
                        var orderDetail = new OrderDetail
                        {
                            IDProduct = item.Product.ProductID,
                            IDOrder = newOrder.ID,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice
                        };

                        _context.OrderDetails.Add(orderDetail);
                    }

                    _context.SaveChanges();
                    Session["ShoppingCart"] = new List<OrderDetail>();
                    return RedirectToAction("ThanhCong");
                }
                else
                {
                    ViewBag.ErrorMessage = "Giỏ hàng trống.";
                }
            }
            else
            {
                return RedirectToAction("Login", new { returnUrl = Url.Action("ThanhToan", "Home_62133314") });
            }
            return RedirectToAction("ViewCart");
        }


        public ActionResult ThanhCong()
        {
            ViewBag.CartTotalQuantity = CalculateTotalQuantity();
            var loggedInUser = Session["LoggedInUser"] as Customer;
            ViewBag.user = loggedInUser;
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View();
        }
        public ActionResult ChiTietSanPham(int productId)
        {
            ViewBag.CartTotalQuantity = CalculateTotalQuantity();
            var loggedInUser = Session["LoggedInUser"] as Customer;
            ViewBag.user = loggedInUser;
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            var product = _context.Products.Find(productId);
            if (product != null)
            {
                return View(product);
            }
            return RedirectToAction("Index");
        }
        public ActionResult Logout()
        {
            Session["LoggedInUser"] = null;

            return RedirectToAction("Index", "Home_62133314");
        }

        public ActionResult AdjustQuantity(int productId, int quantity)
        {
            var cart = Session["ShoppingCart"] as List<OrderDetail>;

            var cartItem = cart.FirstOrDefault(item => item.Product.ProductID == productId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
            }

            var response = new { quantity = quantity };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

    }
}
