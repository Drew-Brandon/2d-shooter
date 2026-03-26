using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GunItem : UseableItem
{
	[Export]
	private float _damage = 10f;

	public float Damage { get => _damage; }

	[Export]
	private int _maxClipAmmo = 10;

	/// <summary>
	/// The maximum amount of ammo that can be in a single clip.
	/// </summary>
	public int MaxClipAmmo { get => _maxClipAmmo; }

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

	protected void Fire(PlayerBody2D plyr, Vector2 targPos, BodyPart targetPart)
	{
        Vector2 start = plyr.GlobalPosition;
        PhysicsRayQueryParameters2D query = _rayCastInfo.CreateQuery(start, targPos, [plyr.GetRid()]);
		RayCastResults results = RayCastInfo.IntersectRay(plyr, query);

		if (results.Collider != null)
		{
			AIBody2D ai = results.Collider as AIBody2D;

			if (ai != null)
			{
				ai.DamagePart(targetPart, _damage);
			}
		}
    }

	public virtual bool AimUse(PlayerBody2D plyr, FiniteInventory inventory, ItemStack stack, Vector2 usePos)
	{
        GunStack gunStack = (GunStack)stack;

        if (gunStack.CurClipAmmo > 0)
        {
            Fire(plyr, usePos, BodyPart.Torso);
            gunStack.CurClipAmmo--;
            return true;
        }

        return false;
    }

	public override bool Use(PlayerBody2D plyr, FiniteInventory inventory, ItemStack stack, Vector2 usePos)
	{
		return AimUse(plyr, inventory, stack, usePos);
	}

	/// <summary>
	/// Reloads the gun.
	/// </summary>
	/// <param name="inventory">
	/// The inventory the gun is being reloaded in.
	/// </param>
	public void Reload(FiniteInventory inventory, GunStack stack)
	{
		inventory.ChargeStack(new ItemStack(_maxClipAmmo - stack.CurClipAmmo, _ammoType));
	}
}
