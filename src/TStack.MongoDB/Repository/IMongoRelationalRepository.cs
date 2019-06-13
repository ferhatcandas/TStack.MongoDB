using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using TStack.MongoDB.Entity;
using TStack.MongoDB.Map;

namespace TStack.MongoDB.Repository
{
    public interface IMongoRelationalRepository<TEntity, TMapper> : IMongoRepository<TEntity>
        where TEntity : IMongoEntity, new()
        where TMapper : MongoMapper<TEntity>, new()
    {
        TEntity Include(TEntity entity, Expression<Func<TEntity, bool>> predicate);
    }
}
