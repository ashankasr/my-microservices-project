using System;
using System.Threading.Tasks;
using MassTransit;
using Play.Common.Interfaces;
using Play.Inventory.Contracts;
using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Exceptions;

namespace Play.Inventory.Service.Consumers
{
    public class GrantItemsConsumer : IConsumer<GrantItems>
    {
        private readonly IRepository<InventoryItem> itemsRepository;
        private readonly IRepository<CatalogItem> catalogItemRepository;

        public GrantItemsConsumer(IRepository<InventoryItem> itemsRepository, IRepository<CatalogItem> catalogItemRepository)
        {
            this.catalogItemRepository = catalogItemRepository;
            this.itemsRepository = itemsRepository;
        }

        public async Task Consume(ConsumeContext<GrantItems> context)
        {
            var message = context.Message;

            var item = await catalogItemRepository.GetAsync(r => r.Id == message.CatalogItemId);

            if (item == null)
            {
                throw new UnknowItemException(message.CatalogItemId);
            }

            var inventosryItem = await itemsRepository.GetAsync(r => r.UserId == message.UserId
             && r.CatalogItemId == message.CatalogItemId);

            if (inventosryItem == null)
            {
                inventosryItem = new InventoryItem()
                {
                    UserId = message.UserId,
                    CatalogItemId = message.CatalogItemId,
                    Quantity = message.Quantity,
                    AcquiredDate = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                };

                await itemsRepository.CreateAsync(inventosryItem);
            }
            else
            {
                inventosryItem.Quantity += message.Quantity;

                await itemsRepository.UpdateAsync(inventosryItem);
            }

            await context.Publish(new InventoryItemsGranted(message.CorrelationId));
        }
    }
}