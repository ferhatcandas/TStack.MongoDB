using System;
using System.Collections.Generic;
using System.Text;
using TStack.MongoDB.Entity;

namespace TStack.MongoDB.Map
{
    public class MongoMapper<LocalEntity, TargetEntity>
        where LocalEntity : IMongoEntity
        where TargetEntity : IMongoEntity
    {
        public LocalEntity Local { get; set; }
        public TargetEntity Target { get; set; }
        public string LocalKey { get; set; }
        public string TargetKey { get; set; }
        public RelationModelType ReturnType { get; set; }
    }
}
