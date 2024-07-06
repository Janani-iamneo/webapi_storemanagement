using Microsoft.AspNetCore.Mvc;
using dotnetapp.Services; 
using dotnetapp.Models;

namespace dotnetapp.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _inventoryService;

        public InventoryController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public IActionResult GetAllItems()
        {
            var items = _inventoryService.GetAllItems();
            return Ok(items);
        }

        [HttpGet("{itemId}")]
        public IActionResult GetItemById(int itemId)
        {
            var item = _inventoryService.GetItemById(itemId);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost]
        public IActionResult CreateItem([FromBody] InventoryItem newItem)
        {
            if (newItem == null)
            {
                return BadRequest();
            }

            _inventoryService.AddItem(newItem);
            return CreatedAtAction(nameof(GetItemById), new { itemId = newItem.ItemId }, newItem);
        }

        [HttpPut("{itemId}")]
        public IActionResult UpdateItem(int itemId, [FromBody] InventoryItem updatedItem)
        {
            if (updatedItem == null || itemId != updatedItem.ItemId)
            {
                return BadRequest();
            }

            var item = _inventoryService.GetItemById(itemId);
            if (item == null)
            {
                return NotFound();
            }

            _inventoryService.UpdateItem(itemId, updatedItem);
            return NoContent();
        }

        [HttpDelete("{itemId}")]
        public IActionResult DeleteItem(int itemId)
        {
            var item = _inventoryService.GetItemById(itemId);
            if (item == null)
            {
                return NotFound();
            }

            _inventoryService.DeleteItem(itemId);
            return NoContent();
        }
    }
}
