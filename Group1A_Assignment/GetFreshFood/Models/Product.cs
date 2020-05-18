using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetFreshFood.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string price { get; set; }
        public string quantity { get; set; }   
        public string short_description { get; set; }
        public string image_path { get; set; }
      
    }
}