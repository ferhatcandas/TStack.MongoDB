using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using TStack.MongoDB.Map;

namespace TStack.MongoDB.Entity
{
    ///// <summary>
    ///// Entity wrapper for non-edittable models
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //public class MongoEntity<T>
    //{
    //}
    public class MongoEntity : IMongoEntity
    {
        /// <summary>
        /// id in string format
        /// </summary>
        [BsonElement(Order = 0)]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// id in objectId format
        /// </summary>
        public ObjectId ObjectId => ObjectId.Parse(Id);
    }
}
