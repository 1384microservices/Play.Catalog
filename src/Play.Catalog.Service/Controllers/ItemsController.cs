using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Extensions;
using Play.Common;
using Play.Common.Configuration;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("[controller]")]

public class ItemsController : ControllerBase
{
    private const string AdminRole = "Admin";
    private readonly IRepository<Item> _repository;
    private readonly IPublishEndpoint _publishEndPoint;
    private readonly Counter<int> _itemUpdatedCounter;

    public ItemsController(IRepository<Item> repository, IPublishEndpoint publishEndPoint, IConfiguration configuration)
    {
        _repository = repository;
        _publishEndPoint = publishEndPoint;

        var meter = new Meter(configuration.GetServiceSettings().Name);

        _itemUpdatedCounter = meter.CreateCounter<int>("CatalogItemUpdated");
    }

    // GET /items
    [HttpGet]
    [Authorize(Policies.Read)]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
    {
        var items = (await _repository.GetAllAsync()).Select(item => item.AsDto());
        return Ok(items);
    }

    // GET /items/12345
    [HttpGet("{id}")]
    [Authorize(Policies.Read)]
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
    [Authorize(Policies.Write)]
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
        await _publishEndPoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description, item.Price));

        return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item.AsDto());
    }


    // PUT /items/{id}
    [HttpPut("{id}")]
    [Authorize(Policies.Write)]
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
        await _publishEndPoint.Publish(new CatalogItemUpdated(item.Id, item.Name, item.Description, item.Price));
        _itemUpdatedCounter.Add(1, new KeyValuePair<string, object>("ItemId", id));
        return NoContent();
    }

    // DELETE
    [HttpDelete("{id}")]
    [Authorize(Policies.Write)]
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