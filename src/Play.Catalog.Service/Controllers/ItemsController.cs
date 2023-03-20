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
using Play.Common;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = AdminRole)]
public class ItemsController : ControllerBase
{
    private const string AdminRole = "Admin";
    private readonly IRepository<Item> _repository;
    private readonly IPublishEndpoint _publishEndPoint;

    public ItemsController(IRepository<Item> repository, IPublishEndpoint publishEndPoint)
    {
        _repository = repository;
        _publishEndPoint = publishEndPoint;
    }

    // GET /items
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
    {
        var items = (await _repository.GetAllAsync()).Select(item => item.AsDto());
        return Ok(items);
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
        await _publishEndPoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

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
        await _publishEndPoint.Publish(new CatalogItemUpdated(item.Id, item.Name, item.Description));

        return NoContent();
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var item = await _repository.GetOneAsync(id);
        if (item != null)
        {
            await _repository.DeleteAsync(item);
            await _publishEndPoint.Publish(new CatalogItemDeleted(id));
        }

        return NoContent();
    }
}