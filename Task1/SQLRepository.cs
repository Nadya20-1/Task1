using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Task1
{
    class SQLRepository<People> : IRepository<People> where People : class
    {

        public DbContext _context;
        public DbSet<People> dbSet;

        public SQLRepository(DbContext context)
        {
            _context = context;
            dbSet = context.Set<People>();
        }

        public IEnumerable<People> Get()
        {
            return dbSet.AsNoTracking().ToList();
        }

        public IEnumerable<People> Get(Func<People, bool> predicate)
        {
            return dbSet.AsNoTracking().Where(predicate).ToList();
        }
        public People FindById(int id)
        {
            return dbSet.Find(id);
        }

        public void Create(People item)
        {
            dbSet.Add(item);
            _context.SaveChanges();
        }

        public void Update(People item)
        {
            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public void Remove(People item)
        {
            dbSet.Remove(item);
            _context.SaveChanges();
        }

        public IEnumerable<People> GetWithInclude(params Expression<Func<People, object>>[] includeProperties)
        {
            return Include(includeProperties).ToList();
        }

        public IEnumerable<People> GetWithInclude(Func<People, bool> predicate,
            params Expression<Func<People, object>>[] includeProperties)
        {
            var query = Include(includeProperties);
            return query.Where(predicate).ToList();
        }

        private IQueryable<People> Include(params Expression<Func<People, object>>[] includeProperties)
        {
            IQueryable<People> query = dbSet.AsNoTracking();
            return includeProperties
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

    }
}
