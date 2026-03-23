using Godot;

/// <summary>
/// Represents a slot in an inventory that can be merged, exchanged and charged.
/// </summary>
public partial class InventorySlot
{
	protected ItemStack Stack = null;

	/// <summary>
	/// The current amount in the slot's stack.
    /// </summary>
    public int CurAmount { get => Stack.CurAmount; }

    /// <summary>
	/// The current item in the slot's stack.
	/// </summary>
	public Item CurItem { get => Stack.CurItem; }

	private InventorySlotUI _slotUI = null;

	private void UpdateUI()
	{
		if (_slotUI != null)
		{
			if (Stack == null)
			{
				_slotUI.ClearIntLabel();
				_slotUI.SetIcon(null);
			}
			else if (Stack is GunStack)
			{
				GunStack gunStack = (GunStack)Stack;
				_slotUI.SetIntLabel(gunStack.CurClipAmmo);
				_slotUI.SetIcon(gunStack.CurItem.Icon);
			}
			else
			{
				_slotUI.SetIntLabel(Stack.CurAmount);
				_slotUI.SetIcon(Stack.CurItem.Icon);
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
        if (Stack == null || Stack.CurItem.Name == toMerge.CurItem.Name)
        {
            int prevAmount = Stack == null ? 0 : Stack.CurAmount;
            int newAmount = Mathf.Clamp(prevAmount + toMerge.CurAmount, 0, toMerge.CurItem.MaxAmount);
            Stack = (ItemStack)toMerge.Clone();
            Stack.CurAmount = newAmount;
            Stack.CurItem = toMerge.CurItem;

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
        if (Stack == null || other.Stack == null || Stack.CurItem != other.Stack.CurItem)
        {
            ItemStack swapped = other.Stack;
            other.Stack = Stack;
            Stack = swapped;
        }
        else
        {
            int remainder = other.MergeStack(Stack);
            Stack.CurAmount = remainder;

            if (Stack.CurAmount == 0)
            {
                Stack = null;
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
		if (Stack.CurItem != null && Stack.CurItem == toCharge.CurItem)
		{
			int toRemove = Mathf.Clamp(toCharge.CurAmount, 0, Stack.CurAmount);
			Stack.CurAmount -= toRemove;

			if (Stack.CurAmount == 0)
			{
				Stack = null;
			}

			UpdateUI();

			toCharge.CurAmount -= toRemove;
		}

		return toCharge.CurAmount;
	}
}
