using EmpresaT3.Areas.Identity.Data;
using EmpresaT3.Core;
using EmpresaT3.Core.Repositories;
using EmpresaT3.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace EmpresaT3.Controllers
{
    [Authorize(Roles = $"{Constants.Roles.Administrator}")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(IUnitOfWork unitOfWork, SignInManager<ApplicationUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            try
            {
                var users = _unitOfWork.User.GetUsers();
                return View(users);
            }
            catch
            {
                return NotFound("Error");
            }
            
        }

        public async Task<IActionResult> Hash(string id)
        {
            try
            {
                var user = _unitOfWork.User.GetUser(id);
                var roles = _unitOfWork.Role.GetRoles();

                var userRoles = await _signInManager.UserManager.GetRolesAsync(user);

                var roleItems = roles.Select(role =>
                    new SelectListItem(
                        role.Name,
                        role.Id,
                        userRoles.Any(ur => ur.Contains(role.Name)))).ToList();

                var vm = new EditUserViewModel
                {
                    User = user,
                    Roles = roleItems
                };

                return View(vm);
            }
            catch(Exception)
            {
                return NotFound("Error desconocido");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                if (userId == null)
                {
                    return NotFound();
                }

                var user = await _signInManager.UserManager.FindByIdAsync(userId);

                var rolesForUser = await _signInManager.UserManager.GetRolesAsync(user);

                if (rolesForUser.Count() > 0)
                {
                    foreach (var item in rolesForUser.ToList())
                    {
                        // item should be the name of the role
                        var result = await _signInManager.UserManager.RemoveFromRoleAsync(user, item);
                    }
                }

                await _signInManager.UserManager.DeleteAsync(user);
                await _signInManager.UserManager.UpdateSecurityStampAsync(user);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("Hash");
            }
            

            
        }




        [HttpPost]
        public async Task<IActionResult> OnPostAsync(EditUserViewModel data)
        {
            try
            {
                var user = _unitOfWork.User.GetUser(data.User.Id);
                if (user == null)
                {
                    return NotFound();
                }

                var userRolesInDb = await _signInManager.UserManager.GetRolesAsync(user);

                //Loop through the roles in ViewModel
                //Check if the Role is Assigned In DB
                //If Assigned -> Do Nothing
                //If Not Assigned -> Add Role

                var rolesToAdd = new List<string>();
                var rolesToDelete = new List<string>();

                foreach (var role in data.Roles)
                {
                    var assignedInDb = userRolesInDb.FirstOrDefault(ur => ur == role.Text);
                    if (role.Selected)
                    {
                        if (assignedInDb == null)
                        {
                            rolesToAdd.Add(role.Text);
                        }
                    }
                    else
                    {
                        if (assignedInDb != null)
                        {
                            rolesToDelete.Add(role.Text);
                        }
                    }
                }

                if (rolesToAdd.Any())
                {
                    await _signInManager.UserManager.AddToRolesAsync(user, rolesToAdd);
                }

                if (rolesToDelete.Any())
                {
                    await _signInManager.UserManager.RemoveFromRolesAsync(user, rolesToDelete);
                }

                user.FirstName = data.User.FirstName;
                user.LastName = data.User.LastName;
                user.Email = data.User.Email;

                _unitOfWork.User.UpdateUser(user);
                await _signInManager.UserManager.UpdateSecurityStampAsync(user);
                return RedirectToAction("Hash", new { id = user.Id });
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }

        }
    }
}
