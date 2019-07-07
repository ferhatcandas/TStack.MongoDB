using MongoDB.Driver;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using TStack.MongoDB.Connection;
using TStack.MongoDB.Entity;
using TStack.MongoDB.Map;

namespace TStack.MongoDB.Repository
{
    /// <summary>
    /// repository implementation for mongo
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public abstract class MongoRepositoryBase<TEntity, TContext> : MongoRepositoryBase<TEntity>, IMongoRepository<TEntity>
        where TEntity : IMongoEntity
        where TContext : MongoConnection, new()
    {
        private readonly TContext _context;
        /// <summary>
        /// 
        /// </summary>
        public MongoRepositoryBase()
        {
            _context = new TContext();
            Collection = Database<TEntity>.GetCollectionFromMongoConnection(_context);
        }
    }
    public abstract class MongoRepositoryBase<TEntity, TContext, IMapper> : MongoRepositoryBase<TEntity>, IMongoRepository<TEntity, IMapper>
      where TEntity : IMongoEntity
      where TContext : MongoConnection, new()
      where IMapper : Mapper<TEntity>, new()
    {
        private readonly TContext _context;
        private readonly IMapper _mapper;

        public MongoRepositoryBase()
        {
            _context = new TContext();
            Collection = Database<TEntity>.GetCollectionFromMongoConnection(_context);
            _mapper = new IMapper();
        }
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, Func<Rule<TEntity>, bool> rule)
        {
            var list = Find(filter);
            Includer(list, rule);
            return list;
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, int pageIndex, int size, Func<Rule<TEntity>, bool> rule)
        {
            var list = Find(filter, pageIndex, size);
            Includer(list, rule);
            return list;
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, int pageIndex, int size, Func<Rule<TEntity>, bool> rule)
        {
            var list = Find(filter, order, pageIndex, size);
            Includer(list, rule);
            return list;
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, int pageIndex, int size, bool isDescending, Func<Rule<TEntity>, bool> rule)
        {
            var list = Find(filter, order, pageIndex, size);
            Includer(list, rule);
            return list;
        }

        public IEnumerable<TEntity> FindAll(Func<Rule<TEntity>, bool> rule)
        {
            var list = FindAll();
            Includer(list, rule);
            return list;
        }

        public IEnumerable<TEntity> FindAll(int pageIndex, int size, Func<Rule<TEntity>, bool> rule)
        {
            var list = FindAll(pageIndex, size);
            Includer(list, rule);
            return list;
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, object>> order, int pageIndex, int size, Func<Rule<TEntity>, bool> rule)
        {
            var list = FindAll(order, pageIndex, size);
            Includer(list, rule);
            return list;
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, object>> order, int pageIndex, int size, bool isDescending, Func<Rule<TEntity>, bool> rule)
        {
            var list = FindAll(order, pageIndex, size, isDescending);
            Includer(list, rule);
            return list;
        }

        public TEntity First(Func<Rule<TEntity>, bool> rule)
        {
            var data = First();
            Includer(data, rule);
            return data;
        }

        public TEntity First(Expression<Func<TEntity, bool>> filter, Func<Rule<TEntity>, bool> rule)
        {
            var data = First(filter);
            Includer(data, rule);
            return data;
        }

        public TEntity First(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, Func<Rule<TEntity>, bool> rule)
        {
            var data = First(filter, order);
            Includer(data, rule);
            return data;
        }

        public TEntity First(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, bool isDescending, Func<Rule<TEntity>, bool> rule)
        {
            var data = First(filter, order, isDescending);
            Includer(data, rule);
            return data;
        }

        public TEntity Get(string id, Func<Rule<TEntity>, bool> rule)
        {
            var data = Get(id);
            Includer(data, rule);
            return data;
        }

        public TEntity Last(Func<Rule<TEntity>, bool> rule)
        {
            var data = Last();
            Includer(data, rule);
            return data;
        }

        public TEntity Last(Expression<Func<TEntity, bool>> filter, Func<Rule<TEntity>, bool> rule)
        {
            var data = Last(filter);
            Includer(data, rule);
            return data;
        }

