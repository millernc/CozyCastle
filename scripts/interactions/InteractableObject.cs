using Godot;

public partial class InteractableObject : StaticBody2D, IInteractable
{
	[Export]
	public string PromptText { get; set; } = "Inspect";

	[Export(PropertyHint.MultilineText)]
	public string InteractionMessage { get; set; } =
		"You interacted with something.";

	public void Interact(Player player)
	{
		Main main = GetTree().CurrentScene as Main;
		main?.ShowDialogue(InteractionMessage);
	}
}
