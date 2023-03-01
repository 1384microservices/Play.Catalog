using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private static readonly List<ItemDto> items = new()
    {
        new ItemDto(Guid.NewGuid(), "Potion", "Restore a small amount of HP", 5, DateTimeOffset.UtcNow),
        new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
        new ItemDto(Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 7, DateTimeOffset.UtcNow)
    };

    // GET /items
    [HttpGet]
    public IEnumerable<ItemDto> Get()
    {
        return items;
    }

    // GET /items/12345
    [HttpGet("{id}")]
    public ActionResult<ItemDto> GetById(Guid id)
    {
        var item = items.Where(i => i.Id == id).SingleOrDefault();
        if (item == null)
            return NotFound();
        return Ok(item);
    }

    // POST /items
    [HttpPost]
    public ActionResult<ItemDto> Post(CreateItemDto dto)
    {
        var item = new ItemDto(Guid.NewGuid(), dto.Name, dto.Description, dto.Price, DateTimeOffset.UtcNow);
        items.Add(item);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }


    // PUT /items/{id}
    [HttpPut("{id}")]
    public IActionResult Put(Guid id, UpdateItemDto dto)
    {
        var existingItem = items.Where(i => i.Id == id).SingleOrDefault();
        if (existingItem == null)
            return NotFound();

        items.Remove(existingItem);
        var newUitem = existingItem with { Name = dto.Name, Description = dto.Description };
        items.Add(newUitem);

        return NoContent();
    }

    // DELETE
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var existingItem = items.Where(i => i.Id == id).SingleOrDefault();
        if (existingItem != null)
            items.Remove(existingItem);
        return NoContent();
    }
}