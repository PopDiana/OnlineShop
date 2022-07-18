using IdentityCore;
using IdentityCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.RecommenderSystem
{
    public class ComputeRecommenderMatrixJob : IJob
    {
        private static decimal[,] productRatings;
        public Task Execute(IJobExecutionContext context)
        {

            var options = new DbContextOptionsBuilder<ApplicationDbContext>();

            options.UseSqlServer(Startup.startupConfiguration.GetConnectionString("DefaultConnection"));
            var _context = new ApplicationDbContext(options.Options);

            var products = _context.Products.ToList();

            var productIndex = products.Max(p => p.ProductId);

            productRatings = new decimal[productIndex + 1, productIndex + 1];

            for(int i = 0; i <= productIndex; ++i)
            {
                for (int j = 0; j <= productIndex; ++j)
                {
                    var p1 = products.FirstOrDefault(p => p.ProductId == i);
                    var p2 = products.FirstOrDefault(p => p.ProductId == j);

                    if(p1 != null && p2 != null)
                    {
                        productRatings[i,j] = Math.Abs(p1.Rating - p2.Rating);
                    }
                }
            }

            Startup.productRatings = productRatings;

            return Task.FromResult(0);
        }
    }
}
