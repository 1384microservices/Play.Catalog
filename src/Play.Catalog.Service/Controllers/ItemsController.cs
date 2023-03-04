using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Extensions;
using Play.Common;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private static int _requestsCounter = 0;

    private readonly IRepository<Item> _repository;

    public ItemsController(IRepository<Item> repository)
    {
        _repository = repository;
    }

    // GET /items
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
    {
        _requestsCounter++;
        Console.WriteLine($"Request {_requestsCounter}: Starting...");

        if (_requestsCounter <= 2)
        {
            Console.WriteLine($"Request {_requestsCounter}: Delaying...");
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        if (_requestsCounter <= 4)
        {

            Console.WriteLine($"Request {_requestsCounter}: 500 internal server error.");
            return StatusCode(500);
        }

        var items = await _repository.GetAllAsync();
        var dtos = items.Select(i => i.AsDto());
        return Ok(dtos);
    }

    // GET /items/12345
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
    {
        var item = await _repository.GetOneAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        return Ok(item.AsDto());
    }

    // POST /items
    [HttpPost]
    public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto dto)
    {
        var item = new Item()
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await _repository.CreateAsync(item);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item.AsDto());
    }


    // PUT /items/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, UpdateItemDto dto)
    {
        var item = await _repository.GetOneAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        item.Name = dto.Name;
        item.Description = dto.Description;
        item.Price = dto.Price;

        await _repository.UpdateAsync(item);

        return NoContent();
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var item = await _repository.GetOneAsync(id);
        if (item != null)
        {
            await _repository.DeleteAsync(item);
        }

        return NoContent();
    }
}