using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        internal static T GetPropertyValue<T>(this object Item, string propertyName)
        {
            return Parse<T>(Item.GetProperty(propertyName).GetValue(Item));
        }
        internal static object GetPropertyValue(this object Item, string propertyName)
        {
            return Item.GetProperty(propertyName).GetValue(Item);
        }
        internal static PropertyInfo GetProperty(this object Item, string propertyName)
        {
            return Item.GetType().GetProperties().First(x => x.Name == propertyName);
        }
        internal static object GetNestedPropertyValue(this object Item, string propertyName)
        {
            var newItem = Item.GetProperty(propertyName);

            return Item.GetProperty(propertyName).GetValue(newItem);
        }
        internal static object GetRepositoryInstanceFromType(this Type type, MongoConnection mongoConnection)
        {
            var repoType = typeof(MongoRepositoryBase<>).MakeGenericType(type);
            var ctor = repoType.GetConstructors().FirstOrDefault(x =>
            x.GetParameters().Length == 1 &&
            x.GetParameters().FirstOrDefault(y => y.ParameterType == typeof(MongoConnection)).Name == "mongoConnection"
            );
            return ctor.Invoke(new object[] { mongoConnection });
        }
        internal static void SetPropertyValue(this object Item, string propertyName, object value)
        {
            var field = Item.GetProperty(propertyName);
            field.SetValue(Item, value);
        }

        internal static object GetExpressionEqual(this Type type, string targetKey, string targetValue)
        {
            var body = Expression.Constant(targetValue);
            var parameter = Expression.Parameter(type);
            var property = Expression.Property(parameter, targetKey);
            var expression = Expression.Equal(property, body);
            var delegateType = typeof(Func<,>).MakeGenericType(type, typeof(bool));
            var lambda = Expression.Lambda(delegateType, expression, parameter);
            return lambda;
        }
        internal static object GetExpressionBind(this Type type, string targetKey, string targetValue)
        {
            var body = Expression.Constant(targetValue);
            var parameter = Expression.Parameter(type);
            var property = Expression.Property(parameter, targetKey);
            var expression = Expression.Bind(property.Member, body);

            var delegateType = typeof(Func<,>).MakeGenericType(type, typeof(object));

            var lamda = Expression.Lambda(delegateType, expression.Expression);

            //var lambda = Expression.Lambda(expression);
            return null;
        }
        internal static object ObjectListToSpecificTypeList(this List<object> list, Type targetType)
        {
            var genricTypedList = Activator.CreateInstance(typeof(List<>).MakeGenericType(targetType));
            foreach (var item in list)
            {
                var itemValue = Convert.ChangeType(item, targetType);
                genricTypedList.GetType().GetMethod("Add").Invoke(genricTypedList, new object[] { itemValue });
            }
            return genricTypedList;
        }
        internal static string GetMemberName<T, T2>(this Expression<Func<T, T2>> expression)
        {
            return (expression.Body as MemberExpression).Member.Name;
        }
        internal static T Parse<T>(object value)
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        internal static MethodInfo GetFilterMethod(this object Item, string methodName)
        {
            return Item.GetMethods().
                Where(x =>
                x.Name == methodName &&
                x.GetParameters().Count() == 1 &&
                x.GetParameters().FirstOrDefault(y => y.Name == "filter") != null
                ).First();
        }
        internal static MethodInfo GetInsertMethod(this object Item)
        {
            return Item.GetMethods().
             FirstOrDefault(
             x => x.Name == "Insert" &&
             x.GetParameters().FirstOrDefault(y => y.Name == "entity") != null);
        }
        internal static MethodInfo GetInsertManyMethod(this object Item)
        {
            return Item.GetMethods().
                FirstOrDefault(
                 x => x.Name == "Insert" &&
                 x.GetParameters().FirstOrDefault(y => y.Name == "entities") != null
                 );
        }
        private static MethodInfo[] GetMethods(this object Item) => Item.GetType().GetMethods();

    }
}
