using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityCore.Data;
using IdentityCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityCore.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ShoppingCart shoppingCart;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(ApplicationDbContext applicationDbContext, ShoppingCart shoppingCart, IHttpContextAccessor httpContextAccessor)
        {
            this.applicationDbContext = applicationDbContext;
            this.shoppingCart = shoppingCart;
            this._httpContextAccessor = httpContextAccessor;
        }

        [Authorize]
        public IActionResult Checkout()
        {
            
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;


            ApplicationUser currentUser = new ApplicationUser();

            var userList = applicationDbContext.Users.ToList();

            foreach(var it in userList)
            {
                if(it.Id == userId)
                {
                    currentUser = it;
                }

            }

            Order order = new Order();
            order.AddressLine1 = currentUser.Street;
            order.City = currentUser.City;
            order.Country = currentUser.Country;
            order.Email = currentUser.Email;
            order.FirstName = currentUser.FirstName;
            order.LastName = currentUser.LastName;
            order.PhoneNumber = currentUser.PhoneNumber;
            order.ZipCode = currentUser.PostalCode;
            order.State = currentUser.Province;
           

            return View(order);
        }

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            var items = shoppingCart.GetShoppingCartItems();
            shoppingCart.ShoppingCartItems = items;

            if (shoppingCart.ShoppingCartItems.Count == 0)
            {
                ModelState.AddModelError("", "Your cart is empty, add some products!");
            }

            if (ModelState.IsValid)
            { 
                order.OrderPlaced = DateTime.Now;

                var shoppingCartItems = shoppingCart.ShoppingCartItems;
                order.OrderTotal = shoppingCart.GetShoppingCartTotal();

                order.OrderDetails = new List<OrderDetail>();

                foreach (var shoppingCartItem in shoppingCartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        Amount = shoppingCartItem.Amount,
                        ProductId = shoppingCartItem.Product.ProductId,
                        Price = shoppingCartItem.Product.Price
                    };

                    order.OrderDetails.Add(orderDetail);
                }

                applicationDbContext.Orders.Add(order);
                applicationDbContext.SaveChanges();


                Utils.Instance.productNumber = 0;
                shoppingCart.ClearCart();
                return RedirectToAction("CheckoutComplete");
            }
            return View(order);
        }

        public IActionResult CheckoutComplete()
        {
            ViewBag.CheckoutCompleteMessage = "Thank you for your order!";
            return View();
        }

    }
}
