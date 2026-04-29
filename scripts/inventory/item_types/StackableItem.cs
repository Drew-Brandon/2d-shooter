using Godot;
using System;

[GlobalClass]
public partial class StackableItem : BaseItem
{
	[Export]
	private int _maxAmount = 16;

	/// <summary>
	/// The maximum amount of items that can be in a stack.
	/// </summary>
	public int MaxAmount { get => _maxAmount; }

	public override string ToString()
	{
		return base.ToString() + string.Format("\nMax Stack Size: {0}", _maxAmount);
	}
}
