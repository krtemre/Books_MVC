using Books.DataAccess.Data;
using Books.DataAccess.Repository.IRepository;
using Books.Models;
using Books.Models.ViewModels;
using Books.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BooksWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string? id)
        {
            var user = _db.ApplicationUsers.Include(s => s.Company).FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return View(new UserVM());
            }
            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            var roleId = userRoles.FirstOrDefault(s => s.UserId == user.Id)?.RoleId;
            user.Role = roles.FirstOrDefault(s => s.Id == roleId)?.Name;

            UserVM UserVM = new()
            {
                ApplicationUser = user,
                Companies = _db.Companies.Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString(),
                }),
                Roles = roles.Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Name,
                }),
            };

            return View(UserVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(UserVM userVM)
        {
            string roleId = _db.UserRoles.FirstOrDefault(s => s.UserId == userVM.ApplicationUser.Id).RoleId;
            string oldRole = _db.Roles.FirstOrDefault(s => s.Id == roleId).Name;

            if (!(userVM.ApplicationUser.Role == oldRole))
            {
                //role updated
                ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(s => s.Id == userVM.ApplicationUser.Id);
                if (userVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = userVM.ApplicationUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _db.SaveChanges();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, userVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }

            return RedirectToAction(nameof(Index));
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _db.ApplicationUsers.Include(s => s.Company).ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(s => s.UserId == user.Id)?.RoleId;
                user.Role = roles.FirstOrDefault(s => s.Id == roleId)?.Name;

                if (user.Company == null)
                    user.Company = new Company() { Name = "" };
            }
            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string? id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(s => s.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            string message = "";

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently loced and we need to unlock them.
                objFromDb.LockoutEnd = DateTime.Now;
                message = "Successfully unlocked the user";
            }
            else
            {
                //locking the user for thousnd years.
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
                message = "Locked the user for 1000 years";
            }
            _db.SaveChanges();

            return Json(new { success = true, message = message });
        }

        #endregion
    }
}
