using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Repository;
using WhiteLagoon.Web.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static WhiteLagoon.Application.Common.Utility.Enum;

namespace WhiteLagoon.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        //UserManager class to perform user-related operations, such as user registration and authentication
        //RoleManager class to manage and access user roles in our application
        //SignInManager is a helper class that deals with External/Application cookies, password validation and 2FA
        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Login(string? returnUrl = null)
        {
            //returnUrl = returnUrl ?? Url.Content("~/");
            returnUrl ??= Url.Content("~/");

            LoginVm loginVm = new LoginVm()
            {
                RedirectionUrl = returnUrl
            };
            return View(loginVm);
        }

        public async Task<IActionResult> Register(string? ReturnUrl)
        {
            var isRolesExist =_roleManager.RoleExistsAsync(UserRoles.Admin.ToString()).GetAwaiter().GetResult();
            //populate the the ASPRoles table
            if (!isRolesExist)
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin.ToString()));
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Customer.ToString()));
            }

            RegisterVm userRegisterVm = new RegisterVm()
            {
                RoleSelectList = _roleManager.Roles.Select(role => new SelectListItem { Text = role.Name, Value = role.Id.ToString() })
                                                      .ToList()
            };

            if(!string.IsNullOrEmpty(ReturnUrl)) 
                userRegisterVm.RedirectionUrl = ReturnUrl;
              
            return View(userRegisterVm);
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterVm registerVm)
        {

            if (ModelState.IsValid)
            {
                AppUser newUser = new AppUser()
                {
                    Name = registerVm.Name,
                    UserName = registerVm.Email,
                    Email = registerVm.Email,
                    PhoneNumber = registerVm.PhoneNumeber,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now
                };

                //create user
                var result = _userManager.CreateAsync(newUser, registerVm.Password).GetAwaiter().GetResult();

                //assigning  a role to registered user
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(registerVm.Role))
                    {
                        var roleName = await _roleManager.FindByIdAsync(registerVm.Role.ToString());
                        if (roleName != null) 
                         await _userManager.AddToRoleAsync(newUser, roleName.ToString());
                        ModelState.AddModelError("", "User role not found");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(newUser, UserRoles.Customer.ToString());
                    }
                   await _signInManager.SignInAsync(newUser, isPersistent: false);
                    //signin the user
                    //isPersistent-Flag indicating whether the sign -in cookie should persist after the browser is closed.

                    if (!string.IsNullOrEmpty(registerVm.RedirectionUrl))
                    {
                        return LocalRedirect(registerVm.RedirectionUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Registration Failed");
            }
            else
            {
                foreach (var error in ViewData.ModelState.Values.SelectMany(modelState => modelState.Errors)) {
                    ModelState.AddModelError("", error.ToString());
                }
                RegisterVm userRegisterVm = new RegisterVm()
                {
                    RoleSelectList = _roleManager.Roles.Select(role => new SelectListItem { Text = role.Name, Value = role.Id })
                                                     .ToList()
                };
            }
                return View(registerVm);

        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginVm loginVm)
        {
            if (ModelState.IsValid)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(loginVm.Email, loginVm.Password, loginVm.RememberMe, lockoutOnFailure: false);
                if (signInResult.Succeeded)
                {
                    if (!string.IsNullOrEmpty(loginVm.RedirectionUrl))
                    {
                        return LocalRedirect(loginVm.RedirectionUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Login Credentials");
                }
            }

            return View(loginVm);
        }

        public async Task<IActionResult> Logout()
        {
           await _signInManager.SignOutAsync();
           return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }
    } 
}
 