using HospitalCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalCore.DataAccess.Interfaces
{
    public interface IEntityRepository<T> where T : IEntity
    {
        List<T> Get();
        T GetById(int id);
        int Insert(T entity);
        bool Update(T entity);
        bool Delete(int id);
    }
}
