using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class AppUserRepository : Repository<AppUser> , IAppUserRepository 
    {
        private ApplicationDbContext _context;

        public AppUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Save()
        {
            _context.SaveChanges();
        }
        public void Update(AppUser appUser)
        {
            _context.AppUsers.Update(appUser);
        }
    }
}
