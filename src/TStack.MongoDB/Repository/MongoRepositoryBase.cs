using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class MongoRepositoryBase<TEntity, TContext> : MongoRepositoryBase<TEntity>, IMongoRepository<TEntity>
        where TEntity : IMongoEntity
        where TContext : MongoConnection, new()
    {
        /// <summary>
        /// 
        /// </summary>
        public MongoRepositoryBase() : base(new TContext())
        {
        }
    }
    public class MongoRepositoryBase<TEntity, TContext, IMapper> : MongoRepositoryBase<TEntity>, IMongoRepository<TEntity, IMapper>
      where TEntity : IMongoEntity
      where TContext : MongoConnection, new()
      where IMapper : Mapper<TEntity>, new()
    {
        private readonly TContext _context;
        private readonly IMapper _mapper;
        public MongoRepositoryBase()
        {
            _context = new TContext();
            Collection = Database<TEntity>.GetCollectionFromClientConnection(_context);
            _mapper = new IMapper();
        }
        #region Insert
        public void Insert(TEntity entity, Func<Rule<TEntity>, bool> rule)
        {
            Collection.InsertOne(entity);
            Importer(entity, rule);
        }

        public void Insert(IEnumerable<TEntity> entities, Func<Rule<TEntity>, bool> rule)
        {
            Collection.InsertMany(entities);
            Importer(entities, rule);
        }
        #endregion 

        #region Read

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, Func<Rule<TEntity>, bool> rule)
        {
            var list = Find(filter);
            Exporter(list, rule);
            return list;
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, int pageIndex, int size, Func<Rule<TEntity>, bool> rule)
        {
            var list = Find(filter, pageIndex, size);
            Exporter(list, rule);
            return list;
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, int pageIndex, int size, Func<Rule<TEntity>, bool> rule)
        {
            var list = Find(filter, order, pageIndex, size);
            Exporter(list, rule);
            return list;
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, int pageIndex, int size, bool isDescending, Func<Rule<TEntity>, bool> rule)
        {
            var list = Find(filter, order, pageIndex, size);
            Exporter(list, rule);
            return list;
        }

        public IEnumerable<TEntity> FindAll(Func<Rule<TEntity>, bool> rule)
        {
            var list = FindAll();
            Exporter(list, rule);
            return list;
        }

        public IEnumerable<TEntity> FindAll(int pageIndex, int size, Func<Rule<TEntity>, bool> rule)
        {
            var list = FindAll(pageIndex, size);
            Exporter(list, rule);
            return list;
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, object>> order, int pageIndex, int size, Func<Rule<TEntity>, bool> rule)
        {
            var list = FindAll(order, pageIndex, size);
            Exporter(list, rule);
            return list;
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, object>> order, int pageIndex, int size, bool isDescending, Func<Rule<TEntity>, bool> rule)
        {
            var list = FindAll(order, pageIndex, size, isDescending);
            Exporter(list, rule);
            return list;
        }

        public TEntity First(Func<Rule<TEntity>, bool> rule)
        {
            var data = First();
            Exporter(data, rule);
            return data;
        }

        public TEntity First(Expression<Func<TEntity, bool>> filter, Func<Rule<TEntity>, bool> rule)
        {
            var data = First(filter);
            Exporter(data, rule);
            return data;
        }

        public TEntity First(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, Func<Rule<TEntity>, bool> rule)
        {
            var data = First(filter, order);
            Exporter(data, rule);
            return data;
        }

        public TEntity First(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, bool isDescending, Func<Rule<TEntity>, bool> rule)
        {
            var data = First(filter, order, isDescending);
            Exporter(data, rule);
            return data;
        }

        public TEntity Get(string id, Func<Rule<TEntity>, bool> rule)
        {
            var data = Get(id);
            Exporter(data, rule);
            return data;
        }

        public TEntity Last(Func<Rule<TEntity>, bool> rule)
        {
            var data = Last();
            Exporter(data, rule);
            return data;
        }

        public TEntity Last(Expression<Func<TEntity, bool>> filter, Func<Rule<TEntity>, bool> rule)
        {
            var data = Last(filter);
            Exporter(data, rule);
            return data;
        }

        public TEntity Last(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, Func<Rule<TEntity>, bool> rule)
        {
            var data = Last(filter, order);
            Exporter(data, rule);
            return data;
        }

        public TEntity Last(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, bool isDescending, Func<Rule<TEntity>, bool> rule)
        {
            var data = Last(filter, order, isDescending);
            Exporter(data, rule);
            return data;
        }
        #endregion
        private void Importer(TEntity entity, Func<Rule<TEntity>, bool> rule)
        {
            var filters = _mapper.GetFilters(rule);

            if (filters?.Count() > 0)
                foreach (var filter in filters)
                {
                    var primaryKeyValue = entity.GetPropertyValue<string>(filter.PrimaryKey);

                    var relatedField = entity.GetPropertyValue(filter.LocalFieldName);

                    SetValueFromDynamicRepository(filter, primaryKeyValue, relatedField);






                }
        }
        private void Importer(IEnumerable<TEntity> entities, Func<Rule<TEntity>, bool> rule)
        {
            foreach (var entity in entities)
                Importer(entity, rule);
        }
        private void Exporter(IEnumerable<TEntity> entities, Func<Rule<TEntity>, bool> rule)
        {
            foreach (var item in entities)
                Exporter(item, rule);
        }
        private void Exporter(TEntity entity, Func<Rule<TEntity>, bool> rule)
        {
            var filters = _mapper.GetFilters(rule);

            if (filters?.Count() > 0)
                foreach (var filter in filters)
                {
                    var primaryKeyValue = entity.GetPropertyValue<string>(filter.PrimaryKey);
                    object value = GetValueFromDynamicRepository(filter, primaryKeyValue);
                    entity.SetPropertyValue(filter.LocalFieldName, value);
                }
        }
        private void SetValueFromDynamicRepository(Rule<TEntity> filter, string relationValue, object relatedField)
        {
            var repository = filter.TargetType.GetRepositoryInstanceFromType(_context);
            MethodInfo method = null;
            object value = null;
            switch (filter.RelationType)
            {
                case RelationType.One:
                    method = repository.GetInsertMethod();
                    relatedField.SetPropertyValue(filter.TargetKey, relationValue);
                    value = relatedField;
                    break;
                case RelationType.Collection:
                    var listedData = (IEnumerable<object>)relatedField;
                    foreach (var row in listedData)
                        row.SetPropertyValue(filter.TargetKey, relationValue);
                    method = repository.GetInsertManyMethod();
                    value = listedData;
                    break;
            }
            method.Invoke(repository, new object[] { value });

        }
        private object GetValueFromDynamicRepository(Rule<TEntity> filter, string filterValue)
        {
            var repository = filter.TargetType.GetRepositoryInstanceFromType(_context);
            object value = null;
            MethodInfo method = null;
            var expression = filter.TargetType.GetExpressionEqual(filter.TargetKey, filterValue);
            switch (filter.RelationType)
            {
                case RelationType.One:
                    method = repository.GetFilterMethod("First");
                    value = method.Invoke(repository, new object[] { expression });
                    break;
                case RelationType.Collection:
                    method = repository.GetFilterMethod("Find");
                    value = ((IEnumerable<object>)method.Invoke(repository, new object[] { expression })).ToList().ObjectListToSpecificTypeList(filter.TargetType);
                    break;
            }
            return value;
        }
    }
    public class MongoRepositoryBase<TEntity> : IMongoRepository<TEntity>
     where TEntity : IMongoEntity
    {
        public MongoRepositoryBase()
        {
            //if (Collection == null)
            //    throw new ArgumentNullException(nameof(Collection));
        }
        public MongoRepositoryBase(MongoConnection mongoConnection)
        {
            Collection = Database<TEntity>.GetCollectionFromClientConnection(mongoConnection);
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
        public bool Delete(string id) => Collection.DeleteOne(i => i.Id == id).IsAcknowledged;

        /// <summary>
        /// delete by id
        /// </summary>
        /// <param name="id">id</param>
        public async Task<bool> DeleteAsync(string id)
        {
            return await Task.Run(() =>
            {
                return Delete(id);
            });
        }

        /// <summary>
        /// delete items with filter
        /// </summary>
        /// <param name="filter">expression filter</param>
        public bool Delete(Expression<Func<TEntity, bool>> filter) => Collection.DeleteMany(filter).IsAcknowledged;

        /// <summary>
        /// delete items with filter
        /// </summary>
        /// <param name="filter">expression filter</param>
        public async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await Task.Run(() =>
            {
                return Delete(filter);
            });
        }

        /// <summary>
        /// delete all documents
        /// </summary>
        public bool DeleteAll() => Collection.DeleteMany(Filter.Empty).IsAcknowledged;

        /// <summary>
        /// delete all documents
        /// </summary>
        public async Task<bool> DeleteAllAsync()
        {
            return await Task.Run(() =>
            {
                return DeleteAll();
            });
        }
        #endregion Delete

        #region Find

        /// <summary>
        /// find entities
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>collection of entity</returns>
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter) => Query(filter).ToEnumerable();

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
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, int pageIndex, int size, bool isDescending)
        {
            var query = Query(filter).Skip(pageIndex * size).Limit(size);
            return (isDescending ? query.SortByDescending(order) : query.SortBy(order)).ToEnumerable();
        }

        #endregion Find

        #region FindAll

        /// <summary>
        /// fetch all items in collection
        /// </summary>
        /// <returns>collection of entity</returns>
        public IEnumerable<TEntity> FindAll() => Query().ToEnumerable();

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
        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, object>> order, int pageIndex, int size, bool isDescending)
        {
            var query = Query().Skip(pageIndex * size).Limit(size);
            return (isDescending ? query.SortByDescending(order) : query.SortBy(order)).ToEnumerable();
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

        #region Last

        /// <summary>
        /// get first item in collection
        /// </summary>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public TEntity Last() => FindAll(i => i.Id, 0, 1, true).FirstOrDefault();

        /// <summary>
        /// get last item in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public TEntity Last(Expression<Func<TEntity, bool>> filter) => Last(filter, i => i.Id);

        /// <summary>
        /// get last item in query with order
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public TEntity Last(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order) => Last(filter, order, false);

        /// <summary>
        /// get last item in query with order and direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public TEntity Last(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, bool isDescending) => First(filter, order, !isDescending);

        #endregion Last

        #region Get

        /// <summary>
        /// get by id
        /// </summary>
        /// <param name="id">id value</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public TEntity Get(string id) => Find(i => i.Id == id).FirstOrDefault();

        #endregion Get

        #region Insert

        /// <summary>
        /// insert entity
        /// </summary>
        /// <param name="entity">entity</param>
        public void Insert(TEntity entity) => Collection.InsertOne(entity);

        /// <summary>
        /// insert entity
        /// </summary>
        /// <param name="entity">entity</param>
        public async Task InsertAsync(TEntity entity) => await Collection.InsertOneAsync(entity);

        /// <summary>
        /// insert entity collection
        /// </summary>
        /// <param name="entities">collection of entities</param>
        public void Insert(IEnumerable<TEntity> entities) => Collection.InsertMany(entities);

        /// <summary>
        /// insert entity collection
        /// </summary>
        /// <param name="entities">collection of entities</param>
        public async Task InsertAsync(IEnumerable<TEntity> entities) => await Collection.InsertManyAsync(entities);
        #endregion Insert

        #region Replace

        /// <summary>
        /// replace an existing entity
        /// </summary>
        /// <param name="entity">entity</param>
        public bool Replace(TEntity entity) => Collection.ReplaceOne(i => i.Id == entity.Id, entity).IsAcknowledged;

        /// <summary>
        /// replace an existing entity
        /// </summary>
        /// <param name="entity">entity</param>
        public async Task<bool> ReplaceAsync(TEntity entity)
        {
            return await Task.Run(() =>
            {
                return Replace(entity);
            });
        }

        /// <summary>
        /// replace collection of entities
        /// </summary>
        /// <param name="entities">collection of entities</param>
        public void Replace(IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
                Replace(entity);
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
        public bool Update(string id, params UpdateDefinition<TEntity>[] updates)
        {
            return Update(Filter.Eq(i => i.Id, id), updates);
        }

        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="updates">updated field(s)</param>
        public Task<bool> UpdateAsync(string id, params UpdateDefinition<TEntity>[] updates)
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
        public bool Update(TEntity entity, params UpdateDefinition<TEntity>[] updates)
        {
            return Update(entity.Id, updates);
        }

        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="updates">updated field(s)</param>
        public Task<bool> UpdateAsync(TEntity entity, params UpdateDefinition<TEntity>[] updates)
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
            var update = Updater.Combine(updates);//.CurrentDate(i => i.UpdateAt);
            return Collection.UpdateMany(filter, update/*.CurrentDate(i => i.UpdateAt)*/).IsAcknowledged;
        }

        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        public async Task<bool> UpdateAsync(FilterDefinition<TEntity> filter, params UpdateDefinition<TEntity>[] updates)
        {
            return await Task.Run(() =>
            {
                return Update(filter, updates);
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
            var update = Updater.Combine(updates);//.CurrentDate(i => i.UpdateAt);
            return Collection.UpdateMany(filter, update).IsAcknowledged;
        }

        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        public async Task<bool> UpdateAsync(Expression<Func<TEntity, bool>> filter, params UpdateDefinition<TEntity>[] updates)
        {
            return await Task.Run(() =>
            {
                return Update(filter, updates);
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
        public bool Any(Expression<Func<TEntity, bool>> filter) => First(filter) != null;

        #region Count
        /// <summary>
        /// get number of filtered documents
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>number of documents</returns>
        public long Count(Expression<Func<TEntity, bool>> filter) => Collection.Count(filter);

        /// <summary>
        /// get number of filtered documents
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>number of documents</returns>
        public async Task<long> CountAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await Collection.CountAsync(filter);
        }

        /// <summary>
        /// get number of documents in collection
        /// </summary>
        /// <returns>number of documents</returns>
        public long Count() => Collection.Count(Filter.Empty);

        /// <summary>
        /// get number of documents in collection
        /// </summary>
        /// <returns>number of documents</returns>
        public async Task<long> CountAsync() => await Collection.CountAsync(Filter.Empty);
        #endregion Count

        #endregion Utils

    }
}
