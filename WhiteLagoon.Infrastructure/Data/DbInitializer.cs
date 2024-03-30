using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using static WhiteLagoon.Application.Common.Utility.Enum;

namespace WhiteLagoon.Infrastructure.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        //UserManager class to perform user-related operations, such as user registration and authentication
        //RoleManager class to manage and access user roles in our application
        //SignInManager is a helper class that deals with External/Application cookies, password validation and 2FA
        public DbInitializer(ApplicationDbContext dbContext,UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }
        public async void Initialize()
        {
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Count() > 0)
                {
                    _dbContext.Database.Migrate();
                }
                var isRolesExist = _roleManager.RoleExistsAsync(UserRoles.Admin.ToString()).GetAwaiter().GetResult();
                //populate the the ASPRoles table
                if (!isRolesExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin.ToString()));
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Customer.ToString()));


                    //adding admin to db
                     _userManager.CreateAsync( new AppUser {
                        UserName = "adminVilla",
                        Email = "adminVilla@WhiteLagoon.com",
                        Name = "adminVilla",
                        NormalizedUserName = "ADMINVILLA",
                        NormalizedEmail = "ADMINVILLA@WHITELAGOON.COM",
                        PhoneNumber = "07736859233"
                    }, "AdminVilla123!" ).GetAwaiter().GetResult();
                }
                    
                AppUser admin = _dbContext.AppUsers.FirstOrDefault(user=>user.Email == "AdminVilla@WhiteLagoon.com");
                _userManager.AddToRoleAsync(admin, nameof(UserRoles.Admin).ToString()).GetAwaiter().GetResult();
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }
    }
}
