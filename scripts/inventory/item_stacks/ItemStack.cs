using System;

/// <summary>
/// Represents a stack/collection of items.
/// </summary>
public partial class ItemStack : ICloneable
{
	/// <summary>
	/// The current amount of items within the stack.
	/// </summary>
	public int CurAmount = 0;

	/// <summary>
	/// The current item stored within the stack.
	/// </summary>
	public Item CurItem = null;

	/// <summary>
	/// The main constructor for ItemStack.
	/// </summary>
	/// <param name="amount">
	/// The amount of items to be stored within this stack.
	/// </param>
	/// <param name="item">
	/// The item to be stored within this stack.
	/// </param>
	public ItemStack(int amount, Item item)
	{
		CurAmount = amount;
		CurItem = item;
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
