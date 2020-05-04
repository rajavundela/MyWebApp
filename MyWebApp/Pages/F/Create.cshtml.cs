using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace MyWebApp.Pages.F
{
    public class CreateModel : PageModel
    {
        public void OnGet()
        {
        }

        public void OnPost(string title, string link, string category, string starring, string description){
            string dbString = Environment.GetEnvironmentVariable("AZURE_DATABASE_CONNECTION_STRING");
            using(SqlConnection con = new SqlConnection(dbString)){
                SqlCommand cmd = new SqlCommand("insertContent", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Starring", starring));
                cmd.Parameters.Add(new SqlParameter("@Title", title));
                cmd.Parameters.Add(new SqlParameter("@Link", link));
                cmd.Parameters.Add(new SqlParameter("@Category", category));
                cmd.Parameters.Add(new SqlParameter("@Description", description));
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
