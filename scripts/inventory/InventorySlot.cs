using Godot;
using System;

/// <summary>
/// Represents a slot in an inventory that can be merged, exchanged and charged.
/// </summary>
public partial class InventorySlot
{
	private ItemStack _stack = null;

	public ItemStack Stack { get => _stack; }

	/// <summary>
	/// The current amount in the slot's stack.
	/// </summary>
	public int CurAmount { get => _stack != null ? _stack.CurAmount : 0; }

	/// <summary>
	/// The current item in the slot's stack.
	/// </summary>
	public Item CurItem { get => _stack != null ? _stack.CurItem : null; }

	private InventorySlotUI _slotUI = null;

	private void UpdateUI()
	{
		if (_slotUI != null)
		{
			if (_stack == null)
			{
				_slotUI.ClearIntLabel();
				_slotUI.SetIcon(null);
			}
			else
			{
				StringName type = _stack.GetType().ToString();

                switch (type)
				{
					case "GunStack":
                        GunStack gunStack = (GunStack)_stack;
                        _slotUI.SetIntLabel(gunStack.CurClipAmmo);
                        _slotUI.SetIcon(gunStack.CurItem.Icon);
                        break;
					case "ItemStack":
                        _slotUI.SetIntLabel(_stack.CurAmount);
                        _slotUI.SetIcon(_stack.CurItem.Icon);
                        break;
				}	
			}
		}
	}

	public void SetUI(InventorySlotUI slotUI)
	{
		_slotUI = slotUI;
		_slotUI.Slot = this;
		UpdateUI();
	}

	public void ClearUI()
	{
		_slotUI.ClearIntLabel();
		_slotUI.SetIcon(null);
		_slotUI.Slot = null;
		_slotUI = null;
	}

	/// <summary>
	/// Merges the slot's stack with the other specified stack.
	/// The process of merging mainly involves adding a certain amount to a stack.
	/// </summary>
	/// <param name="toMerge">
	/// The stack to merge with.
	/// </param>
	/// <returns>
	/// The amount that could not be added to the stack.
	/// </returns>
	public int MergeStack(ItemStack toMerge)
	{
		/*
		 * Perform the merge if the items of the stacks do match.
		 * If they do not match, then do not perform the merge.
		 */
		if (_stack == null || _stack.CurItem.Name == toMerge.CurItem.Name)
		{
			int prevAmount = _stack == null ? 0 : _stack.CurAmount;
			int newAmount = Mathf.Clamp(prevAmount + toMerge.CurAmount, 0, toMerge.CurItem.MaxAmount);
			_stack = (ItemStack)toMerge.Clone();
			_stack.CurAmount = newAmount;
			_stack.CurItem = toMerge.CurItem;

			UpdateUI();

			toMerge.CurAmount -= newAmount - prevAmount;
		}

		return toMerge.CurAmount;
	}

	/// <summary>
	/// Exchanges this slot's stack with the other specified slot's stack.
	/// </summary>
	/// <param name="other">
	/// The slot to exchange with.
	/// </param>
	public void ExchangeSlot(InventorySlot other)
	{
		/*
		 * If one of the slots has a null stack,
		 * or the items do not match,
		 * then just swap the values of the stacks.
		 * Otherwise, merge the other slot's stack with this slot's stack.
		 */
		if (_stack == null || other._stack == null || _stack.CurItem != other._stack.CurItem)
		{
			ItemStack swapped = other._stack;
			other._stack = _stack;
			_stack = swapped;
		}
		else
		{
			int remainder = other.MergeStack(_stack);
			_stack.CurAmount = remainder;

			if (_stack.CurAmount == 0)
			{
				_stack = null;
			}
		}

		// Trigger the slot updates.
		UpdateUI();
		other.UpdateUI();
	}

	/// <summary>
	/// Charges the slot's stack with the other specified stack.
	/// The process of charging mainly involves removing a certain amount off the stack.
	/// </summary>
	/// <param name="toCharge">
	/// The stack to charge.
	/// </param>
	/// <returns>
	/// The amount that could not be removed from the stack.
	/// </returns>
	public int ChargeStack(ItemStack toCharge)
	{
		/*
		 * Perform the charge if the items of the stacks do match.
		 * If they do not match, then do not perform the charge.
		 */
		if (_stack.CurItem != null && _stack.CurItem == toCharge.CurItem)
		{
			int toRemove = Mathf.Clamp(toCharge.CurAmount, 0, _stack.CurAmount);
			_stack.CurAmount -= toRemove;

			if (_stack.CurAmount == 0)
			{
				_stack = null;
			}

			UpdateUI();

			toCharge.CurAmount -= toRemove;
		}

		return toCharge.CurAmount;
	}
}
