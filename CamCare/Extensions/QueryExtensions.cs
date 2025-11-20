using Radzen;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace CamCare.Extensions
{
    public static class QueryExtensions
    {
        public static (int TotalCount, IQueryable<T> query) LoadByLoadDataArgs<T>(this IQueryable<T> query, LoadDataArgs args, Expression<Func<T, bool>>? predicate = null)
        {
            // Sorting
            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                query = query.OrderBy(args.OrderBy);
            }

            // Filtering
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            else if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(args.Filter);
            }

            var totalCount = query.Count();

            // Paging
            if (args.Skip.HasValue)
                query = query.Skip(args.Skip.Value);

            if (args.Top.HasValue)
                query = query.Take(args.Top.Value);

            return (totalCount, query);
        }

    }
}
