using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using GetFreshFood.Models;
using GetFreshFood.Data;


namespace GetFreshFood.Controllers
{
    [RoutePrefix("Home")]
    public class HomeController : Controller
    {

       

        int total_price = 0;

        // GET: Home
        public ActionResult Index()
        {
            //InsertProductData();

            return RedirectToAction("SearchProduct","Home");
        }

        public ActionResult SearchProduct(FormCollection data)
        {   
            // Clear for Shopping Cart Icon
            Session["ShoppingCart"] = null;

            List<Product> product_list = new List<Product>();

            string target_data = (string)data["search_box"];

            using (SqlConnection sql_connection = new SqlConnection(Data.Data.conn))
            {
                sql_connection.Open();

                string sql = "select * from Product where name like '%" + target_data + "%';";

                SqlCommand sql_cmd = new SqlCommand(sql, sql_connection);

                SqlDataReader reader = sql_cmd.ExecuteReader();

                while (reader.Read())
                {
                    Product pro = new Product()
                    {
                        Id = (int)reader["Id"],
                        name = (string)reader["name"],
                        price = (string)reader["price"],
                        quantity = (string)reader["quantity"],
           
                        short_description = (string)reader["short_description"],
                        image_path = (string)reader["image_path"]
                        

                    };

                    product_list.Add(pro);
                }

                ViewBag.product_lis = product_list;
                ViewBag.Search_data = target_data;

                if (Session["Correct_User"] != null)
                {
                    ViewBag.Cart_Data_History = Session["history"];
                }

                sql_connection.Close();
            }
            return View();
        }


        [Route("AddCart")]
        public JsonResult AddCart(Product product)
        {
            // Clear for Shopping Cart Icon
            Session["ShoppingCart"] = null;

            object customer_history;

            if (Session["Correct_User"] != null)
            {
                Product temp_product = new Product()
                {
                    Id = product.Id
                };

                List<string> history = (List<string>)Session["history"];

                if (history == null)
                {
                    history = new List<string>();
                }

                history.Add(product.Id.ToString());

                Session["history"] = history;

                customer_history = new
                {
                    clicked_product_id = history.Count(),
                };

            }
            else if (Session["Correct_User"] == null)
            {
                customer_history = new
                {
                    clicked_product_id = 0,
                };
            }
            else
            {
                return Json(false);
            }

            return Json(customer_history, JsonRequestBehavior.AllowGet);
        }

   
        [HttpPost]
        public JsonResult UpdateCart(Product pro)
        {
            // Clear for Shopping Cart Icon
            Session["ShoppingCart"] = null;

            object customer_history;

            if (Session["Correct_User"] != null)
            {
                List<string> history_cart = (List<string>)Session["history"];

                if (history_cart.Count == 0)
                {
                    customer_history = new
                    {
                        clicked_product_id = 0,
                    };
                }
                else
                {
                    customer_history = new
                    {
                        clicked_product_id = history_cart.Count(),
                    };
                }
            }
            else
            {
                customer_history = new
                {
                    clicked_product_id =00,
                };
            }
            return Json(customer_history, JsonRequestBehavior.AllowGet);
        }

       
        public ActionResult Show_Cart()
        {

            if (Session["Correct_User"]==null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (Add_ProductID_Qua() == null || Session["history"]==null)
            {
                Session["ShoppingCart"] = "empty_product";
                ViewBag.OrderedProductId_Quantity = null;
                ViewBag.TotalPrice = 0;
                return View();
            }
            else
            {
                Session["ShoppingCart"] = "current_pt_list";
                ViewBag.OrderedProductId_Quantity = Add_ProductID_Qua();
                ViewBag.TotalPrice = total_price;
            }
            return View();
            }


        public List<Ordered_Details> Add_ProductID_Qua()
        {
            total_price = 0;

            List<string> order_product_lis = (List<string>)Session["history"];
            
            if(order_product_lis.Count == 0)
             {
                     return null;
             }

             List<Ordered_Details> ordered_products = new List<Ordered_Details>();

            // Ordered Product ID and Quantity
            var query = from x in order_product_lis
                    group x by x into g
                    let count = g.Count()
                    orderby count descending
                    select new { Value = g.Key, Count = count };

           
                foreach (var q in query)
                {
                using (SqlConnection sql_connection = new SqlConnection(Data.Data.conn))
                {
                sql_connection.Open();

                string sql_data = "select * from Product where Id=" + q.Value + ";";

                SqlCommand sql_cmd = new SqlCommand(sql_data, sql_connection);

                SqlDataReader reader = sql_cmd.ExecuteReader();

                while (reader.Read())
                {
                    Product pro = new Product()
                    {
                        Id = (int)reader["Id"],
                        name = (string)reader["name"],
                        price = (string)reader["price"],
                        quantity = (string)reader["quantity"],
                       
                        short_description = (string)reader["short_description"],
                        image_path = (string)reader["image_path"],
                       
                    };

                   Ordered_Details temp_data = new Ordered_Details()
                   {
                            CustomerId = (string)Session["Correct_User"],
                            ProductId = q.Value.ToString(),
                            Ordered_Quantity = q.Count,
                            Ordered_Date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day),
                            Product_Details = pro.short_description,
                            Product_Image_Path = pro.image_path,
                            Product_Name = pro.name,
                            Product_Price = Convert.ToInt32(pro.price.Replace("$", String.Empty))
                   };
                    ordered_products.Add(temp_data);
                 }
                    sql_connection.Close();
                }
            }
            foreach (Ordered_Details temp_data in ordered_products)
            {
                total_price = total_price + (temp_data.Ordered_Quantity * temp_data.Product_Price);
            }
            return ordered_products;
        }

