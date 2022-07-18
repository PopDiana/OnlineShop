using IdentityCore.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        public ApplicationDbContext applicationDbContext { get; set; }

        public NavbarViewComponent(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public IViewComponentResult Invoke()
        {
            var products = applicationDbContext.Products.ToList();
            var categories = new List<string>();
            foreach(var p in products)
            {
                if(!categories.Any(a => a == p.Category))
                {
                    categories.Add(p.Category);
                }
            }
            categories = categories.OrderBy(c => c).ToList();

            return View(categories);
        }
    }
}
