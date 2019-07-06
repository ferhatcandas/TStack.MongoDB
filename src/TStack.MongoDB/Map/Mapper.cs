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
        /// create new rule
        /// </summary>
        /// <returns></returns>
        public Mapper<TEntity> Rule()
        {
            _rule = new Rule<TEntity>();
            return this;
        }
        /// <summary>
        /// ruleName for easyly access this rule
        /// </summary>
        /// <param name="ruleName"></param>
        /// <returns></returns>
        public Mapper<TEntity> Name(string ruleName)
        {
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
        {
            if (string.IsNullOrEmpty(_rule.PrimaryKey))
            {
                var fieldName = expression.GetMemberName();
                _rule.Key(fieldName);
            }
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
                _rule.Name(primaryKey);
            return this;
        }
        /// <summary>
        /// selected object's relationKey(propertyName)
        /// </summary>
        /// <param name="relationKey"></param>
        /// <returns></returns>
        public Mapper<TEntity> RelationKey(string relationKey)
        {
            _rule.RelationKey(relationKey);
            return this;
        }
        /// <summary>
        /// make relation one with selected object
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="expression"></param>
        public void WithOne<TField>(Expression<Func<TEntity, TField>> expression)
            where TField : IMongoEntity
        {
            _rule.WithOne(expression);
            AddRule();
        }
        /// <summary>
        /// make relation many with selected object
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="expression"></param>
        public void WithCollection<TField>(Expression<Func<TEntity, List<TField>>> expression)
            where TField : IMongoEntity
        {
            _rule.WithCollection(expression);
            AddRule();
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
