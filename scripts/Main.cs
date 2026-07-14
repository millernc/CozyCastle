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
	}

	private void MovePlayerToPendingSpawn()
	{
		if (string.IsNullOrWhiteSpace(TransitionData.TargetSpawnName))
		{
			return;
		}

		Player player = GetNodeOrNull<Player>("World/Player");

		Marker2D spawn = GetNodeOrNull<Marker2D>(
			$"World/CurrentRoom/{TransitionData.TargetSpawnName}"
		);

		if (player == null || spawn == null)
		{
			GD.PushWarning(
				$"Could not find player or spawn: {TransitionData.TargetSpawnName}"
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
}
