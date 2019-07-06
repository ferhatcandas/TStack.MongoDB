using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using TStack.MongoDB.Entity;
using TStack.MongoDB;


namespace TStack.MongoDB.Map
{
    public class Rule<TEntity>
           where TEntity : IMongoEntity
    {
        internal string RuleName { get; private set; }
        internal string PrimaryKey { get; set; } = "Id";
        internal string LocalFieldName { get; set; }
        internal string TargetKey { get; set; }
        internal Type TargetType { get; set; }
        internal RelationType RelationType { get; set; } = RelationType.None;
        internal Rule<TEntity> Name(string ruleName)
        {
            RuleName = ruleName;
            return this;
        }
        /// <summary>
        /// currentObject key parameter
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        internal Rule<TEntity> Key(string primaryKey)
        {
            PrimaryKey = primaryKey;
            return this;
        }
        /// <summary>
        /// selected object's relationKey(propertyName)
        /// </summary>
        /// <param name="relationKey"></param>
        /// <returns></returns>
        internal Rule<TEntity> RelationKey(string relationKey)
        {
            if (string.IsNullOrEmpty(relationKey))
                throw new ArgumentNullException(nameof(relationKey));
            TargetKey = relationKey;
            return this;
        }
        /// <summary>
        /// make relation one with selected object
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="expression"></param>
        internal Rule<TEntity> WithOne<TField>(Expression<Func<TEntity, TField>> expression)
            where TField : IMongoEntity
        {
            if (RelationType != RelationType.None)
                throw new ArgumentException("One rule must had one relation with model or collection");
            RelationType = RelationType.One;
            LocalFieldName = expression.GetMemberName();
            TargetType = typeof(TField);
            return this;

        }
        /// <summary>
        /// make relation many with selected object
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="expression"></param>
        internal Rule<TEntity> WithCollection<TField>(Expression<Func<TEntity, List<TField>>> expression)
            where TField : IMongoEntity
        {
            if (RelationType != RelationType.None)
                throw new ArgumentException("One rule must had one relation with model or collection");
            RelationType = RelationType.Collection;
            LocalFieldName = expression.GetMemberName();
            TargetType = typeof(TField);
            return this;
        }
        /// <summary>
        /// checks rule is valid
        /// </summary>
        internal void RuleIsValid()
        {
            if (string.IsNullOrEmpty(TargetKey))
                throw new ArgumentNullException(TargetKey);
            if (string.IsNullOrEmpty(PrimaryKey))
                throw new ArgumentNullException(PrimaryKey);
            var type = typeof(TEntity);
            if (type.GetProperty(PrimaryKey) == null)
                throw new ArgumentException($"{type.Name} primary key is not found");
        }
    }
}
