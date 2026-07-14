public sealed class InventorySlot
{
	public ItemData Item { get; }
	public int Quantity { get; private set; }

	public InventorySlot(ItemData item, int quantity)
	{
		Item = item;
		Quantity = quantity;
	}

	public int Add(int amount)
	{
		int availableSpace = Item.MaxStackSize - Quantity;
		int amountToAdd = System.Math.Min(amount, availableSpace);

		Quantity += amountToAdd;

		return amount - amountToAdd;
	}

	public int Remove(int amount)
	{
		int amountToRemove = System.Math.Min(amount, Quantity);

		Quantity -= amountToRemove;

		return amountToRemove;
	}
}