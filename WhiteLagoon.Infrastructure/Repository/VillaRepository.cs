using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class VillaRepository : Repository<Villa> ,IVillaRepository
    {

        private readonly ApplicationDbContext _dbContext;

        public VillaRepository(ApplicationDbContext dbContext) : base(dbContext)  
        {
            _dbContext = dbContext;
        }
        //public void Add(Villa entity)
        //{
        //    _dbContext.Villas.Add(entity);
        //}

        //public Villa? Get(Expression<Func<Villa, bool>> filter, string? includeProperties = null)
        //{
        //    IQueryable<Villa> query = _dbContext.Set<Villa>();
        //    if(filter != null)
        //    {
        //        query = query.Where(filter);
        //    }
        //    if (!string.IsNullOrEmpty(includeProperties))
        //    {
        //        var includePropertiesList = includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);
        //        foreach (var property in includePropertiesList)
        //        {
        //            query = query.Include(property);
        //        }
        //    }
        //    return query.FirstOrDefault();
        //}

        //public IEnumerable<Villa> GetAll(Expression<Func<Villa, bool>>? filter = null, string? includeProperties = null)
        //{
        //    IQueryable<Villa> query = _dbContext.Set<Villa>();
        //    if(filter != null)
        //    {
        //        query = query.Where(filter);    
        //    }
        //    else if (!string.IsNullOrEmpty(includeProperties))
        //    {
        //        var includePropertiesList = includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);
        //        foreach (var property in includePropertiesList)
        //        {
        //            query = query.Include(property);
        //        }
        //    }
        //    return query.ToList();
        //}

        //public void Remove(Villa entity)
        //{
        //    _dbContext.Villas.Remove(entity);
        //}

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(Villa entity)
        {
            _dbContext.Villas.Update(entity);
        }
    }
}
