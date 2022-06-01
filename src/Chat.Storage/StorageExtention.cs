using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Storage
{
    public static class StorageExtention
    {
        //public static  Task<List<T>> ToListReadUncommittedAsync<T>(this IQueryable<T> query, DbContext context, CancellationToken cancellationToken)
        
        //    return  query. .ToList().PipeTo(Task.FromResult);
        //    //using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadUncommitted, cancellationToken);
        //    //var result = await query.ToListAsync(cancellationToken);
        //    //transaction.Commit();
        //    //return result;
        //}
    }
}
