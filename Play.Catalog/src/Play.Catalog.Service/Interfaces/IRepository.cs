using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        public Task<IReadOnlyCollection<T>> GetAllAsync();

        public Task<T> GetAsync(Guid id);

        public Task CreateAsync(T item);

        public Task UpdateAsync(T item);

        public Task DeleteAsync(Guid id);
    }
}