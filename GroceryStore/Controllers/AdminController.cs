using GroceryStore.Models;
using GroceryStore.Models.DataLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GroceryStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly GroceryContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdminController(GroceryContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // Admin dashboard showing all products
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true" && HttpContext.Session.GetString("IsAdmin") == "true")
            {
                var products = _context.Products.Include(p => p.Category).ToList();
                return View(products);
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Admin");
            }
        }

        // Display form to create a new product
        public IActionResult CreateProduct(int id = 0)
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true" && HttpContext.Session.GetString("IsAdmin") == "true")
            {
                ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName");

                if (id != 0)
                {
                    var product = _context.Products.Find(id);

                    ProductRequestModel productRequestModel = new ProductRequestModel
                    {
                        CategoryId = product.CategoryId,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Category = product.Category,
                        ProductId = product.ProductId


                    };

                    return View(productRequestModel);
                }
                return View();

            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductRequestModel o)
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true" && HttpContext.Session.GetString("IsAdmin") == "true")
            {
                var record = _context.Products.Find(o.ProductId);
                if (record == null)
                {
                    if (o.fileData == null)
                    {
                        ModelState.AddModelError("fileData", "This field is required");

                    }
                }
                if (ModelState.IsValid)
                {
                    string ProductName = "";
                    if (o.fileData != null)
                    {
                        ProductName = await SaveImageAsync(o.fileData);
                    }
                    if (record != null)
                    {
                        if (ProductName != "")
                        {
                            record.ImageUrl = ProductName;
                        }
                        record.Name = o.Name;
                        record.CategoryId = o.CategoryId;
                        record.Description = o.Description;
                        record.Price = o.Price;
                        _context.Products.Update(record);

                    }
                    else
                    {
                        var product = new Product();


                        product.ImageUrl = ProductName;
                        product.Name = o.Name;
                        product.CategoryId = o.CategoryId;
                        product.Description = o.Description;
                        product.Price = o.Price;
                        _context.Products.Add(product);
                    }

                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName");
                return View(o);
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Admin");
            }
        }


        // Delete a product
        public IActionResult DeleteProduct(int id)
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true" && HttpContext.Session.GetString("IsAdmin") == "true")
            {
                var product = _context.Products.Find(id);
                if (product == null)
                {
                    return NotFound();
                }

                _context.Products.Remove(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Admin");
            }
        }

        // Manage Orders
        public IActionResult Orders()
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true" && HttpContext.Session.GetString("IsAdmin") == "true")
            {
                var orders = _context.Orders.Include(o => o.OrderItems)
                                         .ThenInclude(oi => oi.Product)
                                         .ToList();
                return View(orders);
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Admin");
            }
        }
        public IActionResult LogOut()
        {
            HttpContext.Session.SetString("IsAuthenticated", "false");
            HttpContext.Session.SetString("IsAdmin", "false");
            HttpContext.Session.SetInt32("UserId", 0);
            return RedirectToAction("Login", "Admin");
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
                    HttpContext.Session.SetString("IsAdmin", "true");
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    return RedirectToAction("Index", "Admin");

                }

            }
            return View(loginModel);
        }
        public IActionResult User()
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true" && HttpContext.Session.GetString("IsAdmin") == "true")
            {
                var list = _context.Users.ToList();
                return View(list);
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Admin");
            }
        }
        public IActionResult AddUser()
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true" && HttpContext.Session.GetString("IsAdmin") == "true")
            {
                return View();
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public IActionResult AddUser(UserRequestModel o)
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true" && HttpContext.Session.GetString("IsAdmin") == "true")
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
                        IsAdmin = true,
                        Password = o.Password,
                        Username = o.Username
                    };
                    _context.Add(res);
                    _context.SaveChanges();
                    return RedirectToAction("User", "Admin");

                }
                return View(o);
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Admin");
            }
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var fileName = ""; // Generate a unique filename

            if (imageFile != null && imageFile.Length > 0)
            {
                fileName = $"{Guid.NewGuid()}.jpg"; // Generate a unique filename

                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

            }
            return fileName;

        }
    }
}
