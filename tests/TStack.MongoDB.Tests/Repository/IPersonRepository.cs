using System;
using System.Collections.Generic;
using System.Text;
using TStack.MongoDB.Entity;
using TStack.MongoDB.Map;
using TStack.MongoDB.Repository;

namespace TStack.MongoDB.Tests.Repository
{
    public interface IPersonRepository<TEntity> : IMongoRepository<TEntity>
        where TEntity : IMongoEntity, new()
    {
    }
}
