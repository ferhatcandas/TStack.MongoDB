using System;
using System.Collections.Generic;
using System.Text;
using TStack.MongoDB.Entity;

namespace TStack.MongoDB.Map
{
    public class Mapper
    {
        public Mapper(string primaryKey)
        {
            PrimaryKey = primaryKey;
            Relations = new List<MapCollection>();
        }
        public string PrimaryKey { get; set; }
        public List<MapCollection> Relations { get; set; }

    }
    public class MapCollection
    {
        public string LocalFieldName { get; set; }
        public Type TargetType { get; set; }
        public string RelationKey { get; set; }
        public RelationModelType ReturnType { get; set; }
    }
}
