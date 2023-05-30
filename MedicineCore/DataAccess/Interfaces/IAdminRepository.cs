using HospitalCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalCore.DataAccess.Interfaces
{
    public interface IAdminRepository : IEntityRepository<Admin>
    {
        Admin Get(string username);
    }
}
