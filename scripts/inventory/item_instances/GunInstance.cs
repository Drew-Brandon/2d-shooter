using Godot;

public partial class GunInstance : ItemInstanceGeneric<GunItem>
{
	/// <summary>
	/// The amount of ammo currently in the gun's clip.
	/// </summary>
	[Export]
	public int ClipAmmo = 0;

	public GunInstance() : base()
	{
		ClipAmmo = 0;
	}

	public GunInstance(int clipAmmo, GunItem item) : base(item)
	{
		ClipAmmo = clipAmmo;
	}

    public override BaseItemSlot GetSlot()
    {
		GunInstance instance = new(ClipAmmo, Item);
		return new GunItemSlot(instance);
    }

    public override BaseItemSlot LoadIntoSlot()
    {
        GunInstance instance = new(ClipAmmo, Item);
        return new GunItemSlot(instance);
    }
}
