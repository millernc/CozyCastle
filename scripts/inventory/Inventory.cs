using System;
using System.Collections.Generic;

public sealed class Inventory
{
	public int Capacity { get; }

	private readonly List<InventorySlot> _slots = new();

	public IReadOnlyList<InventorySlot> Slots => _slots;

	public Inventory(int capacity)
	{
		if (capacity <= 0)
		{
			throw new ArgumentOutOfRangeException(
				nameof(capacity),
				"Inventory capacity must be greater than zero."
			);
		}

		Capacity = capacity;
	}

	public bool TryAdd(ItemData item, int quantity = 1)
	{
		if (item == null || quantity <= 0)
		{
			return false;
		}

		int remaining = quantity;

		if (item.Stackable)
		{
			foreach (InventorySlot slot in _slots)
			{
				if (slot.Item.Id != item.Id)
				{
					continue;
				}

				remaining = slot.Add(remaining);

				if (remaining == 0)
				{
					return true;
				}
			}
		}

		while (remaining > 0 && _slots.Count < Capacity)
		{
			int stackSize = item.Stackable
				? Math.Min(remaining, item.MaxStackSize)
				: 1;

			_slots.Add(new InventorySlot(item, stackSize));
			remaining -= stackSize;
		}

		return remaining == 0;
	}

	public bool HasItem(string itemId, int quantity = 1)
	{
		int total = 0;

		foreach (InventorySlot slot in _slots)
		{
			if (slot.Item.Id != itemId)
			{
				continue;
			}

			total += slot.Quantity;

			if (total >= quantity)
			{
				return true;
			}
		}

		return false;
	}

	public bool TryRemove(string itemId, int quantity = 1)
	{
		if (!HasItem(itemId, quantity))
		{
			return false;
		}

		int remaining = quantity;

		for (int i = _slots.Count - 1; i >= 0 && remaining > 0; i--)
		{
			InventorySlot slot = _slots[i];

			if (slot.Item.Id != itemId)
			{
				continue;
			}

			int removed = slot.Remove(remaining);
			remaining -= removed;

			if (slot.Quantity == 0)
			{
				_slots.RemoveAt(i);
			}
		}

		return true;
	}
}