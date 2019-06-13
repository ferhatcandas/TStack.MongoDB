using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using TStack.MongoDB.Entity;
using TStack.MongoDB.Map;

namespace TStack.MongoDB.Repository
{
    public static class MongoRelationalRepository
    {

        //public static void Include<TMapper>(this IMongoEntity entity)
        //where TMapper : MongoMapper<MongoEntity, MongoEntity>, new()
        //{

        //    var mapper = new TMapper();
        //    var property = mapper.Local.GetProperty(mapper.LocalKey);
        //    if (CheckFieldTypeIsValid(property, mapper.ReturnType))
        //    {

        //    }
        //    throw new NotImplementedException();
        //}
        //public static void Includes<TEntity, TMapper>(this IMongoRepository<TEntity> repository)
        //    where TEntity : IMongoEntity, new()
        //    where TMapper : MongoMapper<IMongoEntity, IMongoEntity>, new()
        //{
        //    var mapper = new TMapper();

        //}
        //public static void Include<TMapper>(this IMongoEntity entity, TMapper mapper)
        //    where TMapper : MongoMapper<IMongoEntity, IMongoEntity>, new()
        //{


        //    var property = mapper.Local.GetProperty(mapper.LocalKey);
        //    if (CheckFieldTypeIsValid(property, mapper.ReturnType))
        //    {

        //    }
        //    throw new NotImplementedException();
        //}

        private static IMongoCollection<IMongoEntity> GetCollection(PropertyInfo property, IMongoEntity targetEntity, string targetFieldName)
        {



            throw new NotImplementedException();
        }
        public static PropertyInfo GetProperty<TEntity>(this TEntity entity, string fieldName)
            where TEntity : IMongoEntity
        {
            var property = entity.GetType().GetProperty(fieldName);
            if (property == null)
                throw new ArgumentException("Property not found", nameof(fieldName));
            return property;
        }
        private static bool CheckFieldTypeIsValid(this PropertyInfo property, RelationModelType type)
        {
            bool status = false;
            switch (type)
            {
                case RelationModelType.Class:
                    if (property.PropertyType.IsClass)
                        status = true;
                    break;
                case RelationModelType.Array:
                    if (property.PropertyType.IsArray)
                        status = true;
                    break;
                default:
                    break;
            }
            return status;
        }
        private static Type GetFieldType<TEntity>(this TEntity entity, string fieldName)
            where TEntity : IMongoEntity, new()
        {
            var property = entity.GetType().GetProperty(fieldName);
            if (property == null)
                throw new ArgumentException("Property not found", nameof(fieldName));

            var type = property.GetType();
            var value = property.GetValue(property);

            return type;
        }
        #region Collection Name

        /// <summary>
        /// Determines the collection name for T and assures it is not empty
        /// </summary>
        /// <returns>Returns the collection name for T.</returns>
        private static string GetCollectionName<TEntity>(this TEntity entity)
            where TEntity : IMongoEntity
        {
            string collectionName;
            collectionName = typeof(TEntity).GetTypeInfo().BaseType.Equals(typeof(object)) ?
                                      GetCollectionNameFromInterface(entity) :
                                      GetCollectionNameFromType(entity);

            if (string.IsNullOrEmpty(collectionName))
            {
                collectionName = typeof(TEntity).Name;
            }
            return collectionName.ToLowerInvariant();
        }

        /// <summary>
        /// Determines the collection name from the specified type.
        /// </summary>
        /// <returns>Returns the collection name from the specified type.</returns>
        private static string GetCollectionNameFromInterface<TEntity>(this TEntity entity)
            where TEntity : IMongoEntity
        {
            // Check to see if the object (inherited from Entity) has a CollectionName attribute
            var att = CustomAttributeExtensions.GetCustomAttribute<CollectionNameAttribute>(typeof(TEntity).GetTypeInfo());
            return att?.Name ?? typeof(TEntity).Name;
        }

        /// <summary>
        /// Determines the collectionname from the specified type.
        /// </summary>
        /// <returns>Returns the collectionname from the specified type.</returns>
        private static string GetCollectionNameFromType<TEntity>(this TEntity entity)
            where TEntity : IMongoEntity
        {
            Type entitytype = typeof(TEntity);
            string collectionname;

            // Check to see if the object (inherited from Entity) has a CollectionName attribute
            var att = CustomAttributeExtensions.GetCustomAttribute<CollectionNameAttribute>(typeof(TEntity).GetTypeInfo());
            if (att != null)
            {
                // It does! Return the value specified by the CollectionName attribute
                collectionname = att.Name;
            }
            else
            {
                collectionname = entitytype.Name;
            }

            return collectionname;
        }

        #endregion Collection Name

    }
}
