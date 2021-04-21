using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RepositoryPattern
{
    public abstract class Respository<TEntity, TDbContext> where TEntity : class, new() where TDbContext : DbContext
    {
        private readonly TDbContext _DbContext;

        public Respository(TDbContext context)
        {
            _DbContext = context;
        }

        public virtual IEnumerable<TEntity> Find() => _DbContext.Set<TEntity>().AsNoTracking().ToArray();

        public virtual IEnumerable<TEntity> Find(short count) => _DbContext.Set<TEntity>().AsNoTracking().Take(count).ToArray();

        public virtual TEntity Find<TKey>(TKey id) => _DbContext.Set<TEntity>().Find(id);

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate) => _DbContext.Set<TEntity>().AsNoTracking().Where(predicate).ToArray();

        public virtual bool All(Expression<Func<TEntity, bool>> predicate) => _DbContext.Set<TEntity>().All(predicate);

        public virtual bool Any(Expression<Func<TEntity, bool>> predicate) => _DbContext.Set<TEntity>().Any(predicate);

        public virtual TEntity Insert(TEntity entity)
        {
            _DbContext.Set<TEntity>().Add(entity);
            return entity;
        }

        public virtual List<TEntity> Insert(List<TEntity> entities)
        {
            _DbContext.Set<TEntity>().AddRange(entities);
            return entities;
        }

        public virtual IEnumerable<TEntity> Insert(IEnumerable<TEntity> entities)
        {
            _DbContext.Set<TEntity>().AddRange(entities);
            return entities;
        }

        public virtual TEntity Update(TEntity entity)
        {
            _DbContext.Set<TEntity>().Update(entity);
            return entity;
        }

        public virtual List<TEntity> Update(List<TEntity> entities)
        {
            _DbContext.Set<TEntity>().UpdateRange(entities);
            return entities;
        }

        public virtual IEnumerable<TEntity> Update(IEnumerable<TEntity> entities)
        {
            _DbContext.Set<TEntity>().UpdateRange(entities);
            return entities;
        }

        public virtual void Delete<TKey>(TKey id) => _DbContext.Set<TEntity>().Remove(Find(id));

        public virtual void Delete(TEntity entity) => _DbContext.Set<TEntity>().Remove(entity);

        public virtual void Delete(List<TEntity> entities) => _DbContext.Set<TEntity>().RemoveRange(entities);
    }
}
