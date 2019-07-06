using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using TStack.MongoDB.Connection;
using TStack.MongoDB.Entity;
using TStack.MongoDB.Repository;

namespace TStack.MongoDB
{
    public static class MongoExtension
    {
        public static IServiceCollection UseMongoDB(this IServiceCollection services, MongoConnection mongoConnection)
        {
            services.AddSingleton<MongoConnection>(mongoConnection);
            return services;
        }
        internal static string GetPropertyValue(this object Item, string property)
        {
            return Item.GetType().GetProperties().First(x => x.Name == property).GetValue(Item).ToString();
        }
        internal static object GetRepositoryFromType(this Type type, MongoConnection mongoConnection)
        {
            var repoType = typeof(MongoRepositoryBase<>).MakeGenericType(type);
            var ctor = repoType.GetConstructors().FirstOrDefault(x =>
            x.GetParameters().Length == 1 &&
            x.GetParameters().FirstOrDefault(y => y.ParameterType == typeof(MongoConnection)).Name == "mongoConnection"
            );
            return ctor.Invoke(new object[] { mongoConnection });
        }
        internal static object ConvertList(this List<object> value, Type type)
        {
            var containedType = type.GenericTypeArguments.First();
            return value.Select(item => Convert.ChangeType(item, containedType)).ToList();
        }
        internal static void SetPropertyValue(this object Item, string property, object value)
        {
            var field = Item.GetType().GetProperty(property);
            field.SetValue(Item, value);
        }
        internal static MethodInfo GetFilterMethod(this object Item, string methodName)
        {
            return Item.GetType().GetMethods().
                Where(x =>
                x.Name == methodName &&
                x.GetParameters().Count() == 1 &&
                x.GetParameters().FirstOrDefault(y => y.Name == "filter") != null
                ).First();
        }
        internal static object GetExpression(this Type type, string targetKey, string targetValue)
        {
            var body = Expression.Constant(targetValue);
            var parameter = Expression.Parameter(type);
            var property = Expression.Property(parameter, targetKey);
            var expression = Expression.Equal(property, body);
            var delegateType = typeof(Func<,>).MakeGenericType(type, typeof(bool));
            var lambda = Expression.Lambda(delegateType, expression, parameter);
            return lambda;
        }
       internal static object  ObjectListToSpecificTypeList(this List<object> list,Type targetType)
        {
            var genricTypedList = Activator.CreateInstance(typeof(List<>).MakeGenericType(targetType));
            foreach (var item in list)
            {
                var itemValue = Convert.ChangeType(item, targetType);
                genricTypedList.GetType().GetMethod("Add").Invoke(genricTypedList, new object[] { itemValue });
            }
            return genricTypedList;
        }
        internal static string GetMemberName<T,T2>(this Expression<Func<T,T2>> expression)
        {
           return (expression.Body as MemberExpression).Member.Name;
        }
    }
}
