using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TStack.MongoDB.Connection;
using TStack.MongoDB.Entity;
using TStack.MongoDB.Map;

namespace TStack.MongoDB
{
    public static class MongoIncluder
    {
        public static void Include<IMapper,TEntity>(this IMongoEntity entity, Func<Rule<TEntity>, bool> predicate)
            where TEntity : IMongoEntity
        {
            //var mapper = entity.G
        }
        public static void Include<TEntity>(this IMongoEntity entity, Func<Rule<TEntity>, bool> predicate)
          where TEntity : IMongoEntity
        {
            //var mapper = entity.G
        }

        //public static void Include<IMapper, TEntity>(this IMongoEntity entity, Func<Rule<TEntity>, bool> predicate)
        //    where TEntity : IMongoEntity
        //    where IMapper : Mapper<TEntity>, new()
        //{
        //    var mapper = new IMapper();

        //    var filters = mapper.Rules.RuleCollection.Where(predicate);

        //    if (filters.Count() > 0)
        //    {
        //        foreach (var rule in filters)
        //        {
        //            var collection = Database<TEntity>.GetCollectionFromMongoConnection(mapper.Context);
        //            var value = collection.FindAll();
        //            entity.GetType().GetProperty(rule.LocalFieldName).SetValue(entity, value.ToList());
        //        }
        //    }
        //}
        private static IEnumerable<T> FindAll<T>(this IMongoCollection<T> collection)
            where T : IMongoEntity

        {
            var fluent = collection.Find(Builders<T>.Filter.Empty);
            return fluent.ToEnumerable();

        }
    }
}
