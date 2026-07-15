public interface IInteractable
{
	string PromptText { get; }

	void Interact(Player player);
}
