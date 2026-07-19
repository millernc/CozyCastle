using Godot;

public partial class AudioManager : Node
{
	private AudioStreamPlayer _sfxPlayer;
	private EventBus _eventBus;

	[Export]
	public AudioStream PickupSound { get; set; }

	public override void _Ready()
	{
		_sfxPlayer = new AudioStreamPlayer
		{
			Bus = "SFX",
			MaxPolyphony = 8
		};
		
		PickupSound = GD.Load<AudioStream>(
		"res://audio/sfx/pickups/pickup_sfx.wav"
);

		AddChild(_sfxPlayer);

		_eventBus = GetNode<EventBus>("/root/EventBus");
		_eventBus.ItemPickedUp += OnItemPickedUp;
	}

	private void OnItemPickedUp(
		string itemId,
		string displayName,
		int quantity
	)
	{
		PlaySfx(PickupSound);
	}

	public void PlaySfx(AudioStream stream)
	{
		if (stream == null)
		{
			return;
		}

		_sfxPlayer.Stream = stream;
		_sfxPlayer.Play();
	}

	public override void _ExitTree()
	{
		if (_eventBus != null)
		{
			_eventBus.ItemPickedUp -= OnItemPickedUp;
		}
	}
}
