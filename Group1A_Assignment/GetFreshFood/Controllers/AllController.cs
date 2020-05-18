using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.Mvc;

namespace GetFreshFood.Controllers
{
    public class AllController : Controller
    {
        
 
        // SIGN IN
        public ActionResult SignIn()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SignIn(FormCollection sign_in)
        {

            Debug.WriteLine(password_hashing("aaaaaa"));
            string name_val = sign_in["name"];
            string password_val = password_hashing(sign_in["password1"]).ToString();
            string real_name = "";
            string real_password = "";
            
            using (SqlConnection sql_connection = new SqlConnection(Data.Data.conn))
            {
                sql_connection.Open();
                string db_data = "select * from Users where name='"+name_val+"';";

                SqlCommand sql_cmd = new SqlCommand(db_data, sql_connection);

                SqlDataReader reader = sql_cmd.ExecuteReader();

                while (reader.Read())
                {
                    real_password = (string)reader["password"];
                    real_name = (string)reader["name"];
                   
                }
                sql_connection.Close();
            }
            Debug.WriteLine(name_val);
            Debug.WriteLine("######");
            Debug.WriteLine(real_name);
            Debug.WriteLine(real_password);
            Debug.WriteLine("*****");
            Debug.WriteLine(password_val);
            if(name_val == real_name)
            {
                if(real_password == password_val)
                {
                    Session["Correct_User"] = name_val;
                    Session["history"] = new List<string>();
                    Session["Customer_email"] = name_val;
                    return RedirectToAction("SearchProduct", "Home");
                }
                else
                {
                    ViewBag.singin_err = string.Format("Wrong Password!");
                    return View();
                }
            }
            else
            {
                ViewBag.singin_err = string.Format("Account not found!");
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session["Correct_User"] = null;
            Session["history"] = null;

            return RedirectToAction("SearchProduct", "Home");
        }

        // Generate HashCode for Password
        public int password_hashing(string password_value)
        {
            int hashdata = password_value.GetHashCode();
            return hashdata;
        }
        
    }
}