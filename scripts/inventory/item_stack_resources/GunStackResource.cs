using Godot;

[GlobalClass]
public partial class GunStackResource : ItemStackResource
{
	[Export]
	protected int ClipAmmo = 0;

	public override ItemStack GetStack()
	{
		return new GunStack(Amount, Item as GunItem, ClipAmmo);
	}
}
