using Godot;

public partial class GridOverlay : Node2D
{
	[Export]
	public int CellSize { get; set; } = 32;

	[Export]
	public Vector2I GridSize { get; set; } = new(36, 20);

	[Export]
	public bool ShowGrid { get; set; } = true;

	public override void _Draw()
	{
		if (!ShowGrid || CellSize <= 0)
		{
			return;
		}

		float width = GridSize.X * CellSize;
		float height = GridSize.Y * CellSize;

		for (int x = 0; x <= GridSize.X; x++)
		{
			float positionX = x * CellSize;

			DrawLine(
				new Vector2(positionX, 0),
				new Vector2(positionX, height),
				new Color(1, 1, 1, 0.15f),
				1
			);
		}

		for (int y = 0; y <= GridSize.Y; y++)
		{
			float positionY = y * CellSize;

			DrawLine(
				new Vector2(0, positionY),
				new Vector2(width, positionY),
				new Color(1, 1, 1, 0.15f),
				1
			);
		}
	}
}
