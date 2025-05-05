using System;
using System.Collections.Generic;

namespace TopDownGame.Models
{
    public class Inventory
    {
        public int Gold { get; private set; }
        private readonly Dictionary<string, int> _items = new();

        public void AddGold(int amount)
        {
            Gold += amount;
        }

        public bool RemoveGold(int amount)
        {
            if (Gold < amount) return false;
            Gold -= amount;
            return true;
        }

        public void AddItem(string itemId, int count = 1)
        {
            if (_items.ContainsKey(itemId))
                _items[itemId] += count;
            else
                _items.Add(itemId, count);
        }

        public bool RemoveItem(string itemId, int count = 1)
        {
            if (!_items.ContainsKey(itemId) || _items[itemId] < count)
                return false;

            _items[itemId] -= count;
            if (_items[itemId] <= 0)
                _items.Remove(itemId);
            return true;
        }

        public int GetItemCount(string itemId)
        {
            return _items.TryGetValue(itemId, out var count) ? count : 0;
        }

        public Dictionary<string, int> GetAllItems()
        {
            return new Dictionary<string, int>(_items);
        }
    }
}