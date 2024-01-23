using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository  
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IVillaRepository Villa {  get; set; }
        public IVillaNumberRepository VillaNumber { get; set; }
        public IAmenityRepository Amenity { get; set; }

        //The unit of work class serves one purpose:
        //to make sure that when you use multiple repositories, they share a single database context.
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Villa = new VillaRepository(context);
            VillaNumber = new VillaNumberRepository(context);
            Amenity = new AmenityRepository(context);
        }

    }
}
