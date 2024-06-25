using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using SV20T1020460.BusinessLayers;
using SV20T1020460.DomainModels;

namespace SV20T1020460.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        /// <summary>
        /// đăng nhập kh cần login 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username = "", string password = "")
        {
            ViewBag.Username = username;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập đầy đủ tên và mật khẩu!");
                return View();
            }
            var userAccount = UserAccountService.Authorize(username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại ");
                return View();
            }
            var userData = new WebUserData()
            {
                UserId = userAccount.UserID,
                UserName = userAccount.UserName,
                DisplayName = userAccount.FullName,
                Email = userAccount.Email,
                Photo = userAccount.Photo,
                ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                SessionId = HttpContext.Session.Id,
                AdditionalData = "",
                Roles = userAccount.RoleNames.Split(',').ToList(),
            };
            await HttpContext.SignInAsync(userData.CreatePrincipal());
            //return Json(User.GetUserData());
            return RedirectToAction("Index", "Home");
        }
        //[AllowAnonymous]
        //[HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult AccessDenined()
        {
            return View("Index");
        }
        public IActionResult ChangePass()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePass(UserAccount data)
        {
            var userData = new WebUserData()
            {
                UserName = data.UserName,
                DisplayName = data.FullName,
                Email = data.Email,
                Photo = data.Photo,
                Roles = data.RoleNames.Split(',').ToList()
            };
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            return View(data);
        }
        [HttpPost]
        public IActionResult SavePW(string userName, string oldPassword, string newPassword, string confirmNewPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(oldPassword)|| string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmNewPassword))
                    ModelState.AddModelError("ChangePassFailed", "Điền đầy đủ để đổi mật khẩu");
                if (confirmNewPassword == newPassword)
                {
                    var userAccount = UserAccountService.ChangePassword(userName, oldPassword, newPassword);
                    if (!userAccount)
                    {
                        ModelState.AddModelError("oldPassword", "Mật khẩu cũ không đúng");

                    }
                    else 
                    {
                        Logout();
                        return RedirectToAction("Login");
                    } 
                }
                else
                    ModelState.AddModelError("ChangePassFailed", "Xác nhận mật khẩu không hợp lệ");

                return View("ChangePass");
            }
            catch
            {
                ModelState.AddModelError("ChangePassFailed", "Đổi mật khẩu không thành công");
                return View("ChangePass");
            }

        }

    }
}
