using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;

namespace MyWebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string username, string password, bool remember, string ReturnUrl){
            string dbString = Environment.GetEnvironmentVariable("AZURE_DATABASE_CONNECTION_STRING");
            using (SqlConnection con = new SqlConnection(dbString))
            {
                SqlCommand cmd = new SqlCommand("select username, password from users where username=@0", con);
                cmd.Parameters.AddWithValue("@0", username);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())//user found with username
                {
                    if (password == Convert.ToString(reader["password"]))// check for password
                    {
                        var claims = new List<Claim>{
                            new Claim(ClaimTypes.Name, username),
                            new Claim(ClaimTypes.Role, "Administrator")
                        };
                        var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme
                        );
                        ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            principal,
                            new AuthenticationProperties(){
                                IsPersistent = remember
                            });
                        return RedirectToPage(ReturnUrl == null ? "/F/Index" : ReturnUrl);
                    }
                }
            }
            return Page();
        }
    }
}
