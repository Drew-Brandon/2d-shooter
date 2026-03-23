using Godot;

public partial class FiniteContainer : Node2D, IInteractable
{
	private bool _isOpen = false;

	private InventoryUI _curUI;

	[Export]
	private FiniteInventory _inventory = null;

	[Export]
	private PackedScene _uiScene;

	public bool Interact(PlayerBody2D plyr)
	{
		_isOpen = !_isOpen;

		if (_isOpen)
		{
			_curUI = plyr.OpenContainerUI(_uiScene, _inventory);
		}
		else
		{
			_inventory.ClearUI();
			_curUI.QueueFree();
		}

		return true;
	}

	public void StopInteract(PlayerBody2D plyr)
	{

	}
}
