using Godot;

/// <summary>
/// A resource that can create a stack based of the specified info.
/// </summary>
[GlobalClass]
public partial class ItemStackResource : Resource
{
	/// <summary>
	/// The amount of the stack.
	/// </summary>
	[Export]
	protected int Amount = 0;

	/// <summary>
	/// The item of the stack.
	/// </summary>
	[Export]
	protected Item Item = null;

	/// <summary>
	/// Gets the stack that has the data specified in this resource.
	/// </summary>
	/// <returns>
	/// The stack 
	/// </returns>
	public virtual ItemStack GetStack()
	{
		return new ItemStack(Amount, Item);
	}
}
