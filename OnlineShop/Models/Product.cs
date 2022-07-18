using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityCore.Models
{
    public class Product
    {

        public int ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [DisplayName("Upload Image")]
        public string ImagePath { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Year { get; set; }

        public string Category { get; set; }

        public decimal Price { get; set; }

        public bool InStock { get; set; }

        public bool? Auctioned { get; set; }

        public string AuctionLeadingUserId { get; set; }

        public decimal? HighestBid { get; set; }

        public decimal? SecondHighestBid { get; set; }

       
        public DateTime? AuctionAvailableUntil { get; set; }

        public decimal Rating { get; set; }

        public int PersonsRated { get; set; }

        public string TechnicalSpecsDoc { get; set; }

        public List<UserRating> UserRatings { get; set; }


    }
}
