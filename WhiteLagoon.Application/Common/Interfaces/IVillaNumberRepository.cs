using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
        //in repository pattern we donot use update or save methods,its implemented on Iunit 
        void Update(VillaNumber entity);
        void Save();
    }
}
