using Books.DataAccess.Data;
using Books.DataAccess.Repository;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityUser> _roleManager;

        private readonly IUnitOfWork _unitOfWork;

        public UserController(RoleManager<IdentityUser> roleManager, UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string? id)
        {
            var user = _unitOfWork.ApplicationUser.Get(s => s.Id == id, includeProperties: "Company");

            if (user == null)
            {
                return View(new UserVM());
            }

            user.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(s => s.Id == id))
                .GetAwaiter().GetResult().FirstOrDefault();

            UserVM UserVM = new()
            {
                ApplicationUser = user,
                Companies = _unitOfWork.Company.GetAll().Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString(),
                }),
                Roles = _roleManager.Roles.Select(s => new SelectListItem
                {
                    Text = s.UserName,
                    Value = s.UserName,
                }),
            };

            return View(UserVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(UserVM userVM)
        {
            string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(s => s.Id == userVM.ApplicationUser.Id))
                .GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(s => s.Id == userVM.ApplicationUser.Id);

            if (!(userVM.ApplicationUser.Role == oldRole))
            {
                if (userVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = userVM.ApplicationUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, userVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if (oldRole == SD.Role_Company && applicationUser.CompanyId != userVM.ApplicationUser.CompanyId)
                {
                    applicationUser.CompanyId = userVM.ApplicationUser.CompanyId;
                    _unitOfWork.ApplicationUser.Update(applicationUser);
                    _unitOfWork.Save();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();

            foreach (var user in userList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                if (user.Company == null)
                    user.Company = new Company() { Name = "" };
            }
            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string? id)
        {
            var objFromDb = _unitOfWork.ApplicationUser.Get(s => s.Id == id);
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
            _unitOfWork.ApplicationUser.Update(objFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = message });
        }

        #endregion
    }
}
