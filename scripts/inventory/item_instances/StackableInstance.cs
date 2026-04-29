using Godot;
using System;

/// <summary>
/// Represents a stack/collection of items.
/// </summary>
public partial class StackableInstance : ItemInstanceGeneric<StackableItem>
{
	/// <summary>
	/// The current amount of items within the stack.
	/// </summary>
	[Export]
	public int Amount = 0;

	public StackableInstance() : base()
	{
		Amount = 0;
	}

	/// <summary>
	/// The main constructor for StackableInstance.
	/// </summary>
	/// <param name="amount">
	/// The amount of items to be stored within this stack.
	/// </param>
	/// <param name="item">
	/// The item to be stored within this stack.
	/// </param>
	public StackableInstance(int amount, StackableItem item) : base(item)
	{
		Amount = amount;
	}

	public override BaseItemSlot GetSlot()
	{
		StackableInstance stack = new(0, Item);
		return new StackableItemSlot(stack);
	}

	public override BaseItemSlot LoadIntoSlot()
	{
		StackableInstance stack = new(Amount, Item);
		return new StackableItemSlot(stack);
	}
}
