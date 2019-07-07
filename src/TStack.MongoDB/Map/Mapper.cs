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
        internal Rule<TEntity> _rule { get; set; }
        /// <summary>
        /// ruleName for easyly access this rule
        /// </summary>
        /// <param name="ruleName"></param>
        /// <returns></returns>
        public Mapper<TEntity> Rule(string ruleName)
        {
            if (Rules.Any(x => x.RuleName == ruleName))
                throw new ArgumentException($"{nameof(ruleName)} is exist on rule list");
            _rule = new Rule<TEntity>();
            _rule.Name(ruleName);
            return this;
        }
        /// <summary>
        /// currentObject key parameter
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Mapper<TEntity> Key<TField>(Expression<Func<TEntity, TField>> expression)
        //where TField : struct
        {
            if (string.IsNullOrEmpty(_rule.PrimaryKey))
                _rule.Key(expression.GetMemberName());
            return this;
        }
        /// <summary>
        /// currentObject key parameter
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public Mapper<TEntity> Key(string primaryKey)
        {
            if (string.IsNullOrEmpty(_rule.PrimaryKey))
                _rule.Key(primaryKey);
            return this;
        }
        /// <summary>
        /// selected object's relationKey(propertyName)
        /// </summary>
        /// <param name="relationKey"></param>
        /// <returns></returns>
        public void RelationKey(string relationKey)
        {
            _rule.RelationKey(relationKey);
            AddRule();
        }
        /// <summary>
        /// make relation one with selected object
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="expression"></param>
        public Mapper<TEntity> WithOne<TField>(Expression<Func<TEntity, TField>> expression)
            where TField : IMongoEntity
        {
            _rule.WithOne(expression);
            return this;
        }

        /// <summary>
        /// make relation one with selected object and sets relation key
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="expression"></param>
        public void WithOne<TField>(Expression<Func<TEntity, TField>> expression, Expression<Func<TField, object>> relationKey)
            where TField : IMongoEntity
        {
            _rule.WithOne(expression);
            _rule.RelationKey(relationKey.GetMemberName());
        }
        /// make relation many with selected object
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="expression"></param>
        public Mapper<TEntity> WithCollection<TField>(Expression<Func<TEntity, List<TField>>> expression)
            where TField : IMongoEntity
        {
            _rule.WithCollection(expression);
            return this;
        }
        /// make relation many with selected object and sets relation key
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="expression"></param>
        public void WithCollection<TField>(Expression<Func<TEntity, List<TField>>> expression, Expression<Func<TField, object>> relationKey)
            where TField : IMongoEntity
        {
            _rule.WithCollection(expression);
            _rule.RelationKey(relationKey.GetMemberName());
        }
        /// <summary>
        /// add rule to list if is valid
        /// </summary>
        private void AddRule()
        {
            _rule.RuleIsValid();
            Rules.Add(_rule);
        }
    }


}
