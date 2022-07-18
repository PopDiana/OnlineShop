using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using System.Net;
using System.Net.Mail;
using IdentityCore.Data;
using Microsoft.EntityFrameworkCore;
using IdentityCore.Controllers;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using IdentityCore;

namespace OnlineShop.Auctions
{
    public class CheckClosedAuctionsJob : IJob
    {
        

        public Task Execute(IJobExecutionContext context)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>();
            
            options.UseSqlServer(Startup.startupConfiguration.GetConnectionString("DefaultConnection"));
            var _context = new ApplicationDbContext(options.Options);
            var products = _context.Products.ToList();

            foreach (var product in products)
            {

                if (product.Auctioned == true && product.AuctionAvailableUntil.GetValueOrDefault().Date == DateTime.Now.Date)
                {
                    var email = _context.Users.FirstOrDefault(u => u.Id == product.AuctionLeadingUserId).Email;

                    using (var message = new MailMessage("email from", email))
                    {
                        message.Subject = "You have won the auction for " + product.Name + "!";
                        var link = $"https://localhost:44398/Products/Details/" + product.ProductId;
                        message.IsBodyHtml = true;
                        message.Body = "You can buy it <a href='" + link + "'>here</a>.";
                        using (SmtpClient client = new SmtpClient
                        {
                            EnableSsl = true,
                            
                            Host = "smtp.gmail.com",
                            Port = 587,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential("email from", "password")
                        })
                        {
                            client.Send(message);
                        }
                    }
                }
            }



            return Task.FromResult(0);
        }
    }
}
