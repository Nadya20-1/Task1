using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Task1
{
    public static class Extension
    {
        public static IQueryable<T> WhereIfRightIsNotDefault<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate) where T : class
        {
            var body = predicate.Body as BinaryExpression;

            if (body == null) return source;
            var currentValue = Expression.Lambda(body.Right)
                   .Compile().DynamicInvoke();
            if (currentValue.ToString() == "") return source;

            var defaultValueForType = GetDefaultValueForType(body.Right.Type);
            var isEquals = EqualityComparer<object>.Default
              .Equals(currentValue, defaultValueForType);

            if (!isEquals)
            {
                source = source.Where(predicate);
                return source;
            }
            return source;
        }

        private static object GetDefaultValueForType(Type type)
        {
            var p = Expression.Convert(Expression.Default(type), typeof(object));
            return Expression.Lambda<Func<object>>(p).Compile()();
        }
    }
}
