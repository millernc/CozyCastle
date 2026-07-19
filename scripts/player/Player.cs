using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed { get; set; } = 220.0f;

	[Export]
	public ItemData DebugItem { get; set; }

	public FacingDirection Facing { get; private set; } = FacingDirection.Down;
	public PlayerAnimationState AnimationState { get; private set; } = PlayerAnimationState.Idle;

	private Main _main;
	private Area2D _interactionArea;
	private Label _interactionPrompt;
	private IInteractable _currentInteractable;
	private GameSession _gameSession;
	private AnimatedSprite2D _sprite;
	
	public override void _Ready()
	{
		_main = GetTree().CurrentScene as Main;
		_interactionArea = GetNode<Area2D>("InteractionArea");
		_interactionPrompt = GetNode<Label>("InteractionPrompt");
		_gameSession = GetNode<GameSession>("/root/GameSession");
		_sprite = GetNode<AnimatedSprite2D>("Sprite");
		UpdateSpriteDirection();
	}

	public override void _PhysicsProcess(double _)
	{
		if (Input.IsActionJustPressed("pause"))
		{
			_main?.TogglePause();
			return;
		}

		if (_main != null && _main.IsTransitioning)
		{
			Velocity = Vector2.Zero;
			_interactionPrompt.Hide();
			return;
		}

		if (Input.IsActionJustPressed("inventory"))
		{
			_main?.ToggleInventory();
			return;
		}

		if (_main != null && _main.IsInventoryOpen)
		{
			Velocity = Vector2.Zero;
			_interactionPrompt.Hide();
			return;
		}

		if (_main != null && _main.IsDialogueOpen)
		{
			Velocity = Vector2.Zero;
			_interactionPrompt.Hide();

			if (Input.IsActionJustPressed("interact"))
			{
				_main.AdvanceDialogue();
			}

			return;
		}

		Vector2 direction = Input.GetVector(
			"move_left",
			"move_right",
			"move_up",
            "move_down"
		);

		UpdateFacing(direction);
		UpdateInteractionAreaPosition();

		Velocity = direction * Speed;
		MoveAndSlide();

		UpdateInteractionTarget();

		if (Input.IsActionJustPressed("interact"))
		{
			TryInteract();
		}

		if (Input.IsActionJustPressed("debug_add_item"))
		{
			if (DebugItem == null)
			{
				GD.PushWarning("No debug item assigned.");
			}
			else
			{
				bool added = _gameSession.Inventory.TryAdd(DebugItem);

				int appleCount = 0;

				foreach (InventorySlot slot in _gameSession.Inventory.Slots)
				{
					if (slot.Item.Id == DebugItem.Id)
					{
						appleCount += slot.Quantity;
					}
				}

				GD.Print(
					added
						? $"{DebugItem.DisplayName}: {appleCount}"
						: "Inventory full"
				);
			}
		}
	}

	private void TryInteract()
	{
		_currentInteractable?.Interact(this);
	}

	private void UpdateInteractionTarget()
	{
		_currentInteractable = FindNearestInteractable();

		if (_currentInteractable == null)
		{
			_interactionPrompt.Hide();
			return;
		}

		_interactionPrompt.Text = $"E · {_currentInteractable.PromptText}";
		_interactionPrompt.Show();
	}

	private void UpdateFacing(Vector2 direction)
	{
		if (direction == Vector2.Zero)
		{
			return;
		}

		if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
		{
			Facing = direction.X > 0
				? FacingDirection.Right
				: FacingDirection.Left;
		}
		else
		{
			Facing = direction.Y > 0
				? FacingDirection.Down
				: FacingDirection.Up;
		}
		UpdateSpriteDirection();
	}

	private void UpdateInteractionAreaPosition()
	{
		Vector2 offset = Facing switch
		{
			FacingDirection.Up => new Vector2(0, -40),
			FacingDirection.Down => new Vector2(0, 40),
			FacingDirection.Left => new Vector2(-40, 0),
			FacingDirection.Right => new Vector2(40, 0),
			_ => Vector2.Zero
		};

		_interactionArea.Position = offset;
	}

	private IInteractable FindNearestInteractable()
	{
		Godot.Collections.Array<Node2D> nearbyBodies =
			_interactionArea.GetOverlappingBodies();

		Node2D nearestBody = null;
		float nearestDistanceSquared = float.MaxValue;

		foreach (Node2D body in nearbyBodies)
		{
			if (body is not IInteractable)
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

		return nearestBody as IInteractable;
	}

	private void UpdateSpriteDirection()
	{
		string animationName = Facing switch
		{
			FacingDirection.Up => "idle_up",
			FacingDirection.Down => "idle_down",
			FacingDirection.Left => "idle_left",
			FacingDirection.Right => "idle_right",
			_ => "idle_down"
		};

		if (_sprite.Animation != animationName)
		{
			_sprite.Play(animationName);
		}
	}
}
