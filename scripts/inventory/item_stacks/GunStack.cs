/// <summary>
/// Represents a stack/collection of guns.
/// </summary>
public partial class GunStack : ItemStack
{
	/// <summary>
	/// The amount of ammo currently in the gun's clip.
	/// </summary>
	public int CurClipAmmo = 0;

	public GunStack(int amount, GunItem item, int clipAmmo) : base(amount, item)
	{
		CurClipAmmo = clipAmmo;
	}
}
