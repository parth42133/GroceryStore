using GroceryStore.Models;
using GroceryStore.Models.DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GroceryStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly GroceryContext _context;

        public HomeController(GroceryContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? categoryId)
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true")
            {
                var categories = _context.Categories.ToList();
                ViewBag.Categories = categories;

                var products = _context.Products.AsQueryable();

                if (categoryId.HasValue)
                {
                    products = products.Where(p => p.CategoryId == categoryId);
                }

                return View(products.ToList());
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Home");
            }
        }


        public IActionResult ProductDetails(int id)
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true")
            {
                var product = _context.Products.Include(p => p.Category)
                            .FirstOrDefault(p => p.ProductId == id);
            if (product == null) return NotFound();
            return View(product);
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult UserProfile()
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true")
            {
                var UserId = HttpContext.Session.GetInt32("UserId");

            var user = _context.Users.Where(x => x.UserId == UserId).Include(t => t.Orders).ThenInclude(t => t.OrderItems).FirstOrDefault();
            return View(user);
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult LogOut()
        {
            HttpContext.Session.SetString("IsAuthenticated", "false");
            HttpContext.Session.SetString("IsAdmin", "false");
            HttpContext.Session.SetInt32("UserId", 0);
            return RedirectToAction("Login", "Home");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterRequestModel o)
        {
            if (o != null && o.Email != null)
            {
                var isExist = _context.Users.Where(x => x.Email == o.Email).FirstOrDefault();
                if (isExist != null)
                {
                    ModelState.AddModelError("Email", "User with this email already exist");
                }
            }
            if (ModelState.IsValid)
            {
                User res = new User()
                {
                    Email = o.Email,
                    IsAdmin = false,
                    Password = o.Password,
                    Username = o.Username
                };
                _context.Add(res);
                _context.SaveChanges();
                return RedirectToAction("Login", "Home");

            }
            return View(o);
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Where(x => x.Email.ToLower() == loginModel.Email.ToLower() && x.Password == loginModel.Password).FirstOrDefault();
                if (user != null)
                {
                    HttpContext.Session.SetString("IsAuthenticated", "true");
                    HttpContext.Session.SetString("IsAdmin", "false");
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    return RedirectToAction("Index", "Home");

                }

            }
            return View(loginModel);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
