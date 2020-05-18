using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using GetFreshFood.Models;
using GetFreshFood.Controllers;
using GetFreshFood.Data;


namespace MVCCA.Controllers
{
    public class RegisterController : Controller
    {

        public ActionResult Register()
        {


            return View();
        }




 

        public ActionResult ValiEmail(string emailaddress)
        {
            List<string> EMList = UserData.GetAllEmail();
            bool exist = false;

            for (int i = 0; i < EMList.Count; i++)
            {

                if (EMList[i] == emailaddress) exist = true;
            }

            if (exist) return Json(false);
            return Json(true);
        }





        public bool CheckUserName(string username)
        {
            List<string> UserNList = UserData.GetAllUserN();
            bool exist = false;

            for (int i = 0; i < UserNList.Count; i++)
            {

                if (UserNList[i] == username) exist = true;
            }


            return exist;
        }

        

        public ActionResult RegNewUser(FormCollection form)
        {
            string username = form["name"];
            string password = form["password"];
            string ConPw = form["Confirmpassword"];
            string EM = form["Emailaddress"];




            if (CheckUserName(username))
            {

                return RedirectToAction("Regfail", "Register");
            }
            else
            {
                int hashedpassword = passwordhashing(password);
                string pw = hashedpassword.ToString();
                UserData.AddNewUser(username, pw, pw, EM);

                return RedirectToAction("RegSuc", "Register");
            }


        }


        public int passwordhashing(string password_value)
        {
            int hashdata = password_value.GetHashCode();
            return hashdata;
        }
        public ActionResult RegSuc()
        {
            return View();
        }

        public ActionResult RegFail()
        {
            return View();
        }

        public ActionResult TnC()
        {
            return File("~/Content/images/TnC.pdf", "application/pdf");
        }
    }
}