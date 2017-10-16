using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingSpider.Models
{
    public class Products
    {
        public string SourceSite { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImgUrl { get; set; }
        public string ShopName { get; set; }
        public string SourceUrl { get; set; }
        public string commentsCount { get; set; }
    }
}