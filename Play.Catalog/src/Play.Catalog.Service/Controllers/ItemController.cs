using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Extensions;
using Play.Common.Interfaces;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    // [Authorize(Roles = AdminRole)]
    public class ItemController : ControllerBase
    {
        private const string AdminRole = "Admin";
        private readonly IRepository<Item> itemsRepository;
        private readonly IPublishEndpoint publishEndpoint;
        // private static int requestCounter = 0;

        public ItemController(IRepository<Item> itemRepository, IPublishEndpoint publishEndpoint)
        {
            this.itemsRepository = itemRepository;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        [Authorize(Policy = Policies.Read)]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            // requestCounter++;

            // Console.WriteLine($"Request {requestCounter}: Starting...");

            // if (requestCounter <= 2)
            // {
            //     Console.WriteLine($"Request {requestCounter}: Delaying...");
            //     await Task.Delay(TimeSpan.FromSeconds(10));
            // }

            // if (requestCounter <= 4)
            // {
            //     Console.WriteLine($"Request {requestCounter}: 500 Internal Server Error...");
            //     return StatusCode(500);
            // }

            var items = (await itemsRepository.GetAllAsync())
            .Select(r => r.AsDto());

            // Console.WriteLine($"Request {requestCounter}: 200 (Ok)...");

            return Ok(items);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = Policies.Read)]
        public async Task<ActionResult<ItemDto>> GetAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();

            }

            return item.AsDto();
        }

        [HttpPost]
        [Authorize(Policy = Policies.Write)]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItem)
        {
            var item = new Item
            {
                Description = createItem.Description,
                Name = createItem.Name,
                Price = createItem.Price,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
            };

            await itemsRepository.CreateAsync(item);

            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description, item.Price));

            return CreatedAtAction(nameof(GetAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Policies.Write)]
        public async Task<ActionResult> PutAsync(Guid id, UpdateItemDto updateItem)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            item.Description = updateItem.Description;
            item.Name = updateItem.Name;
            item.Price = updateItem.Price;
            item.UpdatedDate = DateTime.UtcNow;

            await itemsRepository.UpdateAsync(item);

            await publishEndpoint.Publish(new CatalogItemUpdated(item.Id, item.Name, item.Description, item.Price));

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.Write)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            await itemsRepository.DeleteAsync(id);

            await publishEndpoint.Publish(new CatalogItemDeleted(id));

            return NoContent();
        }
    }
}
