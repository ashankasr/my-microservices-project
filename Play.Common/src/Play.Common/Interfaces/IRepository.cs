using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Play.Common.Entities;

namespace Play.Common.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyCollection<T>> GetAllAsync();

        Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter);

        Task<T> GetAsync(Guid id);

        Task<T> GetAsync(Expression<Func<T, bool>> filter);

        Task CreateAsync(T item);

        Task UpdateAsync(T item);

        Task DeleteAsync(Guid id);
    }
}