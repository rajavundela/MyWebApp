using System;
using System.Data;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace MyWebApp.Pages.Post
{
    [Authorize]
    public class CreateModel : PageModel
    {
        public void OnGet()
        {
        }
        public void OnPost(string title, string category, string content){

            //preparing slug
            string slug = title.ToLower(); 
            // invalid chars           
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", ""); 
            // convert multiple spaces into one space   
            slug = Regex.Replace(slug, @"\s+", " ").Trim(); 
            // cut and trim 
            slug = slug.Substring(0, slug.Length <= 80 ? slug.Length : 80).Trim();   
            slug = Regex.Replace(slug, @"\s", "-"); // hyphens 

            var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;//? stops if it is null
            string dbString = Environment.GetEnvironmentVariable("AZURE_DATABASE_CONNECTION_STRING");
            using(SqlConnection con = new SqlConnection(dbString)){
                SqlCommand cmd = new SqlCommand("insertPost", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Slug", slug));
                cmd.Parameters.Add(new SqlParameter("@Title", title));
                cmd.Parameters.Add(new SqlParameter("@Category", category));
                cmd.Parameters.Add(new SqlParameter("@Content", content));
                cmd.Parameters.Add(new SqlParameter("@Email", email));
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
