using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Backend.Database.Model;

namespace Backend.Database.Base
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected TaxiHeavenContext _context;

        protected abstract Expression<Func<TEntity, bool>> QueryById(int id);
        protected abstract Expression<Func<TEntity, bool>> QueryById(TEntity entity);

        protected Repository()
        {
            _context = new TaxiHeavenContext();
        }

        protected Repository(TaxiHeavenContext context)
        {
            _context = context;
        }

        public TEntity Get(int id)
        {
            return _context.Set<TEntity>().FirstOrDefault(QueryById(id));
        }

        public IQueryable<TEntity> Get()
        {
            return _context.Set<TEntity>();
        }

        public TEntity Post(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public TEntity Put(TEntity entity)
        {
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
            return entity;
        }

        public int Delete(int id)
        {
            var entity = _context.Set<TEntity>().FirstOrDefault(QueryById(id));
            if (entity == null)
                return 0;
            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
            return id;
        }
    }
}
