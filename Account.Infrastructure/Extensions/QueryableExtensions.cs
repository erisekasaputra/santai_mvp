using System.Linq.Expressions;

namespace Account.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> OrWhere<T>(
        this IQueryable<T> source,
        Expression<Func<T, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        // Get the current query expression
        var existingExpression = source.Expression;

        // Extract the existing 'Where' clause if present
        if (existingExpression is MethodCallExpression existingPredicate && existingPredicate.Method.Name == "Where")
        {
            if (existingPredicate.Arguments[1] is LambdaExpression existingLambda)
            {
                // Combine the existing predicate with the new predicate using OR
                var combinedPredicate = Expression.OrElse(
                    existingLambda.Body,
                    predicate.Body
                );

                var combinedLambda = Expression.Lambda<Func<T, bool>>(combinedPredicate, existingLambda.Parameters);
                var newExpression = Expression.Call(
                    typeof(Queryable),
                    "Where",
                    new Type[] { typeof(T) },
                    existingExpression,
                    combinedLambda
                );

                return source.Provider.CreateQuery<T>(newExpression);
            }
        }

        // No existing 'Where' clause, just apply the predicate
        return source.Where(predicate);
    }
}