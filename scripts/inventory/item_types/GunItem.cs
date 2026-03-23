using Godot;

[GlobalClass]
public partial class GunItem : UseableItem
{
	[Export]
	private int _maxClipAmmo = 10;

	/// <summary>
	/// The maximum amount of ammo that can be in a single clip.
	/// </summary>
	public int MaxClipAmmo { get => _maxClipAmmo; }

	[Export]
	private float _range = 2048f;

	/// <summary>
	/// The range of the gun.
	/// </summary>
	public float Range { get => _range; }

	[Export]
	private Item _ammoType = null;

	/// <summary>
	/// The type of ammo to use for the gun.
	/// </summary>
	public Item AmmoType { get => _ammoType; }

	[Export]
	private RayCastInfo _rayCastInfo = null;
	
	/// <summary>
	/// The ray cast info to use for simulating the gun firing.
	/// </summary>
	public RayCastInfo RayCastInfo { get => _rayCastInfo; }

	public override bool Use(PlayerBody2D plyr, PlayerInventory inventory, ItemStack stack)
	{
		GunStack gunStack = (GunStack)stack;

		if (gunStack.CurClipAmmo > 0)
		{
			Vector2 start = plyr.GlobalPosition;
			Vector2 end = plyr.GlobalPosition + plyr.GlobalTransform.X * _range;
			_rayCastInfo.CreateQuery(start, end, [plyr.GetRid()]);
			gunStack.CurClipAmmo--;
			return true;
		}

		return false;
	}

	/// <summary>
	/// Reloads the gun.
	/// </summary>
	/// <param name="inventory">
	/// The inventory the gun is being reloaded in.
	/// </param>
	public void Reload(PlayerInventory inventory, GunStack stack)
	{
		inventory.ChargeStack(new ItemStack(_maxClipAmmo - stack.CurClipAmmo, _ammoType));
	}
}
