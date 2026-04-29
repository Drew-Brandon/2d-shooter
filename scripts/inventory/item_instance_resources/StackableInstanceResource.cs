using Godot;

/// <summary>
/// A resource that can create a stack based of the specified info.
/// </summary>
[GlobalClass]
public partial class StackableInstanceResource : BaseItemInstanceResource
{
	/// <summary>
	/// The amount of the stack.
	/// </summary>
	[Export]
	protected int Amount = 0;

	/// <summary>
	/// Gets the stack that has the data specified in this resource.
	/// </summary>
	/// <returns>
	/// The stack 
	/// </returns>
	public override BaseItemInstance GetInstance()
	{
		return new StackableInstance(Amount, (StackableItem)Item);
	}
}
