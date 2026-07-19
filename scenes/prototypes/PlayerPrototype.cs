using Godot;

public partial class PlayerPrototype : CharacterBody3D
{
	private enum FacingDirection
	{
		Down,
		Up,
		Left,
		Right
	}

	[Export]
	public float Speed { get; set; } = 4.0f;

	[Export]
	public Texture2D FrontTexture { get; set; }

	[Export]
	public Texture2D BackTexture { get; set; }

	[Export]
	public Texture2D LeftTexture { get; set; }

	[Export]
	public Texture2D RightTexture { get; set; }

	private Sprite3D _sprite;
	private FacingDirection _facing = FacingDirection.Down;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite3D>("Sprite3D");
		UpdateSprite();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 input = Input.GetVector(
			"move_left",
			"move_right",
			"move_up",
			"move_down"
		);

		Vector3 direction = new(
			input.X,
			0.0f,
			input.Y
		);

		// Pokémon-style four-direction movement:
		// prevent diagonal movement by keeping the stronger axis.
		if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Z))
		{
			direction.Z = 0.0f;
		}
		else
		{
			direction.X = 0.0f;
		}

		if (direction != Vector3.Zero)
		{
			direction = direction.Normalized();

			UpdateFacing(direction);

			Velocity = new Vector3(
				direction.X * Speed,
				Velocity.Y,
				direction.Z * Speed
			);
		}
		else
		{
			Velocity = new Vector3(
				0.0f,
				Velocity.Y,
				0.0f
			);
		}

		MoveAndSlide();
	}

	private void UpdateFacing(Vector3 direction)
	{
		FacingDirection newFacing;

		if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Z))
		{
			newFacing = direction.X < 0
				? FacingDirection.Left
				: FacingDirection.Right;
		}
		else
		{
			newFacing = direction.Z < 0
				? FacingDirection.Up
				: FacingDirection.Down;
		}

		if (newFacing == _facing)
		{
			return;
		}

		_facing = newFacing;
		UpdateSprite();
	}

	private void UpdateSprite()
	{
		_sprite.Texture = _facing switch
		{
			FacingDirection.Up => BackTexture,
			FacingDirection.Left => LeftTexture,
			FacingDirection.Right => RightTexture,
			_ => FrontTexture
		};
	}
}
