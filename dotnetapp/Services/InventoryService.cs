using System.Collections.Generic;
using System.Linq; 
using dotnetapp.Models;

namespace dotnetapp.Services
{
    public class InventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<InventoryItem> GetAllItems()
        {
            return _context.InventoryItems.ToList();
        }

        public InventoryItem GetItemById(int itemId)
        {
            return _context.InventoryItems.Find(itemId);
        }

        public void AddItem(InventoryItem newItem)
        {
            _context.InventoryItems.Add(newItem);
            _context.SaveChanges();
        }

        public void UpdateItem(int itemId, InventoryItem updatedItem)
        {
            var item = _context.InventoryItems.Find(itemId);
            if (item != null)
            {
                item.ItemName = updatedItem.ItemName;
                item.Quantity = updatedItem.Quantity;
                item.Price = updatedItem.Price;
                item.Category = updatedItem.Category;
                _context.SaveChanges();
            }
        }

        public void DeleteItem(int itemId)
        {
            var item = _context.InventoryItems.Find(itemId);
            if (item != null)
            {
                _context.InventoryItems.Remove(item);
                _context.SaveChanges();
            }
        }
    }
}
