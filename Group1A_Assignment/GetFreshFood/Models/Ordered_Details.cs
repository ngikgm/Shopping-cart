using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetFreshFood.Models
{
    public class Ordered_Details
    {
        public string CustomerId { get; set; }
        public string ProductId { get; set; }
        public int Ordered_Quantity { get; set; }
        public DateTime Ordered_Date { get; set; }

        public string Product_Image_Path { get; set; }

        public string Product_Name { get; set; }
        public int Product_Price { get;set; }
        public string Product_Details { get; set; }
    }
}