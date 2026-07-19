using Godot;

public partial class GameSession : Node
{
	[Signal]
	public delegate void TimeChangedEventHandler(
		int day,
		int hour,
		int minute
	);

	public Inventory Inventory { get; private set; }

	public int Day { get; private set; } = 1;
	public int Hour { get; private set; } = 8;
	public int Minute { get; private set; } = 0;

	public float GameMinutesPerRealSecond { get; set; } = 1.0f;

	private double _minuteAccumulator;

	public override void _Ready()
	{
		Inventory = new Inventory(capacity: 20);
	}

	public override void _Process(double delta)
	{
		_minuteAccumulator += delta * GameMinutesPerRealSecond;

		while (_minuteAccumulator >= 1.0)
		{
			_minuteAccumulator -= 1.0;
			AdvanceOneMinute();
		}
	}

	private void AdvanceOneMinute()
	{
		Minute++;

		if (Minute >= 60)
		{
			Minute = 0;
			Hour++;
		}

		if (Hour >= 24)
		{
			Hour = 0;
			Day++;
		}

		EmitSignal(
			SignalName.TimeChanged,
			Day,
			Hour,
			Minute
		);
	}
}
