using Godot;

/// <summary>
/// The resource that can be used to construct GunStacks.
/// </summary>
[GlobalClass]
public partial class GunInstanceResource : BaseItemInstanceResource
{
	/// <summary>
	/// The amount of ammo within the clip.
	/// </summary>
	[Export]
	protected int ClipAmmo = 0;

	public override BaseItemInstance GetInstance()
	{
		return new GunInstance(ClipAmmo, (GunItem)Item);
	}
}