        [HttpPost]
        public JsonResult UpdateCartInfo(Ordered_Details order_details)
        {
            bool add_product = false;
            
            Ordered_Details temp_data = new Ordered_Details()
            {
                ProductId = order_details.ProductId,
                Ordered_Quantity = order_details.Ordered_Quantity
            };
            List<string> old_history_lis = (List<string>)Session["history"];
           
            // Ordered Product ID and Quantity
            var query = from x in old_history_lis
                        group x by x into g
                        let count = g.Count()
                        orderby count descending
                        select new { Value = g.Key, Count = count };

            foreach(var old_history_data in query)
            {
                if(old_history_data.Value == temp_data.ProductId)
                {
                    if(old_history_data.Count < temp_data.Ordered_Quantity)
                    {
                        add_product = true;
                    }
                }
            }

            int temp_update_productID = Int32.Parse(temp_data.ProductId);

            if (add_product)
            {
                old_history_lis.Add(temp_update_productID.ToString());
            }
            else
            {
                old_history_lis.Remove(temp_update_productID.ToString());
            }

            // Update session
            Session["history"] = null;
            Session["history"] = old_history_lis;

            // Calculate Total Cost
            List<Ordered_Details> temp_orderedDetails = Add_ProductID_Qua();
            int temp_totalPrice = total_price;
            object new_total_price = new
            {
                total_price = temp_totalPrice
            };
            return Json(new_total_price,JsonRequestBehavior.AllowGet);
        }

