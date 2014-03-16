using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VKSearch.Models;

namespace VKSearch.Controllers
{
    public class HomeController : Controller
    {

        string connectionString = @"Data Source=DANIEL\SQLEXPRESS;Initial Catalog=VkSearch;Integrated Security=SSPI;";

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewResults()
        {
            var result = new List<VkUser>();

            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();

                using (SqlCommand sqlcom = new SqlCommand())
                {
                    sqlcom.Connection = sqlcon;
                    sqlcom.CommandText = @"SELECT * FROM searchResult (nolock) where checked = 0";

                    var reader = sqlcom.ExecuteReader();
                    if(reader!=null)
                    {
                        while (reader.Read())
                        {
                            result.Add(new VkUser()
                            {
                                first_name = (string)reader["firstName"],
                                last_name = (string)reader["lastName"],
                                id = Convert.ToInt64((string)reader["userId"]),
                                photo_200_orig = (string)reader["photo"],
                                status = (string)reader["info"]
                            });
                        }
                    }
                }
            }
            return View("ViewResults", result);
        }

        public JsonResult CheckSearchResult(long userId)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();

                using (SqlCommand sqlcom = new SqlCommand())
                {
                    sqlcom.Connection = sqlcon;
                    sqlcom.CommandText = @"UPDATE searchResult
                                            SET checked = 1
                                            WHERE userId = @userId";

                    sqlcom.Parameters.AddWithValue("@userId", userId);
                    sqlcom.ExecuteNonQuery();
                }
            }

            return Json(true);
        }

        [HttpPost]
        public JsonResult AddSearchResult(VkUser user)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();

                bool isExists = false;
                using (SqlCommand sqlcom = new SqlCommand())
                {
                    sqlcom.Connection = sqlcon;
                    sqlcom.CommandText = @"SELECT 1 FROM searchResult
                                            WHERE userId = @userId";

                    sqlcom.Parameters.AddWithValue("@userId", user.id);

                    object result = sqlcom.ExecuteScalar();
                    if (result != null)
                        isExists = true;
                }

                if (!isExists)
                {
                    using (SqlCommand sqlcom = new SqlCommand())
                    {
                        sqlcom.Connection = sqlcon;
                        sqlcom.CommandText = @"INSERT INTO searchResult
                                            VALUES (@userId, @firstName, @lastName, @photo, @info, @checked)";

                        sqlcom.Parameters.AddWithValue("@userId", user.id);
                        sqlcom.Parameters.AddWithValue("@firstName", user.first_name);
                        sqlcom.Parameters.AddWithValue("@lastName", user.last_name);
                        sqlcom.Parameters.AddWithValue("@photo", user.photo_200_orig);
                        sqlcom.Parameters.AddWithValue("@info", "Birthday:" + user.bdate + ". Status: " + user.status);
                        sqlcom.Parameters.AddWithValue("@checked", false);

                        sqlcom.ExecuteNonQuery();
                    }
                }
            }

            return Json(true);
        }
    }
}
