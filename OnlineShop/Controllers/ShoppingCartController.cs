using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityCore.Data;
using IdentityCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;

namespace IdentityCore.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly ShoppingCart _shoppingCart;

        public ShoppingCartController(ApplicationDbContext applicationDbContext, ShoppingCart shoppingCart)
        {
            _appDbContext = applicationDbContext;
            _shoppingCart = shoppingCart;
        }


        public ViewResult Index(string voucher)
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            var couponCodes = _appDbContext.CouponCodes.ToList();
            decimal percentage = 0;

            var shoppingCartViewModel = new ShoppingCartViewModel
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal(),
                Discount = 0
            };

            if(voucher != null)
            {
                foreach(var it in couponCodes)
                {
                    if (voucher == it.CouponName)
                    {
                        percentage = it.CouponPercentage;
                    }
                        
                }

                decimal discount = (shoppingCartViewModel.ShoppingCartTotal * percentage) / 100;

                shoppingCartViewModel.ShoppingCartTotal -= discount;
                shoppingCartViewModel.Discount = discount;

            }

            return View(shoppingCartViewModel);
        }

        
        public RedirectToActionResult AddToShoppingCart(int productId)
        {

            var product = _appDbContext.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                if(product.Auctioned == true && product.AuctionAvailableUntil < DateTime.Now && User.Identity.IsAuthenticated)
                {
                    var currentUser = _appDbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                    string userId = currentUser.Id;
                    if(product.AuctionLeadingUserId == userId)
                    {
                        product.Price = product.SecondHighestBid.GetValueOrDefault();
                        _appDbContext.Update(product);
                        _appDbContext.SaveChanges();
                        _shoppingCart.AddToCart(product, 1);
                        Utils.Instance.productNumber++;
                    }
                    product.Auctioned = false;
                    product.InStock = false;
                    _appDbContext.Update(product);
                    _appDbContext.SaveChanges();
                }
                else
                {
                    _shoppingCart.AddToCart(product, 1);
                    Utils.Instance.productNumber++;
                }
            }
          
            
            return RedirectToAction("Index");
        }

        
       
        public RedirectToActionResult RemoveFromShoppingCart(int productId)
        {
            
            var selectedProduct = _appDbContext.Products.FirstOrDefault(p => p.ProductId == productId);

            if (selectedProduct != null)
            {
                _shoppingCart.RemoveFromCart(selectedProduct);
                Utils.Instance.productNumber--;
            }
            return RedirectToAction("Index");
        }


    }
}
