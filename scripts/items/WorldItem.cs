using Godot;

public partial class WorldItem : StaticBody2D, IInteractable
{
	[Export]
	public ItemData Item { get; set; }

	public string PromptText =>
		Item == null
			? "Pick up"
			: $"Pick up {Item.DisplayName}";

	public void Interact(Player player)
	{
		if (Item == null)
		{
			GD.PushWarning($"{Name} has no ItemData assigned.");
			return;
		}

		GameSession gameSession =
			GetNode<GameSession>("/root/GameSession");

		bool added = gameSession.Inventory.TryAdd(Item);

		if (!added)
		{
			GD.Print("Inventory full.");
			return;
		}

		EventBus eventBus =
			GetNode<EventBus>("/root/EventBus");

		eventBus.EmitSignal(
			EventBus.SignalName.ItemPickedUp,
			Item.Id,
			Item.DisplayName,
			1
		);

		eventBus.EmitSignal(
			EventBus.SignalName.InventoryChanged
		);

		QueueFree();
	}
}
