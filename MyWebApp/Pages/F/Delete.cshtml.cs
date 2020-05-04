using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace MyWebApp.Pages.F
{
    public class DeleteModel : PageModel
    {
        public void OnGet(){

        }
        public IActionResult OnPost(Int64 id){
            string dbString = Environment.GetEnvironmentVariable("AZURE_DATABASE_CONNECTION_STRING");
            using(SqlConnection con = new SqlConnection(dbString)){
                SqlCommand cmd = new SqlCommand("delete from content where id=@0", con);
                cmd.Parameters.AddWithValue("@0", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToPage("/F/Index");
        }
    }
}
