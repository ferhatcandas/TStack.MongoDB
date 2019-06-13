using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TStack.MongoDB.Connection;
using TStack.MongoDB.Entity;

namespace TStack.MongoDB.Map
{
    public class Mapper<TEntity>
        where TEntity : IMongoEntity
    {
        public Mapper()
        {
            Rules = new List<Rule<TEntity>>();
        }
        internal List<Rule<TEntity>> Rules { get; set; }
        public void AddRule(Rule<TEntity> rule)
        {
            RuleIsValid(rule);
            Rules.Add(rule);
        }
        private void RuleIsValid(Rule<TEntity> rule)
        {
            if (string.IsNullOrEmpty(rule.TargetKey))
                throw new ArgumentNullException(rule.TargetKey);
            if (string.IsNullOrEmpty(rule.PrimaryKey))
                throw new ArgumentNullException(rule.PrimaryKey);
            var type = typeof(TEntity);
            if (type.GetProperty(rule.PrimaryKey) == null)
                throw new ArgumentException($"{type.Name} primary key is not found");
        }
    }
    public class Rule<TEntity>
        where TEntity : IMongoEntity
    {
        public string RuleName { get; private set; }
        internal string PrimaryKey { get; set; } = "Id";
        internal string LocalFieldName { get; set; }
        internal string TargetKey { get; set; }
        internal Type TargetType { get; set; }

        internal RelationType RelationType { get; set; } = RelationType.None;
        public Rule<TEntity> Name(string ruleName)
        {
            RuleName = ruleName;
            return this;
        }
        public Rule<TEntity> Key(string primaryKey)
        {
            PrimaryKey = primaryKey;
            return this;
        }
        public Rule<TEntity> RelationKey(string relationKey)
        {
            if (string.IsNullOrEmpty(relationKey))
                throw new ArgumentNullException(nameof(relationKey));
            TargetKey = relationKey;
            return this;
        }
        public Rule<TEntity> WithOne<TField>(Expression<Func<TEntity, TField>> expression)
            where TField : IMongoEntity
        {
            if (RelationType != RelationType.None)
                throw new ArgumentException("One rule must had one relation with model or collection");
            RelationType = RelationType.One;
            LocalFieldName = (expression.Body as MemberExpression).Member.Name;
            TargetType = typeof(TField);
            return this;

        }
        public Rule<TEntity> WithCollection<TField>(Expression<Func<TEntity, List<TField>>> expression)
            where TField : IMongoEntity
        {
            if (RelationType != RelationType.None)
                throw new ArgumentException("One rule must had one relation with model or collection");
            RelationType = RelationType.Collection;
            LocalFieldName = (expression.Body as MemberExpression).Member.Name;
            TargetType = typeof(TField);
            return this;
        }
    }
    internal enum RelationType
    {
        None,
        One,
        Collection
    }
}
