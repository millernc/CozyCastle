using Godot;

public partial class Lamp : StaticBody3D
{
	private OmniLight3D _light;
	private float _onEnergy;

	public string PromptText =>
		_light != null && _light.LightEnergy > 0.0f
			? "Turn off lamp"
			: "Turn on lamp";

	public override void _Ready()
	{
		AddToGroup("interactable");

		_light = GetNode<OmniLight3D>("Light");

		_onEnergy = _light.LightEnergy;

		// _light.LightEnergy = 0.0f;

		GD.Print("Lamp is ready and interactable.");
	}

	public void Interact()
	{
		bool isOn = _light.LightEnergy > 0.0f;
		_light.LightEnergy = isOn ? 0.0f : _onEnergy;

		GD.Print(isOn ? "Lamp turned off." : "Lamp turned on.");
	}
}
