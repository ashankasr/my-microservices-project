using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Interfaces
{
    public interface IItemsRepository
    {
        public Task<IReadOnlyCollection<Item>> GetAllAsync();

        public Task<Item> GetAsync(Guid id);

        public Task CreateAsync(Item item);

        public Task UpdateAsync(Item item);

        public Task DeleteAsync(Guid id);
    }
}