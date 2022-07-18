using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityCore.Data;
using IdentityCore.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Http;
using System.Web;
using System.Net.Mail;
using System.Net;
using OnlineShop.Models;

namespace IdentityCore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        public IActionResult Search(string searchString, string sort = null)
        {
            List<Product> prods = _context.Products.ToList();
            List<Product> products = new List<Product>();

            string[] words = searchString.Split(' ');
           
            ViewBag.searchString = searchString;

            var searchBy = words[0].ToLower();

            if(searchBy == "name")
            {
                products = prods.Where(p => p.Name.Contains(words[1])).ToList();
            }

            if(searchBy == "price")
            {
                products = prods.Where(p => p.Price.ToString() == words[1]).ToList();
            }

            if(searchBy == "category")
            {
                products = prods.Where(p => p.Category.ToLower() == words[1].ToLower()).ToList();
            }

            if(searchBy == "year")
            {
                products = prods.Where(p => p.Year.ToString() == words[1]).ToList();
            }

            if (sort == "asc")
            {
                products = products.OrderBy(x => x.Year).ToList();
            }
            else if (sort == "desc")
            {
                products = products.OrderByDescending(x => x.Year).ToList();
            }

            return View(products);
        }
        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Description,ImagePath,Year,Category,TechnicalSpecsDoc,Price,InStock,Auctioned,AuctionAvailableUntil")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.Auctioned == true)
                {
                    product.HighestBid = product.Price;
                    product.SecondHighestBid = product.Price;
                    var admin = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                    product.AuctionLeadingUserId = admin.Id;
                    if (product.AuctionAvailableUntil == null)
                    {
                        var now = DateTime.Now;
                        product.AuctionAvailableUntil = now.AddDays(5);
                    }
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = _context.Users.FirstOrDefault(u => u.Id == product.AuctionLeadingUserId);

                    if (product.Auctioned == true && product.AuctionAvailableUntil < DateTime.Now && user.UserName == User.Identity.Name)
                    {
                        ViewBag.hasWon = true;
                    }
                    else
                    {
                        ViewBag.hasWon = false;
                    }

                    if (product.AuctionAvailableUntil < DateTime.Now)
                    {
                        ViewBag.auctionClosed = true;
                    }
                    else
                    {
                        ViewBag.auctionClosed = false;
                    }

                    var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id;
                    var userRating = await _context.UserRatings.FirstOrDefaultAsync(r => r.ProductId == product.ProductId && r.UserId == userId);
                    if (userRating != null)
                    {
                        ViewBag.alreadyRated = true;

                        ViewBag.rating = userRating.Rating.GetValueOrDefault();
                    }
                    else
                    {
                        ViewBag.alreadyRated = false;

                    }
                } else
                {
                    ViewBag.hasWon = false;
                    ViewBag.auctionClosed = true;
                    ViewBag.alreadyRated = false;
                }
                List<Product> recommended = GetRecommendedProducts(product.ProductId).ToList();
                var recommendedProducts = new List<Product>();
                if (recommended.Count != 0)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        recommendedProducts.Add(recommended[i]);
                    }
                    ViewBag.recommendedProducts = recommendedProducts;
                }
                var errMsg = TempData["ErrorMessage"] as string;
                ViewBag.errMsg = errMsg;
            }

            return View(product);
        }
        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Description,ImagePath,Year,Category,TechnicalSpecsDoc,Price,InStock")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public RedirectToActionResult Bid()
        {
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost, ActionName("Bid")]
        [ValidateAntiForgeryToken]
        public RedirectToActionResult Bid(int productId, decimal price)
        {

            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product.Auctioned == true)
            {
                if (price < product.HighestBid)
                {
                    TempData["ErrorMessage"] = "Bid must be higher that starting price";
                    //FlashMessage.Warning("Bid must be higher that starting price");

                    return RedirectToAction($"Details/" + productId);
                }
                if (price > product.HighestBid && product.AuctionAvailableUntil > DateTime.Now)
                {
                    product.SecondHighestBid = product.HighestBid;
                    product.HighestBid = price;
                    var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                    product.AuctionLeadingUserId = user.Id;

                    _context.Update(product);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction($"Details/" + productId);
        }



        [Authorize]
        public bool Rate(int productId, decimal rating)
        {

            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);


            if (product != null)
            {

                var userRating = new UserRating();

                var currentUser = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

                if (currentUser != null)
                {
                    var remove = RemoveRating(product.ProductId);

                    var totalRating = product.Rating;
                    var usersNumber = product.PersonsRated;
                    totalRating = (usersNumber * totalRating + rating) / (usersNumber + 1);

                    product.PersonsRated++;
                    product.Rating = totalRating;
                    _context.Update(product);
                    _context.SaveChanges();

                    userRating.Rating = rating;
                    userRating.ProductId = product.ProductId;
                    userRating.UserId = currentUser.Id;
                    _context.Add(userRating);
                    _context.SaveChanges();

                    return true;
                }

            }
            return false;
        }
        [Authorize]
        public bool RemoveRating(int productId)
        {

            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);

            if (product != null)
            {
                var totalRating = product.Rating;
                var usersNumber = product.PersonsRated;
                var currentUser = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

                if (currentUser != null)
                {
                    var userRating = _context.UserRatings.FirstOrDefault(r => r.ProductId == product.ProductId
                    && r.UserId == currentUser.Id);
                    if (userRating != null)
                    {
                        var rating = userRating.Rating;

                        _context.Remove(userRating);
                        _context.SaveChanges();

                        totalRating = (totalRating * usersNumber - rating.GetValueOrDefault()) / (usersNumber == 1 ? usersNumber : usersNumber - 1);
                        usersNumber--;

                        product.Rating = totalRating;
                        product.PersonsRated = usersNumber;

                        _context.Update(product);
                        _context.SaveChanges();

                        return true;

                    }

                }

            }
            return false;
        }

        private IEnumerable<Product> GetRecommendedProducts(int productId)
        {
            var matrix = Startup.productRatings;

            var products = _context.Products.ToList();
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            products.Remove(product);

            products.Sort((Product a, Product b) =>
            {
                if (matrix[a.ProductId, productId] < matrix[b.ProductId, productId])
                {
                    return 1;
                }
                else if (matrix[a.ProductId, productId] > matrix[b.ProductId, productId])
                {
                    return -1;
                }
                else
                {
                    return 0;
                }

            });

            return products;

        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
