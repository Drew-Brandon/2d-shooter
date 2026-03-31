using Godot;

/// <summary>
/// The resource that can be used to construct GunStacks.
/// </summary>
[GlobalClass]
public partial class GunStackResource : ItemStackResource
{
	/// <summary>
	/// The amount of ammo within the clip.
	/// </summary>
	[Export]
	protected int ClipAmmo = 0;

	public override ItemStack GetStack()
	{
		return new GunStack(Amount, Item as GunItem, ClipAmmo);
	}
}
