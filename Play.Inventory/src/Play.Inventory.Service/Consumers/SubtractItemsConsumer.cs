using System;
using System.Threading.Tasks;
using MassTransit;
using Play.Common.Interfaces;
using Play.Inventory.Contracts;
using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Exceptions;

namespace Play.Inventory.Service.Consumers
{
    public class SubtractItemsConsumer : IConsumer<SubtractItems>
    {
        private readonly IRepository<InventoryItem> itemsRepository;
        private readonly IRepository<CatalogItem> catalogItemRepository;

        public SubtractItemsConsumer(IRepository<InventoryItem> itemsRepository, IRepository<CatalogItem> catalogItemRepository)
        {
            this.catalogItemRepository = catalogItemRepository;
            this.itemsRepository = itemsRepository;
        }

        public async Task Consume(ConsumeContext<SubtractItems> context)
        {
            var message = context.Message;

            var item = await catalogItemRepository.GetAsync(r => r.Id == message.CatalogItemId);

            if (item == null)
            {
                throw new UnknowItemException(message.CatalogItemId);
            }

            var inventosryItem = await itemsRepository.GetAsync(r => r.UserId == message.UserId
             && r.CatalogItemId == message.CatalogItemId);

            if (inventosryItem != null)
            {
                inventosryItem.Quantity += message.Quantity;

                await itemsRepository.UpdateAsync(inventosryItem);
            }

            await context.Publish(new InventoryItemsSubtracted(message.CorrelationId));
        }
    }
}