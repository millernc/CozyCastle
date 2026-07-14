using Godot;

public partial class Door : StaticBody2D, IInteractable
{
	[Export]
	public string PromptText { get; set; } = "Enter";

	[Export]
	public string TargetSpawnName { get; set; }

	[Export(PropertyHint.File, "*.tscn")]
	public string TargetScenePath { get; set; }

	public void Interact(Player player)
	{
		if (string.IsNullOrWhiteSpace(TargetScenePath))
		{
			GD.PushWarning($"{Name} has no target scene assigned.");
			return;
		}

		if (string.IsNullOrWhiteSpace(TargetSpawnName))
		{
			GD.PushWarning($"{Name} has no target spawn assigned.");
			return;
		}

		Main main = GetTree().CurrentScene as Main;

		if (main == null)
		{
			GD.PushError("Door could not find Main.");
			return;
		}

		main.TransitionToScene(
			TargetScenePath,
			TargetSpawnName
		);
	}
}
