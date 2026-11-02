 using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversityAssestManagement.Models;
using UniversityAssestManagement.Models.ViewModels;
//using UniversityAssetsManagement.Models.ViewModels;
//using UniversityAssetsManagement.Models.ViewModels;

namespace UniversityAssetsManagement.Controllers
{
    public class AccountController : Controller
    {

        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }




        [HttpGet]
        public IActionResult Login()
        {

            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)

        {
            //Console.WriteLine("=== LOGIN ATTEMPT ===");
            //Console.WriteLine($"Email: {model.Email}");
            //Console.WriteLine($"Password is null: {model.Password is null}");
            //Console.WriteLine($"ModelState valid: {ModelState.IsValid}");

            if (!ModelState.IsValid) return View(model);

            var user= await _userManager.FindByEmailAsync(model.Email);

            if(user is null)
            {
                ModelState.AddModelError("", "Invalid email or password");

                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError("", " Your account has been disabled. Contact the administrator");
                return View(model);
            }



            var result = await _signInManager.PasswordSignInAsync( model.Email, model.Password, isPersistent: false, lockoutOnFailure: true);


            if (result.Succeeded)
            {

                // Add CollegeId claim so services can read it
                if (user.CollegeId.HasValue)
                {
                    var claims = new List<Claim>
                    {
                        new Claim("CollegeId",   user.CollegeId.Value.ToString()),
                        new Claim("CollegeName", user.CollegeName ?? ""),
                    };
                                await _userManager.AddClaimsAsync(user, claims);
                    await _signInManager.RefreshSignInAsync(user);
                }

                 
              // if (user.MustChangePassword) return RedirectToAction("ChangePassword", "Account");

                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("SuperAdmin"))
                    return RedirectToAction("Index", "Dashboard");

                if (roles.Contains("CollegeAdmin"))
                    return RedirectToAction("Index", "CollegeAdmin");

                return RedirectToAction("Index", "Dashboard");  
            }


            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Account locked due to too many attempts. Try again in 10 minutes");
                return View(model);
            }
            ModelState.AddModelError("", "Invalid email or passwordd");
            return View (model);

        }



        //changepassword

        //[HttpGet]
        //public IActionResult ChangePasswrod() => View();


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        //{

        //    if (!ModelState.IsValid) return View(model);

        //    var user = await _userManager.GetUserAsync(User);
        //    if (user is null) return RedirectToAction("Login");

        //    var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        //    if (result.Succeeded)
        //    {

        //        user.MustChangePassword = false;
        //        await _userManager.UpdateAsync(user);
        //        await _signInManager.RefreshSignInAsync(user);


        //        TempData["success"] = "Password changed successfully";

        //        var role = await _userManager.GetRolesAsync(user);
        //        return role.Contains("SuperAdmin") ? RedirectToAction("Index", "Home") : RedirectToAction("Index", "CollegeUser");
        //    }

        //    foreach (var error in result.Errors)
        //        ModelState.AddModelError("", error.Description);


        //    return View(model);
        //}

        public IActionResult Index()
        {
            return View();
        }



        public IActionResult CollegeLogin()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()=> View();




    }
}
