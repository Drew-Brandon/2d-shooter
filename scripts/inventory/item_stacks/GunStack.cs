/// <summary>
/// Represents a stack/collection of guns.
/// </summary>
public partial class GunStack : ItemStack
{
	/// <summary>
	/// The amount of ammo currently in the gun's clip.
	/// </summary>
	public int CurClipAmmo = 0;

    /// <summary>
    /// The main constructor for GunStack.
    /// </summary>
    /// <param name="amount">
    /// The amount of items to be stored within this stack.
    /// </param>
    /// <param name="item">
    /// The item to be stored within this stack.
    /// </param>
    /// <param name="clipAmmo">
	/// The amount of ammo within the gun's clip.
	/// </param>
	public GunStack(int amount, GunItem item, int clipAmmo) : base(amount, item)
	{
		CurClipAmmo = clipAmmo;
	}
}
