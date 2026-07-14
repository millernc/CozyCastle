using Godot;

public partial class NPC : StaticBody2D, IInteractable
{
	[Export]
	public string NPCName { get; set; } = "Bob";

	[Export]
	public string PromptText { get; set; } = "Talk";

	[Export(PropertyHint.MultilineText)]
	public string[] DialogueLines { get; set; } =
	{
		"Hello.",
		"This place is still a little empty.",
		"But I think it has potential."
	};

	public void Interact(Player player)
	{
		Main main = GetTree().CurrentScene as Main;
		main?.StartDialogue(NPCName, DialogueLines);
	}
}
