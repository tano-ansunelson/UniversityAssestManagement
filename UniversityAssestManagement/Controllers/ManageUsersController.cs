using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAssestManagement.Data;
using UniversityAssestManagement.Models;
using UniversityAssestManagement.Models.ViewModels;
//using UniversityAssetsManagement.Models.ViewModels;

namespace UniversityAssestManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class ManageUsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;

        public ManageUsersController(
            UserManager<AppUser> userManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        // ── GET /ManageUsers ─────────────────────────────────────────
        // Lists all college admin accounts
        public async Task<IActionResult> Index()
        {
            var collegeUsers = await _userManager.GetUsersInRoleAsync("CollegeAdmin");

            var rows = collegeUsers.Select(u => new CollegeUserRow
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? "",
                CollegeName = u.CollegeName ?? "—",
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                CreatedBy = u.CreatedBy ?? "—",
            })
            .OrderBy(u => u.CollegeName)
            .ToList();

            return View(new CollegeUserListViewModel { Users = rows });
        }

        // ── GET /ManageUsers/Create ──────────────────────────────────
        public async Task<IActionResult> Create()
        {
            var colleges = await GetCollegeDropdownAsync();
            Console.WriteLine($"Colleges loaded:{colleges.Count}");
            var vm = new CreateCollegeUserViewModel
            {
                Colleges = await GetCollegeDropdownAsync()
            };
            return View(vm);
        }

        // ── POST /ManageUsers/Create ─────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCollegeUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(x => x.Errors))
                    Console.WriteLine("Validation Error:" + error.ErrorMessage);

                model.Colleges = await GetCollegeDropdownAsync();
                return View(model);
            }

            // Check email not already taken
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                model.Colleges = await GetCollegeDropdownAsync();
                return View(model);
            }

            // Get college name from DB
            var college = await _db.Colleges.FindAsync(model.CollegeId);
            if (college is null)
            {
                ModelState.AddModelError("CollegeId", "Selected college not found.");
                model.Colleges = await GetCollegeDropdownAsync();
                return View(model);
            }

            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                CollegeId = model.CollegeId,
                CollegeName = college.Name,
                IsActive = true,
                MustChangePassword = true,  // forces password change on first login
                CreatedAt = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name ?? "SuperAdmin",
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                model.Colleges = await GetCollegeDropdownAsync();
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, "CollegeAdmin");

            TempData["Success"] =
                $"Account created for {model.FullName} ({college.Name}).";

            return RedirectToAction(nameof(Index));
        }

        // ── POST /ManageUsers/ToggleActive ───────────────────────────
        // Enable or disable an account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = user.IsActive
                ? $"{user.FullName}'s account has been enabled."
                : $"{user.FullName}'s account has been disabled.";

            return RedirectToAction(nameof(Index));
        }

        // ── POST /ManageUsers/ResetPassword ──────────────────────────
        // Super admin resets a college user's password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            // Remove old password and set new one
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                user.MustChangePassword = true;
                await _userManager.UpdateAsync(user);
                TempData["Success"] =
                    $"Password reset for {user.FullName}. They must change it on next login.";
            }
            else
            {
                TempData["Error"] = string.Join(", ",
                    result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Index));
        }

        // ── GET /ManageUsers/Edit/id ─────────────────────────────────
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            var vm = new CreateCollegeUserViewModel
            {
                FullName = user.FullName,
                Email = user.Email ?? "",
                CollegeId = user.CollegeId ?? 0,
                Colleges = await GetCollegeDropdownAsync(),
            };

            ViewBag.UserId = id;
            return View(vm);
        }

        // ── POST /ManageUsers/Edit/id ────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CreateCollegeUserViewModel model)
        {
            // Remove password validations for edit
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");

            if (!ModelState.IsValid)
            {
                model.Colleges = await GetCollegeDropdownAsync();
                ViewBag.UserId = id;
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            var college = await _db.Colleges.FindAsync(model.CollegeId);
            if (college is null)
            {
                ModelState.AddModelError("CollegeId", "Selected college not found.");
                model.Colleges = await GetCollegeDropdownAsync();
                ViewBag.UserId = id;
                return View(model);
            }

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.CollegeId = model.CollegeId;
            user.CollegeName = college.Name;

            await _userManager.UpdateAsync(user);
            TempData["Success"] = $"{user.FullName}'s account updated.";
            return RedirectToAction(nameof(Index));
        }

        // ── Helper: college dropdown ─────────────────────────────────
        private async Task<List<CollegeDropdownItem>> GetCollegeDropdownAsync() =>
            await _db.Colleges
                     .OrderBy(c => c.Name)
                     .Select(c => new CollegeDropdownItem
                     {
                         Id = c.Id,
                         Name = c.Name,
                     })
                     .ToListAsync();



        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {

            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return Forbid();
            return Json(new
            {
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                college = user.CollegeName,
            });
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user is null) return Forbid();


            //remove from role first

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
                await _userManager.RemoveFromRolesAsync(user, roles);
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                TempData["Success"] = $"{user.FullName}'s account has been permanently deleted.";
            else
                TempData["Error"]= string.Join(",", result.Errors.Select(e => e.Description));

            return RedirectToAction(nameof(Index));
        }
    }
}