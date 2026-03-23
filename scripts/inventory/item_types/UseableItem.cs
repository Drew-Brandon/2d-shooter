using Godot;

/// <summary>
/// An item that can be used by players.
/// </summary>
[GlobalClass]
public abstract partial class UseableItem : Item
{
	[Export]
	private PackedScene _displayUI;

	/// <summary>
	/// The display to instantiate in the player's UI.
	/// </summary>
	public PackedScene DisplayUI { get => _displayUI; }

	/// <summary>
	/// Equips the item with the specified player.
	/// </summary>
	/// <param name="plyr">
	/// The player that equips this item.
	/// </param>
	/// <returns>
	/// The display that was instantiated in the player's UI.
	/// </returns>
	public Control Equip(PlayerBody2D plyr)
	{
		return plyr.AddUI(_displayUI);
	}

	/// <summary>
	/// Uses the item.
	/// </summary>
	/// <param name="plyr">
	/// The player that is using this item.
	/// </param>
	/// <param name="inventory">
	/// The inventory that this item is being used in.
	/// </param>
	/// <param name="stack">
	/// The stack that this item is being used in.
	/// </param>
	/// <returns>
	/// Whether or not the item was successfuly used.
	/// </returns>
	public virtual bool Use(PlayerBody2D plyr, PlayerInventory inventory, ItemStack stack)
	{
		return false;
	}
}
