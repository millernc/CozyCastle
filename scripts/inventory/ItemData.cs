using Godot;

[GlobalClass]
public partial class ItemData : Resource
{
	[Export]
	public string Id { get; set; } = string.Empty;

	[Export]
	public string DisplayName { get; set; } = "Unnamed Item";

	[Export(PropertyHint.MultilineText)]
	public string Description { get; set; } = string.Empty;

	[Export]
	public bool Stackable { get; set; } = true;

	[Export]
	public int MaxStackSize { get; set; } = 99;
}
