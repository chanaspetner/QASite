using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using QASite.Data;
using Microsoft.AspNetCore.Authorization;

namespace QASite.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var connectionString = _configuration.GetConnectionString("ConStr");
            var repo = new QARepository(connectionString);
            var user = repo.Login(email, password);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var claims = new List<Claim>
            {
                new Claim("user", email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            return Redirect("/questions/index");
        }

        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(User user, string password)
        {
            var connectionString = _configuration.GetConnectionString("ConStr");
            var repo = new QARepository(connectionString);
            repo.AddUser(user, password);
            return RedirectToAction("login");
        }

        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/questions/index");
        }
    }
}