        // Read PurchaseHistory 
        public ActionResult PurchaseHistory()
        {
            string customer = (string)Session["Correct_User"];

            //  Read from Purchased Table
            if(Session["Correct_User"] !=null)
            {
            List<Purchased_Product> purchased_products_lis = new List<Purchased_Product>();
            List<Num_of_Id> id_quantity_lis = new List<Num_of_Id>();
            

            using (SqlConnection sql_connection = new SqlConnection(Data.Data.conn))
            {
                
                sql_connection.Open();

                string target_sql_data = "select * from PurchasedProduct where CustomerId='"+ customer + "';";

                SqlCommand sql_cmd = new SqlCommand(target_sql_data, sql_connection);

                SqlDataReader reader = sql_cmd.ExecuteReader();

                while (reader.Read())
                {

                    Purchased_Product one_purchase_product = new Purchased_Product()
                    {
                        Id = (int)reader["Id"],
                        ProductId = (string)reader["ProductId"],
                        ProductName = (string)reader["ProductName"],
                        ProductDetails = (string) reader["ProductDetails"],
                        ProductImagePath = (string) reader["ProductImagePath"],
                        ProductPurchasedDate = Convert.ToDateTime((string) reader["ProductPurchasedDate"]),
                        ProductActivationCode = (string) reader["ProductActivationCode"],
                        CustomerId = customer
                    };

                    purchased_products_lis.Add(one_purchase_product);
                }
                sql_connection.Close();
            }
 
            var groupedResult = purchased_products_lis.GroupBy(s => s.ProductId);

            bool flager = true;

            foreach (var ageGroup in groupedResult)
            {
                    List<string> activation_code_lis = new List<string>();

                    Num_of_Id temp_id_quantity = new Num_of_Id();
                    temp_id_quantity.ProductId = ageGroup.Key;
                    temp_id_quantity.ProductQuantity = ageGroup.Count().ToString();
                    
                    foreach (Purchased_Product s in ageGroup)  //Each group has a inner collection  
                    {
                        if(flager)
                        {
                            temp_id_quantity.ProductName = s.ProductName;
                            temp_id_quantity.ProductDetails = s.ProductDetails;
                            temp_id_quantity.ProductImagePath = s.ProductImagePath;
                            temp_id_quantity.ProductPurchasedDate = s.ProductPurchasedDate;
                            flager = false;
                        }
                        //Debug.WriteLine(s.ProductActivationCode);
                        //Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!**********");
                        activation_code_lis.Add(s.ProductActivationCode);

                    }
                    temp_id_quantity.ProductActivationCode = activation_code_lis;

                    Debug.WriteLine("*****");
                    flager = true;
                    id_quantity_lis.Add(temp_id_quantity);
                    activation_code_lis = null;
             }
                ViewBag.Purchased_Products_List = id_quantity_lis;
            }
            else
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        // Clicked CheckOut Link
        // Buy Product
        public ActionResult Buy_Product()
        {
            List<string> purchase_list = (List<string>)Session["history"];
            List<Purchased_Product> purchased_product_lis = new List<Purchased_Product>();

            if (purchase_list.Count != 0)
            { 

            foreach (string pro_id in purchase_list)
            {
                using (SqlConnection sql_connection = new SqlConnection(Data.Data.conn))
                {
                    sql_connection.Open();

                    string target_sql_data = "select * from Product where Id=" + pro_id + ";";

                    SqlCommand sql_cmd = new SqlCommand(target_sql_data, sql_connection);

                    SqlDataReader reader = sql_cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Product pro = new Product()
                        {
                            Id = (int)reader["Id"],
                            name = (string)reader["name"],
                            price = (string)reader["price"],
                            quantity = (string)reader["quantity"],
                           
                            short_description = (string)reader["short_description"],
                            image_path = (string)reader["image_path"],
                          
                        };

                        Purchased_Product temp_data = new Purchased_Product()
                        {
                            ProductImagePath = pro.image_path,
                            ProductId = pro.Id.ToString(),
                            ProductName = pro.name,
                            ProductDetails = pro.short_description,
                            ProductActivationCode = Guid.NewGuid().ToString(),
                            ProductPurchasedDate = DateTime.Now,
                        };

                        purchased_product_lis.Add(temp_data);
                    }
                    sql_connection.Close();
                }
            }
                Add_To_ProductTbl(purchased_product_lis);
                // WRITE TO TABLE

                return RedirectToAction("PurchaseHistory");
            }
            else
            {
                return Content("<h3>Empty List to Write</h3>");
            }
        }

        public void Add_To_ProductTbl(List<Purchased_Product> purchased_data)
        {
           string customer = (string)Session["Correct_User"];

           foreach (Purchased_Product pro_data in purchased_data)
           {
                using (SqlConnection sql_connection = new SqlConnection(Data.Data.conn))
                {
                    sql_connection.Open();

                    string target_query = "('" + pro_data.ProductId + "','" + pro_data.ProductName + "','" + pro_data.ProductDetails + "','" + pro_data.ProductPurchasedDate + "','" + pro_data.ProductActivationCode + "','" + pro_data.ProductImagePath + "','" + customer + "')";

                    string cmdText = "insert into PurchasedProduct(ProductId,ProductName,ProductDetails,ProductPurchasedDate,ProductActivationCode,ProductImagePath,CustomerId) values" + target_query;

                    SqlCommand sql_cmd = new SqlCommand(cmdText, sql_connection);

                    try
                    {
                        sql_cmd.ExecuteNonQuery();
                        sql_connection.Close();
                        Session["history"] = null;
                        Session["history"] = new List<string>();
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("Fail to write to Purchased table.");
                    }
                }
            }
        }


        }
    }
