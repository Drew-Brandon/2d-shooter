using Godot;

public partial class BaseItemInstance : GodotObject
{
	[Export]
	protected BaseItem InternalItem = null;
	public BaseItem GenericItem { get => InternalItem; }

	public BaseItemInstance()
	{
		InternalItem = null;
	}

	public BaseItemInstance(BaseItem item = null)
	{
		InternalItem = item;
	}

	public virtual BaseItemSlot GetSlot()
	{
		return new BaseItemSlot(this);
	}

	public virtual BaseItemSlot LoadIntoSlot()
	{
		return new BaseItemSlot(this);
	}
}
