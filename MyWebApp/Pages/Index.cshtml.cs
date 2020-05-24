using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace MyWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            var posts = new List<Dictionary<string, string>>();
            string dbString = Environment.GetEnvironmentVariable("AZURE_DATABASE_CONNECTION_STRING");
            using(SqlConnection con = new SqlConnection(dbString)){
                con.Open();
                SqlCommand cmd = new SqlCommand("GetRecentPosts", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();
                var markdown = new MarkdownSharp.Markdown();
                while(reader.Read()){
                    var post = new Dictionary<string, string>();
                    for(int i = 0; i < reader.FieldCount; i++){
                        post.Add(reader.GetName(i), reader.GetValue(i).ToString());
                    }
                    
                    post["Content"] = markdown.Transform(post["Content"]);
                    Match m = Regex.Match(post["Content"], @"<p>\s*(.+?)\s*</p>");
                    if (m.Success)
                    {
                        post["Content"] =  m.Groups[1].Value;
                    }
                    else
                    {
                        post["Content"] = "";
                    }
                    posts.Add(post);
                }
            }
            ViewData["Posts"] = posts;
        }
    }
}
