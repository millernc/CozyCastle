using Godot;

public partial class EventBus : Node
{
	[Signal]
	public delegate void InventoryChangedEventHandler();

	[Signal]
	public delegate void ItemPickedUpEventHandler(
		string itemId,
		string displayName,
		int quantity
	);
}
