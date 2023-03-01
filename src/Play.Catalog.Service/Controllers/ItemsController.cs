using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Extensions;
using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IItemsRepository _repository;

    public ItemsController(IItemsRepository repository)
    {
        _repository = repository;
    }

    // GET /items
    [HttpGet]
    public async Task<IEnumerable<ItemDto>> GetAsync()
    {
        var items = await _repository.GetAllAsync();
        var dtos = items.Select(i => i.AsDto());
        return dtos;
    }

    // GET /items/12345
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
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
        var item = await _repository.GetByIdAsync(id);
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
        var item = await _repository.GetByIdAsync(id);
        if (item != null)
        {
            await _repository.DeleteAsync(item);
        }

        return NoContent();
    }
}