        public TEntity Last(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, Func<Rule<TEntity>, bool> rule)
        {
            var data = Last(filter, order);
            Includer(data, rule);
            return data;
        }

        public TEntity Last(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, bool isDescending, Func<Rule<TEntity>, bool> rule)
        {
            var data = Last(filter, order, isDescending);
            Includer(data, rule);
            return data;
        }
        private void Includer(IEnumerable<TEntity> entities, Func<Rule<TEntity>, bool> rule)
        {
            foreach (var item in entities)
                Includer(item, rule);
        }
        private void Includer(TEntity entity, Func<Rule<TEntity>, bool> rule)
        {
            var filters = _mapper.Rules.Where(rule);

            if (filters.Count() > 0)
            {
                foreach (var filt in filters)
                {
                    var filterValue = entity.GetPropertyValue(filt.PrimaryKey);

                    var repository = filt.TargetType.GetRepositoryFromType(_context);

                    object value = null;
                    switch (filt.RelationType)
                    {
                        case RelationType.One:
                            value = GetValueItemFromDynamicRepository(repository, filt.TargetType, filt.TargetKey, filterValue);
                            break;
                        case RelationType.Collection:
                            value = GetValueCollectionFromDynamicRepository(repository, filt.TargetType, filt.TargetKey, filterValue);
                            break;
                    }
                    entity.SetPropertyValue(filt.LocalFieldName, value);
                }
            }
        }
        private object GetValueCollectionFromDynamicRepository(object repository, Type targetType, string targetKey, string targetValue)
        {

            var method = repository.GetFilterMethod("Find");

            var expr = targetType.GetExpression(targetKey, targetValue);

            var value = ((IEnumerable<object>)method.Invoke(repository, new object[] { expr })).ToList();

            return value.ObjectListToSpecificTypeList(targetType);
        }
        private object GetValueItemFromDynamicRepository(object repository, Type targetType, string targetKey, string targetValue)
        {
            var method = repository.GetFilterMethod("First");

            var expr = targetType.GetExpression(targetKey, targetValue);

