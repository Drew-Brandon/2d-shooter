using Godot;

[GlobalClass]
public partial class BaseItemInstanceResource : Resource
{
	[Export]
	private BaseItem _item = null;
	protected BaseItem Item { get => _item; }

	public virtual BaseItemInstance GetInstance()
	{
		return new BaseItemInstance(_item);
	}
}
