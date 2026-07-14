using Godot;

public partial class GameSession : Node
{
	public Inventory Inventory { get; private set; }

	public override void _Ready()
	{
		Inventory = new Inventory(capacity: 20);
	}
}
