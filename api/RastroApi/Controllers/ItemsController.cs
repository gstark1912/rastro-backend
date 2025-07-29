using Microsoft.AspNetCore.Mvc;
using RastroApi.Models;
using RastroApi.Services;

namespace RastroApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IItemRepository _itemRepository;

    public ItemsController(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Item>>> GetAll()
    {
        var items = await _itemRepository.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Item>> GetById(string id)
    {
        var item = await _itemRepository.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Item>> Create(Item item)
    {
        var createdItem = await _itemRepository.CreateAsync(item);
        return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Item item)
    {
        var existingItem = await _itemRepository.GetByIdAsync(id);
        if (existingItem == null)
        {
            return NotFound();
        }

        item.Id = id;
        await _itemRepository.UpdateAsync(id, item);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingItem = await _itemRepository.GetByIdAsync(id);
        if (existingItem == null)
        {
            return NotFound();
        }

        await _itemRepository.DeleteAsync(id);
        return NoContent();
    }
}
