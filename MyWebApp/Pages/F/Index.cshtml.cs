using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace MyWebApp.Pages.F
{
    public class IndexModel : PageModel
    {
        private string dbString = Environment.GetEnvironmentVariable("AZURE_DATABASE_CONNECTION_STRING");
        public void OnGet()
        {           
            var display = new List<IDictionary<string, string>>();
            using (SqlConnection con = new SqlConnection(dbString))
            {
                SqlCommand cmd = new SqlCommand("select Starring, Title, Link, Description, Category from Content", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var record = new Dictionary<string, string>();
                    for(int i = 0; i < reader.FieldCount; i++)
                    {
                        record.Add(reader.GetName(i), reader.GetValue(i).ToString());
                    }
                    display.Add(record);
                }
                reader.Close();
            }
            ViewData["Display"] = display;
        }
    }
}
