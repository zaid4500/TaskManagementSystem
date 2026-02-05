using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ManagementBE.Kernel.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Repository<TEntity> : BaseRepository, IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly DbSet<TEntity> _dbSet;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        public Repository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _dbSet = DbContext?.Set<TEntity>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public virtual TEntity Find(params object[] keyValues) => _dbSet.Find(keyValues);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public virtual ValueTask<TEntity> FindAsync(params object[] keyValues) => _dbSet.FindAsync(keyValues);

        private IQueryable<TEntity> ApplyIsDeletedFilter(IQueryable<TEntity> query)
        {
            var isDeletedProperty = typeof(TEntity).GetProperty("IsDeleted");

            if (isDeletedProperty != null && isDeletedProperty.PropertyType == typeof(bool?))
            {
                var parameter = Expression.Parameter(typeof(TEntity), "e");
                var property = Expression.Property(parameter, isDeletedProperty);

                // Check for null and IsDeleted != true
                var nullCheck = Expression.Equal(property, Expression.Constant(null, typeof(bool?)));
                var falseCheck = Expression.Equal(property, Expression.Constant(false, typeof(bool?)));

                var combinedCheck = Expression.Or(falseCheck, nullCheck);
                var lambda = Expression.Lambda<Func<TEntity, bool>>(combinedCheck, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        public IQueryable<TEntity> GetAll()
        {
            return ApplyIsDeletedFilter(_dbSet);
            //return _dbSet;
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null,
                                          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, 
                                          bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            query = ApplyIsDeletedFilter(query);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }

        public virtual TEntity Insert(TEntity entity)
        {
            return _dbSet.Add(entity).Entity;
        }

        public virtual void Insert(IEnumerable<TEntity> entities) => _dbSet.AddRange(entities);

        public virtual Task InsertAsync(params TEntity[] entities) => _dbSet.AddRangeAsync(entities);

        public virtual Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken)) => _dbSet.AddRangeAsync(entities, cancellationToken);

        public void Update(TEntity entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Update(params TEntity[] entities) => _dbSet.UpdateRange(entities);

        public virtual void Update(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);

        public int Delete(object id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
                Delete(entity);
            return 1;
        }

        public int Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            return 1;
        }

        public virtual int Delete(params TEntity[] entities)
        {
            _dbSet.RemoveRange(entities);
            return 1;
        }

        public virtual int Delete(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
            return 1;
        }

        public virtual TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null,
                                                 Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                 Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                 bool disableTracking = true,
                                                 bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = ApplyIsDeletedFilter(include(query));
            }

            query = ApplyIsDeletedFilter(query);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).FirstOrDefault();
            }
            else
            {
                return query.FirstOrDefault();
            }
        }

        public virtual async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
                                                                  Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                                  Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                                  bool disableTracking = true,
                                                                  bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = ApplyIsDeletedFilter(include(query));
            }

            query = ApplyIsDeletedFilter(query);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).FirstOrDefaultAsync();
            }
            else
            {
                return await query.FirstOrDefaultAsync();
            }
        }

        public virtual TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                  Expression<Func<TEntity, bool>> predicate = null,
                                                  Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                  Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                  bool disableTracking = true,
                                                  bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = ApplyIsDeletedFilter(include(query));
            }

            query = ApplyIsDeletedFilter(query);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector).FirstOrDefault();
            }
            else
            {
                return query.Select(selector).FirstOrDefault();
            }
        }

        public virtual async Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                  Expression<Func<TEntity, bool>> predicate = null,
                                                  Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                  Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                  bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = ApplyIsDeletedFilter(include(query));
            }

            query = ApplyIsDeletedFilter(query);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).Select(selector).FirstOrDefaultAsync();
            }
            else
            {
                return await query.Select(selector).FirstOrDefaultAsync();
            }
        }

        public void Dispose()
        {
            DbContext.DisposeAsync();
        }
    }
}
