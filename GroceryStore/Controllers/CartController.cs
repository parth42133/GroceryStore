using GroceryStore.Models.DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GroceryStore.Controllers
{
    public class CartController : Controller
    {
        private readonly GroceryContext _context;

        public CartController(GroceryContext context)
        {
            _context = context;
        }

        // Display the cart
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true")
            {
                var cart = GetCartItems();
            return View(cart);
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Home");
            }
        }

        // Add item to the cart
        public IActionResult AddToCart(int id)
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true")
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                var cart = GetCartItems();
                var userId = GetSessionUserId();

                var cartItem = cart.FirstOrDefault(c => c.ProductId == id && c.UserId == userId);
                if (cartItem != null)
                {
                    cartItem.Quantity++;
                }
                else
                {
                    cart.Add(new ShoppingCartItem
                    {
                        ProductId = id,
                        Quantity = 1,
                        Product = product,
                        UserId = userId
                    });
                }
                SaveCartItems(cart);
            }
            return RedirectToAction("Index");
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Home");
            }
        }

        // View past orders
        public IActionResult PastOrders()
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true")
            {
                var userId = GetSessionUserId();

            var orders = _context.Orders
                                 .Include(o => o.OrderItems)
                                 .ThenInclude(oi => oi.Product)
                                 .Where(o => o.UserId == userId)
                                 .ToList();

            return View(orders);
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Home");
            }
        }

        // View order details
        public IActionResult OrderDetails(int orderId)
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true")
            {
                var order = _context.Orders
                                .Include(o => o.OrderItems)
                                .ThenInclude(oi => oi.Product)
                                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Home");
            }
        }

        // Checkout
        public IActionResult Checkout()
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true")
            {
                if (ModelState.IsValid)
            {
                var userId = GetSessionUserId();

                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    TotalAmount = 0,
                    OrderItems = new List<OrderItem>()
                };

                var cartItems = GetCartItems();

                foreach (var cartItem in cartItems.Where(ci => ci.UserId == userId))
                {
                    var orderItem = new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Product.Price
                    };

                    order.TotalAmount += (orderItem.Price ?? 0) * (orderItem.Quantity ?? 0);
                    order.OrderItems.Add(orderItem);
                }

                _context.Orders.Add(order);
                _context.SaveChanges();

                // Clear the cart for the current user
                cartItems.RemoveAll(ci => ci.UserId == userId);
                SaveCartItems(cartItems);

                return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
            }

            return View(GetCartItems());
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Home");
            }
        }

        // Order confirmation
        public IActionResult OrderConfirmation(int orderId)
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true")
            {
                var order = _context.Orders
                                .Include(o => o.OrderItems)
                                .ThenInclude(oi => oi.Product)
                                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Home");
            }
        }

        // Get cart items
        private List<ShoppingCartItem> GetCartItems()
        {
            var userId = GetSessionUserId();
            return _context.ShoppingCartItems
                           .Include(ci => ci.Product) // Include product details if needed
                           .Where(ci => ci.UserId == userId)
                           .ToList();
        }


        // Update quantity
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true")
            {
                var cartItems = GetCartItems();
            var userId = GetSessionUserId();

            var existingCartItem = cartItems.FirstOrDefault(ci => ci.ProductId == productId && ci.UserId == userId);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity = quantity;
                SaveCartItems(cartItems);
            }

            return Ok();
            }
            else
            {
                // User is not authenticated, redirect to login page
                return RedirectToAction("Login", "Home");
            }
        }

        // Remove from cart
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var userId = GetSessionUserId(); // Retrieve the current user's ID

            // Find the existing cart item for the user in the database
            var existingCartItem = _context.ShoppingCartItems
                                            .FirstOrDefault(ci => ci.ProductId == productId && ci.UserId == userId);

            if (existingCartItem != null)
            {
                // Remove the item from the database
                _context.ShoppingCartItems.Remove(existingCartItem);
                _context.SaveChanges(); // Save changes to the database
            }

            return Ok(); // Return a success response
        }

        // Save cart items
        private void SaveCartItems(List<ShoppingCartItem> cart)
        {
            var userId = GetSessionUserId();

            // Remove existing cart items for this user
            var existingCartItems = _context.ShoppingCartItems.Where(ci => ci.UserId == userId).ToList();
            _context.ShoppingCartItems.RemoveRange(existingCartItems);

            // Add new cart items for this user
            foreach (var item in cart)
            {
                ShoppingCartItem cartItem = new ShoppingCartItem()
                {
                     CartItemId = item.CartItemId,
                      ProductId=item.ProductId,
                       UserId= userId,
                        Quantity = item.Quantity
                };
                _context.ShoppingCartItems.Add(cartItem);
            }

            _context.SaveChanges();
        }


        // Get the current user's ID from the session
        private int? GetSessionUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }
    }
}
