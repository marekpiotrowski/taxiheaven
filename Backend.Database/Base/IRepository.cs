using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Database.Base
{
    public interface IRepository<TEntity> where TEntity: class
    {
        TEntity Get(int id);
        IQueryable<TEntity> Get();
        TEntity Post(TEntity entity);
        TEntity Put(TEntity entity);
        int Delete(int id);
    }
}
