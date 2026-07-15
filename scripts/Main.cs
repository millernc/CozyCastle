using Godot;

public partial class Main : Node2D
{
	private PanelContainer _dialogueBox;
	private Label _dialogueText;
	private string[] _currentDialogueLines;
	private int _currentDialogueIndex;
	private Label _speakerName;
	private ColorRect _fadeOverlay;
	private Tween _fadeTween;
	private bool _isTransitioning;
	public bool IsTransitioning => _isTransitioning;
	private PanelContainer _inventoryPanel;
	private VBoxContainer _itemList;
	private GameSession _gameSession;

	public bool IsInventoryOpen => _inventoryPanel.Visible;
	public bool IsDialogueOpen => _dialogueBox.Visible;

	public override void _Ready()
	{
		_dialogueBox = GetNode<PanelContainer>("UI/DialogueBox");
		_speakerName = GetNode<Label>(
			"UI/DialogueBox/MarginContainer/VBoxContainer/SpeakerName"
		);
		_dialogueText = GetNode<Label>(
			"UI/DialogueBox/MarginContainer/VBoxContainer/DialogueText"
		);

		_dialogueBox.Hide();
		MovePlayerToPendingSpawn();
		
		_fadeOverlay = GetNode<ColorRect>("UI/FadeOverlay");
		_fadeOverlay.Modulate = new Color(1, 1, 1, 0);
		
		FadeFromBlack();

		_gameSession = GetNode<GameSession>("/root/GameSession");

		_inventoryPanel =
			GetNode<PanelContainer>("UI/InventoryPanel");

		_itemList =
			GetNode<VBoxContainer>(
				"UI/InventoryPanel/MarginContainer/VBoxContainer/ItemList"
			);

		_inventoryPanel.Hide();
	}

	private void MovePlayerToPendingSpawn()
	{
		if (string.IsNullOrWhiteSpace(TransitionData.TargetSpawnName))
		{
			return;
		}

		Player player = GetNodeOrNull<Player>("World/Player");
		Node currentRoom = GetNodeOrNull("World/CurrentRoom");

		if (player == null || currentRoom == null)
		{
			GD.PushWarning("Could not find Player or CurrentRoom.");
			return;
		}

		Marker2D spawn = currentRoom.FindChild(
			TransitionData.TargetSpawnName,
			recursive: true,
			owned: false
		) as Marker2D;

		if (spawn == null)
		{
			GD.PushWarning(
				$"Could not find spawn: {TransitionData.TargetSpawnName}"
			);
			return;
		}

		player.GlobalPosition = spawn.GlobalPosition;
		TransitionData.TargetSpawnName = null;
	}

	public void ShowDialogue(string message)
	{
		StartDialogue(string.Empty, new[] { message });
	}

	public void HideDialogue()
	{
		EndDialogue();
	}

	public void StartDialogue(string speaker, string[] lines)
	{
		if (lines == null || lines.Length == 0)
		{
			return;
		}

		_speakerName.Text = speaker;
		_speakerName.Visible = !string.IsNullOrWhiteSpace(speaker);

		_currentDialogueLines = lines;
		_currentDialogueIndex = 0;

		ShowCurrentDialogueLine();
		_dialogueBox.Show();
	}

	public void AdvanceDialogue()
	{

		if (!IsDialogueOpen || _currentDialogueLines == null)
		{
			return;
		}
		_currentDialogueIndex++;
		if (_currentDialogueIndex >= _currentDialogueLines.Length)
		{
			EndDialogue();
			return;
		}
		ShowCurrentDialogueLine();

	}

	private void ShowCurrentDialogueLine()
	{
		_dialogueText.Text = _currentDialogueLines[_currentDialogueIndex];
	}

	public void EndDialogue()
	{
		_dialogueBox.Hide();
		_currentDialogueLines = null;
		_currentDialogueIndex = 0;
	}
	
	public async void TransitionToScene(
		string targetScenePath,
		string targetSpawnName
	)
	{
		if (_isTransitioning)
		{
			return;
		}

		_isTransitioning = true;

		await FadeToBlack();

		TransitionData.TargetSpawnName = targetSpawnName;

		Error error = GetTree().ChangeSceneToFile(targetScenePath);

		if (error != Error.Ok)
		{
			GD.PushError(
				$"Could not change scene to {targetScenePath}: {error}"
			);
		}
	}
	
	private async System.Threading.Tasks.Task FadeToBlack()
	{
		_fadeTween?.Kill();

		_fadeTween = CreateTween();
		_fadeTween.TweenProperty(
			_fadeOverlay,
			"modulate:a",
			1.0f,
			0.25f
		);

		await ToSignal(_fadeTween, Tween.SignalName.Finished);
	}

	private async void FadeFromBlack()
	{
		_fadeOverlay.Modulate = new Color(1, 1, 1, 1);

		_fadeTween?.Kill();

		_fadeTween = CreateTween();
		_fadeTween.TweenProperty(
			_fadeOverlay,
			"modulate:a",
			0.0f,
			0.25f
		);

		await ToSignal(_fadeTween, Tween.SignalName.Finished);

		_isTransitioning = false;
	}

	public void ToggleInventory()
	{
		if (IsDialogueOpen || IsTransitioning)
		{
			return;
		}

		if (IsInventoryOpen)
		{
			CloseInventory();
		}
		else
		{
			OpenInventory();
		}
	}

	private void OpenInventory()
	{
		RefreshInventory();
		_inventoryPanel.Show();
	}

	private void CloseInventory()
	{
		_inventoryPanel.Hide();
	}

	private void RefreshInventory()
	{
		foreach (Node child in _itemList.GetChildren())
		{
			child.QueueFree();
		}

		if (_gameSession.Inventory.Slots.Count == 0)
		{
			Label emptyLabel = new()
			{
				Text = "Your inventory is empty."
			};

			_itemList.AddChild(emptyLabel);
			return;
		}

		foreach (InventorySlot slot in _gameSession.Inventory.Slots)
		{
			Label itemLabel = new()
			{
				Text = $"{slot.Item.DisplayName} × {slot.Quantity}"
			};

			_itemList.AddChild(itemLabel);
		}
	}
}
