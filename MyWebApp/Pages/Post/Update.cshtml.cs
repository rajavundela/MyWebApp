using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Security.Claims;

namespace MyWebApp.Pages.Post
{
    [Authorize]
    public class UpdateModel : PageModel
    {
        public string Title{get; set;}
        public string PostContent{get; set;}
        public string Category{get;set;}
        private string dbString = Environment.GetEnvironmentVariable("AZURE_DATABASE_CONNECTION_STRING");

        public void OnGet(Int64 id)
        {
            using(SqlConnection con = new SqlConnection(dbString)){
                con.Open();
                SqlCommand cmd = new SqlCommand("select Title, Content, Category from Posts where Id=@0", con);
                cmd.Parameters.AddWithValue("@0", id);
                SqlDataReader reader = cmd.ExecuteReader();
                if(reader.Read()){
                    Title = reader["Title"].ToString();
                    PostContent = reader["Content"].ToString();
                    Category = reader["Category"].ToString();
                }
                reader.Close();
            }
        }

        public IActionResult OnPost(long id, string title, string content, string category){
            //preparing slug
            string slug = title.ToLower(); 
            // invalid chars           
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", ""); 
            // convert multiple spaces into one space   
            slug = Regex.Replace(slug, @"\s+", " ").Trim(); 
            // cut and trim 
            slug = slug.Substring(0, slug.Length <= 80 ? slug.Length : 80).Trim();   
            slug = Regex.Replace(slug, @"\s", "-"); // hyphens 

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;//? stops if it is null
            using(SqlConnection con = new SqlConnection(dbString)){
                SqlCommand cmd = new SqlCommand("UpdatePost", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Slug", slug));
                cmd.Parameters.Add(new SqlParameter("@Title", title));
                cmd.Parameters.Add(new SqlParameter("@Category", category));
                cmd.Parameters.Add(new SqlParameter("@Content", content));
                cmd.Parameters.Add(new SqlParameter("@AuthorId", userId));
                cmd.Parameters.Add(new SqlParameter("@PostId", id));
                con.Open();
                // For UPDATE, INSERT, and DELETE statements, the return value is the number of rows affected by the command. For all other types of statements, the return value is -1
                if(cmd.ExecuteNonQuery() > 0){
                    TempData["Message"] = "Post updated.";
                    return Redirect($"/Post/{id}/{slug}");
                }
                else{
                    // It means either id is invalid or author is not post's author
                    return Unauthorized();
                }
            }
            
        }
    }
}
