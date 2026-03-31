using Godot;
using System;

/// <summary>
/// Represents an inventory with a finite/fixed size.
/// </summary>
public partial class FiniteInventory : Node
{
	[Export]
	private int _size = 16;

	[Export]
	private ItemStackResource[] _startStacks = null;

	private InventorySlot[] _slots = null;

	private void InitStacks()
	{
		_slots = new InventorySlot[_size];

		for (int i = 0; i < _size; i++)
		{
			_slots[i] = new InventorySlot();
		}
	}

	private void AddStartStacks()
	{
		if (_startStacks != null)
		{
			for (int i = 0; i < _startStacks.Length; i++)
			{
				AddStack(_startStacks[i].GetStack());
			}

			Array.Clear(_startStacks);
			_startStacks = null;
		}
	}

	public override void _Ready()
	{
		InitStacks();
		AddStartStacks();
	}

	/// <summary>
	/// Adds the specified stack to the inventory.
	/// </summary>
	/// <param name="toAdd">
	/// The stack to add.
	/// </param>
	/// <returns>
	/// The amount of the stack that could not be added.
	/// </returns>
	public int AddStack(ItemStack toAdd)
	{
		for (int i = 0; i < _size; i++)
		{
			toAdd.CurAmount = _slots[i].MergeStack(toAdd);

			if (toAdd.CurAmount == 0)
			{
				break;
			}
		}

		return toAdd.CurAmount;
	}

	/// <summary>
	/// Removes the specified stack from the inventory.
	/// </summary>
	/// <param name="toRemove">
	/// The stack to remove.
	/// </param>
	/// <returns>
	/// The amount of the stack that could not be removed.
	/// </returns>
	public int ChargeStack(ItemStack toRemove)
	{
		for (int i = 0; i < _size; i++)
		{
			_slots[i].ChargeStack(toRemove);

			if (toRemove.CurAmount == 0)
			{
				break;
			}
		}

		return toRemove.CurAmount;
	}

	/// <summary>
	/// Sets the UI for this inventory.
	/// </summary>
	/// <param name="ui">
	/// The UI to set to.
	/// </param>
	public void SetUI(InventoryUI ui)
	{
		for (int i = 0; i < _size; i++)
		{
			_slots[i].SetUI(ui.GetSlot(i));
		}
	}

	/// <summary>
	/// Clears the UI for this inventory.
	/// </summary>
	public void ClearUI()
	{
		for (int i = 0; i < _size; i++)
		{
			_slots[i].ClearUI();
		}
	}

	/// <summary>
	/// Gets the slot at the specified index.
	/// </summary>
	/// <param name="index">
	/// The index of the slot to get.
	/// </param>
	/// <returns>
	/// The slot at the specified index.
	/// </returns>
	public InventorySlot GetSlot(int index)
	{
		return _slots[index];
	}
}
