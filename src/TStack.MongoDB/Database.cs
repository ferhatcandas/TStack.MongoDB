using MongoDB.Driver;
using System;
using System.Reflection;
using TStack.MongoDB.Connection;
using TStack.MongoDB.Entity;

namespace TStack.MongoDB
{
    /// <typeparam name="T">The type to get the collection of.</typeparam>
    public class Database<T> 
    {
        public IMongoCollection<T> GetCollection(MongoConnection mongoConnection)
        {
            var client = GetClient(mongoConnection);

            return GetDataBaseFromClient(client, mongoConnection.Database).GetCollection<T>(GetCollectionName());
        }
        public static IMongoCollection<T> GetCollectionFromMongoConnection(MongoConnection mongoConnection)
        {
            var client = GetClient(mongoConnection);

            return GetDataBaseFromClient(client, mongoConnection.Database).GetCollection<T>(GetCollectionName());
        }
        private static IMongoDatabase GetDataBaseFromClient(IMongoClient mongoClient, string databaseName)
        {
            return mongoClient.GetDatabase(databaseName);
        }
        internal static MongoClient GetClient(MongoConnection mongoConnection)
        {
            return new MongoClient(GetSettings(mongoConnection));
        }
        private static MongoClientSettings GetSettings(MongoConnection mongo)
        {
            MongoCredential mongoCredential = null;
            if (!string.IsNullOrEmpty(mongo.Username))
                mongoCredential = MongoCredential.CreateCredential(mongo.Database, mongo.Username, mongo.Password);
            return new MongoClientSettings()
            {
                Server = new MongoServerAddress(mongo.Host, mongo.Port),
                SocketTimeout = mongo.SocketTimeout,
                ConnectTimeout = mongo.ConnectTimeout,
                ServerSelectionTimeout = mongo.ServerSelectionTimeout,
                Credential = mongoCredential
            };
        }
        #region Collection Name

        /// <summary>
        /// Determines the collection name for T and assures it is not empty
        /// </summary>
        /// <returns>Returns the collection name for T.</returns>
        private static string GetCollectionName()
        {
            string collectionName;
            collectionName = typeof(T).GetTypeInfo().BaseType.Equals(typeof(object)) ?
                                      GetCollectionNameFromInterface() :
                                      GetCollectionNameFromType();

            if (string.IsNullOrEmpty(collectionName))
            {
                collectionName = typeof(T).Name;
            }
            return collectionName.ToLowerInvariant();
        }

        /// <summary>
        /// Determines the collection name from the specified type.
        /// </summary>
        /// <returns>Returns the collection name from the specified type.</returns>
        private static string GetCollectionNameFromInterface()
        {
            // Check to see if the object (inherited from Entity) has a CollectionName attribute
            var att = CustomAttributeExtensions.GetCustomAttribute<CollectionNameAttribute>(typeof(T).GetTypeInfo());
            return att?.Name ?? typeof(T).Name;
        }

        /// <summary>
        /// Determines the collectionname from the specified type.
        /// </summary>
        /// <returns>Returns the collectionname from the specified type.</returns>
        private static string GetCollectionNameFromType()
        {
            Type entitytype = typeof(T);
            string collectionname;

            // Check to see if the object (inherited from Entity) has a CollectionName attribute
            var att = CustomAttributeExtensions.GetCustomAttribute<CollectionNameAttribute>(typeof(T).GetTypeInfo());
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