            return method.Invoke(repository, new object[] { expr });
        }
    }
    public class MongoRepositoryBase<TEntity> : IMongoRepository<TEntity>
     where TEntity : IMongoEntity
    {
        public MongoRepositoryBase()
        {

        }
        public MongoRepositoryBase(MongoConnection mongoConnection)
        {
            Collection = Database<TEntity>.GetCollectionFromMongoConnection(mongoConnection);
        }
        #region MongoSpecific
        /// <summary>
        /// mongo collection
        /// </summary>
        public IMongoCollection<TEntity> Collection
        {
            get; internal set;
        }
        /// <summary>
        /// filter for collection
        /// </summary>
        public FilterDefinitionBuilder<TEntity> Filter => Builders<TEntity>.Filter;

        /// <summary>
        /// projector for collection
        /// </summary>
        public ProjectionDefinitionBuilder<TEntity> Project => Builders<TEntity>.Projection;

        /// <summary>
        /// updater for collection
        /// </summary>
        public UpdateDefinitionBuilder<TEntity> Updater => Builders<TEntity>.Update;

        private IFindFluent<TEntity, TEntity> Query(Expression<Func<TEntity, bool>> filter) => Collection.Find(filter);

        private IFindFluent<TEntity, TEntity> Query() => Collection.Find(Filter.Empty);
        #endregion MongoSpecific
        #region CRUD

        #region Delete

        /// <summary>
        /// delete entity
        /// </summary>
        /// <param name="entity">entity</param>
        public bool Delete(TEntity entity) => Delete(entity.Id);

        /// <summary>
        /// delete entity
        /// </summary>
        /// <param name="entity">entity</param>
        public Task<bool> DeleteAsync(TEntity entity) => Task.Run(() =>
        {
            return Delete(entity);
        });
        /// <summary>
        /// delete by id
        /// </summary>
        /// <param name="id">id</param>
        public virtual bool Delete(string id) => Retry(() =>
        {
            return Collection.DeleteOne(i => i.Id == id).IsAcknowledged;
        });

        /// <summary>
        /// delete by id
        /// </summary>
        /// <param name="id">id</param>
        public virtual Task<bool> DeleteAsync(string id) => Retry(() =>
        {
            return Task.Run(() =>
            {
                return Delete(id);
            });
        });

        /// <summary>
        /// delete items with filter
        /// </summary>
        /// <param name="filter">expression filter</param>
        public bool Delete(Expression<Func<TEntity, bool>> filter) => Retry(() =>
        {
            return Collection.DeleteMany(filter).IsAcknowledged;
        });

        /// <summary>
        /// delete items with filter
        /// </summary>
        /// <param name="filter">expression filter</param>
        public Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> filter) => Retry(() =>
        {
            return Task.Run(() =>
            {
                return Delete(filter);
            });
        });

        /// <summary>
        /// delete all documents
        /// </summary>
        public virtual bool DeleteAll() => Retry(() =>
        {
            return Collection.DeleteMany(Filter.Empty).IsAcknowledged;
        });

        /// <summary>
        /// delete all documents
        /// </summary>
        public virtual Task<bool> DeleteAllAsync() => Retry(() =>
        {
            return Task.Run(() =>
            {
                return DeleteAll();
            });
        });
        #endregion Delete

        #region Find

        /// <summary>
        /// find entities
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>collection of entity</returns>
        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter) => Query(filter).ToEnumerable();

        /// <summary>
        /// find entities with paging
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of entity</returns>
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, int pageIndex, int size)
        {
            return Find(filter, i => i.Id, pageIndex, size);
        }

        /// <summary>
        /// find entities with paging and ordering
        /// default ordering is descending
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of entity</returns>
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, int pageIndex, int size)
        {
            return Find(filter, order, pageIndex, size, true);
        }

        /// <summary>
        /// find entities with paging and ordering in direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>collection of entity</returns>
        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, int pageIndex, int size, bool isDescending)
        {
            return Retry(() =>
            {
                var query = Query(filter).Skip(pageIndex * size).Limit(size);
                return (isDescending ? query.SortByDescending(order) : query.SortBy(order)).ToEnumerable();
            });
        }

        #endregion Find

        #region FindAll

        /// <summary>
        /// fetch all items in collection
        /// </summary>
        /// <returns>collection of entity</returns>
        public virtual IEnumerable<TEntity> FindAll()
        {
            return Retry(() =>
            {
                return Query().ToEnumerable();
            });
        }

        /// <summary>
        /// fetch all items in collection with paging
        /// </summary>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of entity</returns>
        public IEnumerable<TEntity> FindAll(int pageIndex, int size)
        {
            return FindAll(i => i.Id, pageIndex, size);
        }

        /// <summary>
        /// fetch all items in collection with paging and ordering
        /// default ordering is descending
        /// </summary>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of entity</returns>
        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, object>> order, int pageIndex, int size)
        {
            return FindAll(order, pageIndex, size, true);
        }

        /// <summary>
        /// fetch all items in collection with paging and ordering in direction
        /// </summary>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>collection of entity</returns>
        public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, object>> order, int pageIndex, int size, bool isDescending)
        {
            return Retry(() =>
            {
                var query = Query().Skip(pageIndex * size).Limit(size);
                return (isDescending ? query.SortByDescending(order) : query.SortBy(order)).ToEnumerable();
            });
        }

        #endregion FindAll

        #region First

        /// <summary>
        /// get first item in collection
        /// </summary>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public TEntity First()
        {
            return FindAll(i => i.Id, 0, 1, false).FirstOrDefault();
        }

        /// <summary>
        /// get first item in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public TEntity First(Expression<Func<TEntity, bool>> filter)
        {
            return First(filter, i => i.Id);
        }

        /// <summary>
        /// get first item in query with order
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public TEntity First(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order)
        {
            return First(filter, order, false);
        }

        /// <summary>
        /// get first item in query with order and direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public TEntity First(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, bool isDescending)
        {
            return Find(filter, order, 0, 1, isDescending).FirstOrDefault();
        }

        #endregion First

        #region Get

        /// <summary>
        /// get by id
        /// </summary>
        /// <param name="id">id value</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual TEntity Get(string id)
        {
            return Retry(() =>
            {
                return Find(i => i.Id == id).FirstOrDefault();
            });
        }

        #endregion Get

        #region Insert

        /// <summary>
        /// insert entity
        /// </summary>
        /// <param name="entity">entity</param>
        public virtual void Insert(TEntity entity)
        {
            Retry(() =>
            {
                Collection.InsertOne(entity);
                return true;
            });
        }

        /// <summary>
        /// insert entity
        /// </summary>
        /// <param name="entity">entity</param>
        public virtual Task InsertAsync(TEntity entity)
        {
            return Retry(() =>
            {
                return Collection.InsertOneAsync(entity);
            });
        }

        /// <summary>
        /// insert entity collection
        /// </summary>
        /// <param name="entities">collection of entities</param>
        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            Retry(() =>
            {
                Collection.InsertMany(entities);
                return true;
            });
        }

        /// <summary>
        /// insert entity collection
        /// </summary>
        /// <param name="entities">collection of entities</param>
        public virtual Task InsertAsync(IEnumerable<TEntity> entities)
        {
            return Retry(() =>
            {
                return Collection.InsertManyAsync(entities);
            });
        }
        #endregion Insert

        #region Last

        /// <summary>
        /// get first item in collection
        /// </summary>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public TEntity Last()
        {
            return FindAll(i => i.Id, 0, 1, true).FirstOrDefault();
        }

        /// <summary>
        /// get last item in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public TEntity Last(Expression<Func<TEntity, bool>> filter)
        {
            return Last(filter, i => i.Id);
        }

        /// <summary>
        /// get last item in query with order
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public TEntity Last(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order)
        {
            return Last(filter, order, false);
        }

        /// <summary>
        /// get last item in query with order and direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public TEntity Last(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, bool isDescending)
        {
            return First(filter, order, !isDescending);
        }

        #endregion Last

        #region Replace

        /// <summary>
        /// replace an existing entity
        /// </summary>
        /// <param name="entity">entity</param>
        public virtual bool Replace(TEntity entity)
        {
            return Retry(() =>
            {
                return Collection.ReplaceOne(i => i.Id == entity.Id, entity).IsAcknowledged;
            });
        }

        /// <summary>
        /// replace an existing entity
        /// </summary>
        /// <param name="entity">entity</param>
        public virtual Task<bool> ReplaceAsync(TEntity entity)
        {
            return Retry(() =>
            {
                return Task.Run(() =>
                {
                    return Replace(entity);
                });
            });
        }

        /// <summary>
        /// replace collection of entities
        /// </summary>
        /// <param name="entities">collection of entities</param>
        public void Replace(IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {
                Replace(entity);
            }
        }

        #endregion Replace

        #region Update

        /// <summary>
        /// update a property field in an entity
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="entity">entity</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Update<TField>(TEntity entity, Expression<Func<TEntity, TField>> field, TField value)
        {
            return Update(entity, Updater.Set(field, value));
        }

        /// <summary>
        /// update a property field in an entity
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="entity">entity</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        public Task<bool> UpdateAsync<TField>(TEntity entity, Expression<Func<TEntity, TField>> field, TField value)
        {
            return Task.Run(() =>
            {
                return Update(entity, Updater.Set(field, value));
            });
        }

        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        public virtual bool Update(string id, params UpdateDefinition<TEntity>[] updates)
        {
            return Update(Filter.Eq(i => i.Id, id), updates);
        }

        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="updates">updated field(s)</param>
        public virtual Task<bool> UpdateAsync(string id, params UpdateDefinition<TEntity>[] updates)
        {
            return Task.Run(() =>
            {
                return Update(Filter.Eq(i => i.Id, id), updates);
            });
        }

        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        public virtual bool Update(TEntity entity, params UpdateDefinition<TEntity>[] updates)
        {
            return Update(entity.Id, updates);
        }

        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="updates">updated field(s)</param>
        public virtual Task<bool> UpdateAsync(TEntity entity, params UpdateDefinition<TEntity>[] updates)
        {
            return Task.Run(() =>
            {
                return Update(entity.Id, updates);
            });
        }

        /// <summary>
        /// update a property field in entities
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="filter">filter</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Update<TField>(FilterDefinition<TEntity> filter, Expression<Func<TEntity, TField>> field, TField value)
        {
            return Update(filter, Updater.Set(field, value));
        }

        /// <summary>
        /// update a property field in entities
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="filter">filter</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        public Task<bool> UpdateAsync<TField>(FilterDefinition<TEntity> filter, Expression<Func<TEntity, TField>> field, TField value)
        {
            return Task.Run(() =>
            {
                return Update(filter, Updater.Set(field, value));
            });
        }

        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Update(FilterDefinition<TEntity> filter, params UpdateDefinition<TEntity>[] updates)
        {
            return Retry(() =>
            {
                var update = Updater.Combine(updates);//.CurrentDate(i => i.UpdateAt);
                return Collection.UpdateMany(filter, update/*.CurrentDate(i => i.UpdateAt)*/).IsAcknowledged;
            });
        }

        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        public Task<bool> UpdateAsync(FilterDefinition<TEntity> filter, params UpdateDefinition<TEntity>[] updates)
        {
            return Retry(() =>
            {
                return Task.Run(() =>
                {
                    return Update(filter, updates);
                });
            });
        }

        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Update(Expression<Func<TEntity, bool>> filter, params UpdateDefinition<TEntity>[] updates)
        {
            return Retry(() =>
            {
                var update = Updater.Combine(updates);//.CurrentDate(i => i.UpdateAt);
                return Collection.UpdateMany(filter, update).IsAcknowledged;
            });
        }

        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        public Task<bool> UpdateAsync(Expression<Func<TEntity, bool>> filter, params UpdateDefinition<TEntity>[] updates)
        {
            return Retry(() =>
            {
                return Task.Run(() =>
                {
                    return Update(filter, updates);
                });
            });
        }

        #endregion Update

        #endregion CRUD

        #region Utils

        /// <summary>
        /// validate if filter result exists
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>true if exists, otherwise false</returns>
        public bool Any(Expression<Func<TEntity, bool>> filter)
        {
            return Retry(() =>
            {
                return First(filter) != null;
            });
        }

        #region Count
        /// <summary>
        /// get number of filtered documents
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>number of documents</returns>
        public long Count(Expression<Func<TEntity, bool>> filter)
        {
            return Retry(() =>
            {
                return Collection.Count(filter);
            });
        }

        /// <summary>
        /// get number of filtered documents
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>number of documents</returns>
        public Task<long> CountAsync(Expression<Func<TEntity, bool>> filter)
        {
            return Retry(() =>
            {
                return Collection.CountAsync(filter);
            });
        }

        /// <summary>
        /// get number of documents in collection
        /// </summary>
        /// <returns>number of documents</returns>
        public long Count()
        {
            return Retry(() =>
            {
                return Collection.Count(Filter.Empty);
            });
        }

        /// <summary>
        /// get number of documents in collection
        /// </summary>
        /// <returns>number of documents</returns>
        public Task<long> CountAsync()
        {
            return Retry(() =>
            {
                return Collection.CountAsync(Filter.Empty);
            });
        }
        #endregion Count

        #endregion Utils

        #region RetryPolicy
        /// <summary>
        /// retry operation for three times if IOException occurs
        /// </summary>
        /// <typeparam name="TResult">return type</typeparam>
        /// <param name="action">action</param>
        /// <returns>action result</returns>
        /// <example>
        /// return Retry(() => 
        /// { 
        ///     do_something;
        ///     return something;
        /// });
        /// </example>
        protected virtual TEntityResult Retry<TEntityResult>(Func<TEntityResult> action)
        {
            return RetryPolicy
                .Handle<MongoConnectionException>(i => i.InnerException.GetType() == typeof(IOException) ||
                                                       i.InnerException.GetType() == typeof(SocketException))
                .Retry(0)
                .Execute(action);
        }
        #endregion
    }
}
