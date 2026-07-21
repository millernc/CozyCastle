using Godot;

public partial class LampT : StaticBody2D, IInteractable
{
	private PointLight2D _light;

	public string PromptText =>
		_light != null && _light.Enabled
			? "Turn off lamp"
			: "Turn on lamp";

	public override void _Ready()
	{
		_light = GetNode<PointLight2D>("Light");
	}

	public void Interact(Player player)
	{
		_light.Enabled = !_light.Enabled;
	}
}
