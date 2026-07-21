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
	public float Speed { get; set; } = 2.0f;

	[Export]
	public Texture2D FrontTexture { get; set; }

	[Export]
	public Texture2D BackTexture { get; set; }

	[Export]
	public Texture2D LeftTexture { get; set; }

	[Export]
	public Texture2D RightTexture { get; set; }

	[Export]
	public float InteractionDistance { get; set; } = 0.7f;

	private Sprite3D _sprite;
	private Area3D _interactionArea;
	private Label _interactionPrompt;
	private IInteractable3D _currentInteractable;
	private FacingDirection _facing = FacingDirection.Down;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite3D>("Sprite3D");
		_interactionArea = GetNode<Area3D>("InteractionArea");
		_interactionPrompt = GetTree().CurrentScene.GetNodeOrNull<Label>(
			"UI/InteractionPrompt"
		);

		if (_interactionPrompt == null)
		{
			GD.PushWarning("Could not find UI/InteractionPrompt.");
		}
		else
		{
			_interactionPrompt.Hide();
		}

		UpdateSprite();
		UpdateInteractionArea();
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

		// Prevent diagonal movement by keeping the stronger axis.
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
		UpdateInteractionTarget();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("interact"))
		{
			TryInteract();
			GetViewport().SetInputAsHandled();
		}
	}

	private void TryInteract()
	{
		_currentInteractable?.Interact(this);
	}

	private void UpdateInteractionTarget()
	{
		_currentInteractable = FindNearestInteractable();

		if (_interactionPrompt == null)
		{
			return;
		}

		if (_currentInteractable == null)
		{
			_interactionPrompt.Hide();
			return;
		}

		_interactionPrompt.Text = $"E  ·  {_currentInteractable.PromptText}";
		_interactionPrompt.Show();
	}

	private IInteractable3D FindNearestInteractable()
	{
		Node3D nearestBody = null;
		float nearestDistanceSquared = float.MaxValue;

		foreach (Node3D body in _interactionArea.GetOverlappingBodies())
		{
			if (body is not IInteractable3D)
			{
				continue;
			}

			float distanceSquared =
				GlobalPosition.DistanceSquaredTo(body.GlobalPosition);

			if (distanceSquared < nearestDistanceSquared)
			{
				nearestDistanceSquared = distanceSquared;
				nearestBody = body;
			}
		}

		return nearestBody as IInteractable3D;
	}

	private void UpdateFacing(Vector3 direction)
	{
		FacingDirection newFacing;

		if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Z))
		{
			newFacing = direction.X < 0.0f
				? FacingDirection.Left
				: FacingDirection.Right;
		}
		else
		{
			newFacing = direction.Z < 0.0f
				? FacingDirection.Up
				: FacingDirection.Down;
		}

		if (newFacing == _facing)
		{
			return;
		}

		_facing = newFacing;

		UpdateSprite();
		UpdateInteractionArea();
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

	private void UpdateInteractionArea()
	{
		_interactionArea.Position = _facing switch
		{
			FacingDirection.Up =>
				new Vector3(0.0f, 0.0f, -InteractionDistance),

			FacingDirection.Down =>
				new Vector3(0.0f, 0.0f, InteractionDistance),

			FacingDirection.Left =>
				new Vector3(-InteractionDistance, 0.0f, 0.0f),

			FacingDirection.Right =>
				new Vector3(InteractionDistance, 0.0f, 0.0f),

			_ => Vector3.Zero
		};
	}
}
