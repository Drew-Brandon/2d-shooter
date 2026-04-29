using Godot;
using System;

/// <summary>
/// Represents a node that is used to execute the behavior of an item.
/// </summary>
public partial class ItemNode : Node
{
	private bool _canUse = false;

	public bool CanUse { get => _canUse; set => _canUse = value; }

	/// <summary>
	/// Uses the node for its specific purpose.
	/// </summary>
	/// <param name="plyr">
	/// The player using the node.
	/// </param>
	/// <param name="slot">
	/// The slot that the node is being used with/from.
	/// </param>
	/// <param name="usePos">
	/// The position the node is being used on.
	/// </param>
	/// <returns>
	/// Whether or not the node was successfuly used.
	/// </returns>
	public bool Use(PlayerBody2D plyr, PlayerInventory inventory, BaseItemSlot slot, Vector2 usePos)
	{
		if (_canUse)
		{
			return OnUse(plyr, inventory, slot, usePos);
		}

		return false;
	}

	protected virtual bool OnUse(PlayerBody2D plyr, PlayerInventory inventory, BaseItemSlot slot, Vector2 usePos)
	{
		return false;
	}

	/// <summary>
	/// Equips the node from the specified item slot.
	/// </summary>
	/// <param name="slot">
	/// The slot that the node is being equipped from.
	/// </param>
	public virtual void Equip(BaseItemSlot slot)
	{
		
	}

	/// <summary>
	/// Unequips the node from the specified item slot.
	/// </summary>
	/// <param name="slot">
	/// The slot that the node is being unequipped from.
	/// </param>
	public virtual void Unequip(BaseItemSlot slot)
	{

	}
}
