using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Common.Interfaces;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Extensions;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemController : ControllerBase
    {
        private readonly IRepository<InventoryItem> itemsRepository;
        private readonly CatalogClient catalogClient;
        public ItemController(IRepository<InventoryItem> itemRepository, CatalogClient catalogClient)
        {
            this.itemsRepository = itemRepository;
            this.catalogClient = catalogClient;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var catalogItems = await catalogClient.GetCatalogItemsAsync();

            var inventoryItems = await itemsRepository.GetAllAsync(r => r.UserId == userId);

            var inventoryItemDtos = inventoryItems.Select(inventoryItem =>
            {
                var catalogItem = catalogItems.SingleOrDefault(r => r.Id == inventoryItem.CatalogItemId);

                if (catalogItem != null)
                {
                    return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
                }
                else
                {
                    return inventoryItem.AsDto(string.Empty, string.Empty);
                }
            });

            return Ok(inventoryItemDtos);
        }

        // [HttpGet("{userId}")]
        // public async Task<ActionResult<InventoryItemDto>> GetAsync(Guid userId)
        // {
        //     var item = await itemsRepository.GetAsync(r => r.UserId == userId);

        //     if (item == null)
        //     {
        //         return NotFound();

        //     }

        //     return item.AsDto();
        // }

        [HttpPost]
        public async Task<ActionResult<InventoryItemDto>> PostAsync(GrantItemsDto grantItem)
        {
            var inventosryItem = await itemsRepository.GetAsync(r => r.UserId == grantItem.UserId
            && r.CatalogItemId == grantItem.CatalogItemId);

            if (inventosryItem == null)
            {
                inventosryItem = new InventoryItem()
                {
                    UserId = grantItem.UserId,
                    CatalogItemId = grantItem.CatalogItemId,
                    Quantity = grantItem.Quantity,
                    AcquiredDate = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                };

                await itemsRepository.CreateAsync(inventosryItem);
            }
            else
            {
                inventosryItem.Quantity += grantItem.Quantity;

                await itemsRepository.UpdateAsync(inventosryItem);
            }

            return Ok();
        }
    }
}