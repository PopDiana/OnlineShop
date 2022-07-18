using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdentityCore.Models;
using IdentityCore.Data;
using OnlineShop.Models;
using System.Net.Mail;
using System.Net;


namespace IdentityCore.Controllers
{
    public class HomeController : Controller
    {
        public ApplicationDbContext applicationDbContext { get; set; }
        public HomeController(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public IActionResult Index(string byCategory, bool nameAsc, bool nameDesc,
            bool priceAsc, bool priceDesc, bool stock, string searchString)
        {
            List<Product> products = applicationDbContext.Products.ToList();        

            if (searchString != null)
            {
                products = products.Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                ViewBag.searchString = searchString;
            }

            if (byCategory != null)
            {
                products = products.Where(p => p.Category == byCategory).ToList();
            }
            if (nameAsc == true)
            {
                products = products.OrderBy(p => p.Name).ToList();
            }
            else if (nameDesc == true)
            {
                products = products.OrderByDescending(p => p.Name).ToList();
            }

            if (priceAsc == true)
            {
                products = products.OrderBy(p => p.Price).ToList();
            }
            else if (priceDesc == true)
            {
                products = products.OrderByDescending(p => p.Price).ToList();
            }

            if (stock == true)
            {
                products = products.Where(p => p.InStock == true).ToList();
            }
            List<Product> recommended = applicationDbContext.Products.ToList();
            var recommendedProducts = new List<Product>();

            if (recommended.Count != 0 && User.Identity.IsAuthenticated)
            {
                
                var userId = applicationDbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id;

                for (int i = 0, j = 0; i < recommended.Count && j < 4; ++i)
                {
                    
                        var rating = applicationDbContext.UserRatings.FirstOrDefault(r =>
                        r.ProductId == recommended[i].ProductId && r.UserId == userId);
                        if (rating == null)
                        {
                            recommendedProducts.Add(recommended[i]);
                            j++;
                        }

                    
                }
                ViewBag.recommendedProducts = recommendedProducts;
            }


            return View(products);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Search(string searchString)
        {
            return RedirectToAction("Index", new { searchString = searchString });
        }

        public IActionResult Feedback()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Feedback([Bind("Email, Description")] Feedback feedback)
        {
            if(ModelState.IsValid)
            {
                using (var message = new MailMessage(feedback.Email, "email to"))
                {
                    message.Subject = "Feedback from -> " + feedback.Email;
                    message.Body = feedback.Description;
                    using (SmtpClient client = new SmtpClient
                    {
                        EnableSsl = true,

                        Host = "smtp.gmail.com",
                        Port = 587,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("email", "password")
                    })
                    {
                        client.Send(message);
                    }
                }
            }
            ModelState.AddModelError("", "Your message has been sent. Thank you for your feedback!");
            return View();
        }



        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
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
