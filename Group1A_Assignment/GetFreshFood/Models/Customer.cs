using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace GetFreshFood.Models
{
    public class Customer
    {
        public int id { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Username can only consist of alphabets")]
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string name { get; set; }


        [StringLength(32, MinimumLength =2, ErrorMessage = "Password length incorrect.")]
        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        public string password { get; set; }

        [System.ComponentModel.DataAnnotations.Compare(
            "password", ErrorMessage = "Passwords must match")]
        [Display(Name = "Confirm Password")]
        public string Confirmpassword { get; set; }


        [Remote("ValiEmail", "Register", HttpMethod = "POST",
            ErrorMessage = "Email address already registered")]
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Emailaddress { get; set; }


    }
}