using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Extensions;
using Play.Common.Interfaces;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemController : ControllerBase
    {
        private readonly IRepository<Item> itemsRepository;

        private static int requestCounter = 0;

        public ItemController(IRepository<Item> itemRepository)
        {
            this.itemsRepository = itemRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            requestCounter++;

            Console.WriteLine($"Request {requestCounter}: Starting...");

            if (requestCounter <= 2)
            {
                Console.WriteLine($"Request {requestCounter}: Delaying...");
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
            
            if (requestCounter <= 4)
            {
                Console.WriteLine($"Request {requestCounter}: 500 Internal Server Error...");
                return StatusCode(500);
            }

            var items = (await itemsRepository.GetAllAsync())
            .Select(r => r.AsDto());

            Console.WriteLine($"Request {requestCounter}: 200 (Ok)...");

            return Ok(items);
        }

        [HttpGet("{id}")]
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

            return CreatedAtAction(nameof(GetAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
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

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            await itemsRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}
