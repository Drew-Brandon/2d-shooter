using Godot;

/// <summary>
/// A container that is used to interface with a inventory of finite size.
/// </summary>
public partial class FiniteContainer : Node2D, IInteractable
{
	private InventoryUI _curUI;

	[Export]
	private float _range = 256f;

	[Export]
	private FiniteInventory _inventory = null;

	[Export]
	private PackedScene _uiScene;

	public bool Interact(PlayerBody2D plyr)
	{
		if (plyr.GlobalPosition.DistanceTo(GlobalPosition) > _range)
		{
			return false;
		}

		plyr.ToggleInventory(true);
		_curUI = plyr.AddUI<InventoryUI>(_uiScene);
		_inventory.SetUI(_curUI);
		return true;
	}

	public void StopInteract(PlayerBody2D plyr)
	{
		plyr.ToggleInventory(false);
		_inventory.ClearUI();
		_curUI.QueueFree();
	}

	public bool InteractUpdate(float delta, PlayerBody2D plyr)
	{
		return !plyr.InventoryOpen || plyr.GlobalPosition.DistanceTo(GlobalPosition) > _range;
	}
}
