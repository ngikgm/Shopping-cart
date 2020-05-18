using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using GetFreshFood.Models;
using GetFreshFood.Data;


namespace GetFreshFood.Data
{
    public class UserData : Data
    {

       
        public static void AddNewUser(string UN, string PW, string CPW, string EM)
        {
             using (SqlConnection conns = new SqlConnection(conn))
              {

                int index = GetLastId()+1;
                conns.Open();

                

                  string cmdadduser = @"Insert into dbo.users (id,name,password,Confirmpassword,Emailaddress) values ("+index+",'" + UN + "','" + PW + "','" + CPW + "','" + EM + "')";
                  SqlCommand cmdAddU = new SqlCommand(cmdadduser, conns);
                  cmdAddU.ExecuteNonQuery();

              }

           

        }


        public static int GetLastId()
        {
            int x=0;

            using (SqlConnection conns = new SqlConnection(conn))
            {
                conns.Open();


                string cmdpw = @"select id  from dbo.users where id=(select max(id) from dbo.users)";
                SqlCommand cmdPW = new SqlCommand(cmdpw, conns);
                SqlDataReader readerP = cmdPW.ExecuteReader();
                if (readerP.Read())
                {
                    x = (int)readerP["id"];
                }
                return x;
            }
            
        }
        public static string GetPassword(string username)
        {
            string pw = "";

            using (SqlConnection conns = new SqlConnection(conn))
            {
                conns.Open();


                string cmdpw = @"select password from dbo.users where name='" + username + "'";
                SqlCommand cmdPW = new SqlCommand(cmdpw, conns);
                SqlDataReader readerP = cmdPW.ExecuteReader();
                if (readerP.Read())
                {
                    pw = (string)readerP["password"];
                }

            }
            return pw;
        }

        public static int GetUserId(string username)
        {
            int userid = 0;

            using (SqlConnection conns = new SqlConnection(conn))
            {
                conns.Open();


                string cmdId = @"select id from dbo.users where name='" + username + "'";
                SqlCommand cmdI = new SqlCommand(cmdId, conns);
                SqlDataReader readerI = cmdI.ExecuteReader();
                if (readerI.Read())
                {
                    userid = (int)readerI["id"];
                }

            }
            return userid;
        }

        public static List<string> GetAllEmail()
        {
            List<string> EMList = new List<string>();

            using (SqlConnection conns = new SqlConnection(conn))
            {
                conns.Open();


                string cmdem = @"select Emailaddress from dbo.users";
                SqlCommand cmdEM = new SqlCommand(cmdem, conns);
                SqlDataReader readerE = cmdEM.ExecuteReader();
                while (readerE.Read())
                {
                    EMList.Add((string)readerE["Emailaddress"]);
                }

            }
            return EMList;
        }

        public static List<string> GetAllUserN()
        {
            List<string> UNList = new List<string>();

            using (SqlConnection conns = new SqlConnection(conn))
            {
                conns.Open();


                string cmdun = @"select name from dbo.users";
                SqlCommand cmdUN = new SqlCommand(cmdun, conns);
                SqlDataReader readerU = cmdUN.ExecuteReader();
                while (readerU.Read())
                {
                    UNList.Add((string)readerU["name"]);
                }

            }
            return UNList;
        }


     
    }
}