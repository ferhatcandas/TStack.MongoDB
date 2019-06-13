using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using TStack.MongoDB.Connection;
using TStack.MongoDB.Entity;

namespace TStack.MongoDB.Map
{
    public class Mapper<T>
        where T : IMongoEntity
    {
        public Mapper(MongoConnection mongoConnection)
        {
            Context = mongoConnection;
            Rules = new Rules<T>();
        }
        internal MongoConnection Context { get; }
        public Rules<T> Rules { get; set; }
    }
    public class Rules<T> 
        where T : IMongoEntity
    {
        public Rules()
        {
            RuleCollection = new List<Rule<T>>();
        }
        public List<Rule<T>> RuleCollection { get; set; }
    }
    public class Rule<T> 
        where T : IMongoEntity
    {
        public Rule(string ruleName, string primaryKey, string localFieldName, string relationKey)
        {
            RuleName = ruleName;
            PrimaryKey = primaryKey;
            LocalFieldName = localFieldName;
            RelationKey = relationKey;
        }
        internal string PrimaryKey { get; set; }
        public string RuleName { get; set; }
        internal string LocalFieldName { get; set; }
        internal string RelationKey { get; set; }

    }

}
