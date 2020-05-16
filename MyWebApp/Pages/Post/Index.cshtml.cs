using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace MyWebApp.Pages.Post
{
    public class IndexModel : PageModel
    {
        public string Title{get; set;}
        public string PostContent{get; set;}
        public string Category{get;set;}
        public string CreatedAt{get;set;}
        public string UpdatedAt{get;set;}
        public string AuthorId{get;set;}
        public string AuthorFullName{get;set;}
        public string AuthorLink{get;set;}
        public string AuthorIntro{get;set;}

        public void OnGet(Int64 id, string slug)
        {
            string dbString = Environment.GetEnvironmentVariable("AZURE_DATABASE_CONNECTION_STRING");
            using(SqlConnection con = new SqlConnection(dbString)){
                SqlCommand cmd = new SqlCommand("DisplayPost", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Id", id));
                cmd.Parameters.Add(new SqlParameter("@slug", slug));

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if(reader.Read()){
                    PostContent = reader["Content"].ToString();
                    CreatedAt = reader["CreatedAt"].ToString();
                    UpdatedAt = reader["UpdatedAt"].ToString();
                    Category = reader["CreatedAt"].ToString();
                    Title = reader["Title"].ToString();
                    AuthorId = reader["Id"].ToString();
                    AuthorFullName = reader["FullName"].ToString();
                    AuthorLink = reader["Link"].ToString();
                    AuthorIntro = reader["Intro"].ToString();
                }
            }
            
            var markdown = new MarkdownSharp.Markdown();
            PostContent = markdown.Transform(PostContent);
            ViewData["Id"] = id;
            ViewData["Slug"] = slug;
        }
    }
}
