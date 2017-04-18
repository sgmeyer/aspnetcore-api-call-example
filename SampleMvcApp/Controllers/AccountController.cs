using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SampleMvcApp.Models;
using SampleMvcApp.ViewModels;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SampleMvcApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties() { RedirectUri = returnUrl };
            properties.Items.Add("audience", "https://api.watchguard.com/");
            properties.Items.Add("scope", "read:users update:users");

            return new ChallengeResult("Auth0", properties);
        }

        [Authorize]
        public async Task Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Auth0", new AuthenticationProperties
            {
                // Indicate here where Auth0 should redirect the user after a logout.
                // Note that the resulting absolute Uri must be whitelisted in the 
                // **Allowed Logout URLs** settings for the client.
                RedirectUri = Url.Action("Index", "Home")
            });
            await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var accessToken = User.Claims.FirstOrDefault(c => c.Type == "access_token").Value;
            var refreshToken = User.Claims.FirstOrDefault(c => c.Type == "refresh_token").Value;

            using(var client = new HttpClient()) 
            {
                client.DefaultRequestHeaders.Add("Authorization","Bearer " + accessToken);
                var url = "https://sgmeyer-clean.auth0.com/userinfo";
                var response = await client.GetAsync(url);
                var data = await response.Content.ReadAsStringAsync();
                var userInfo = JsonConvert.DeserializeObject<UserInfo>(data);

                Console.WriteLine(userInfo.Email);
                Console.WriteLine(userInfo.EmailVerified);
                Console.WriteLine(userInfo.Name);
                Console.WriteLine(userInfo.Nickname);
                Console.WriteLine(userInfo.Picture);
                Console.WriteLine(userInfo.Sub);
                Console.WriteLine(userInfo.UpdatedAt);

                return View(new UserProfileViewModel()
                {
                    Name = userInfo.Name,
                    EmailAddress = userInfo.Email,
                    ProfileImage = userInfo.Picture
                });
            }
        }

        /// <summary>
        /// This is just a helper action to enable you to easily see all claims related to a user. It helps when debugging your
        /// application to see the in claims populated from the Auth0 ID Token
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Claims()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
