using System.Linq.Expressions;

public static class IQueryableExtensions
{
    public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string sortColumn, bool ascending)
    {
        if (string.IsNullOrEmpty(sortColumn))
            return source;

        var parameter = Expression.Parameter(typeof(T), "x");
        var selector = Expression.PropertyOrField(parameter, sortColumn);
        var method = ascending ? "OrderBy" : "OrderByDescending";

        var expression = Expression.Lambda(selector, parameter);
        var resultExpression = Expression.Call(
            typeof(Queryable),
            method,
            new Type[] { typeof(T), selector.Type },
            source.Expression,
            Expression.Quote(expression));

        return source.Provider.CreateQuery<T>(resultExpression);
    }
}
