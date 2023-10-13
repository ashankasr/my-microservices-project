using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Play.Common.Entities;
using Play.Common.Interfaces;

namespace Play.Common.MongoDb
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IMongoCollection<T> dbCollection;
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

        public Repository(IMongoDatabase database, string collectionName)
        {
            dbCollection = database.GetCollection<T>(collectionName);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).ToListAsync();
        }

        public async Task<T> GetAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(r => r.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await dbCollection.InsertOneAsync(item);
        }

        public async Task UpdateAsync(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            FilterDefinition<T> filter = filterBuilder.Eq(r => r.Id, item.Id);

            await dbCollection.ReplaceOneAsync(filter, item);
        }

        public async Task DeleteAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(r => r.Id, id);

            await dbCollection.DeleteOneAsync(filter);
        }
    }
}