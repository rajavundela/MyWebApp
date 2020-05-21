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

        public async Task<IActionResult> OnPost(string email, string password, bool remember, string ReturnUrl){
            string dbString = Environment.GetEnvironmentVariable("AZURE_DATABASE_CONNECTION_STRING");
            using (SqlConnection con = new SqlConnection(dbString))
            {
                SqlCommand cmd = new SqlCommand("select Id, email, password from users where email=@0", con);
                cmd.Parameters.AddWithValue("@0", email);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())//user found with email
                {
                    if (password == reader["password"].ToString())// check for password
                    {
                        var claims = new List<Claim>{
                            new Claim(ClaimTypes.Email, email),
                            new Claim(ClaimTypes.Role, "User"),
                            new Claim(ClaimTypes.NameIdentifier, reader["Id"].ToString()) //userid
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
                        TempData["Message"] = "Welcome !";
                        return Redirect(ReturnUrl == null ? "/Index" : ReturnUrl);
                    }
                    else{
                        TempData["Message"] = "Invalid Password.";
                    }
                }
                else{
                    TempData["Message"] = "User does not exist with this email.";
                }
            }
            return Page();
        }
    }
}
