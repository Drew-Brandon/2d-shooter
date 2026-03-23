using Godot;
using System;

/// <summary>
/// Represents an inventory with a finite/fixed size.
/// </summary>
public abstract partial class FiniteInventory<T> : Node where T : InventorySlot, new()
{
	[Export]
	protected int Size = 16;

	[Export]
	private ItemStackResource[] _startStacks = null;

	protected T[] Slots = null;

	protected void InitStacks()
	{
		Slots = new T[Size];

		for (int i = 0; i < Size; i++)
		{
			Slots[i] = new T();
		}
	}

	protected void AddStartStacks()
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
		InitStacks<InventorySlot>();
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
		for (int i = 0; i < Size; i++)
		{
			toAdd.CurAmount = Slots[i].MergeStack(toAdd);

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
		for (int i = 0; i < Size; i++)
		{
			Slots[i].ChargeStack(toRemove);

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
		for (int i = 0; i < Size; i++)
		{
			Slots[i].SetUI(ui.GetSlot(i));
		}
	}

	/// <summary>
	/// Clears the UI for this inventory.
	/// </summary>
	public void ClearUI()
	{
		for (int i = 0; i < Size; i++)
		{
			Slots[i].ClearUI();
		}
	}
}
