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
	private float _damage = 10f;

	/// <summary>
	/// The amount of damage the gun should do normally.
	/// </summary>
	public float Damage { get => _damage; }

	[Export]
	private float _range = 4096f;

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

	[Export]
	private Texture2D _cursor = null;

	/// <summary>
	/// The cursor to use when the gun is equipped.
	/// </summary>
	public Texture2D Cursor { get => _cursor; }

	protected override void OnEquip()
	{
		Input.SetCustomMouseCursor(_cursor);
	}

	/// <summary>
	/// Fires the gun towards the specified target position and target body part.
	/// </summary>
	/// <param name="plyr">
	/// The player that fired the gun.
	/// </param>
	/// <param name="targPos">
	/// The target position to fire at.
	/// </param>
	/// <param name="targetPart">
	/// The body part to fire at.
	/// </param>
	protected void Fire(PlayerBody2D plyr, Vector2 targPos, BodyPart targetPart)
	{
		Vector2 start = plyr.GlobalPosition;
		Vector2 end = (targPos - start).LimitLength(_range) + start;
		PhysicsRayQueryParameters2D query = _rayCastInfo.CreateQuery(start, end, [plyr.GetRid()]);
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

	/// <summary>
	/// Aims the gun at the specified target.
	/// </summary>
	/// <param name="plyr">
	/// The player that fired the gun.
	/// </param>
	/// <param name="stack">
	/// The stack that the gun was fired from.
	/// </param>
	/// <param name="usePos">
	/// The position the gun will target.
	/// </param>
	/// <param name="targPart">
	/// The body part to target.
	/// </param>
	/// <returns>
	/// Whether or not the gun was succesfuly used.
	/// </returns>
	public virtual bool AimUse(PlayerBody2D plyr, ItemStack stack, Vector2 usePos, BodyPart targPart = BodyPart.Torso)
	{
		GunStack gunStack = (GunStack)stack;

		if (gunStack.CurClipAmmo > 0)
		{
			Fire(plyr, usePos, targPart);
			gunStack.CurClipAmmo--;
			return true;
		}

		return false;
	}

	public override bool Use(PlayerBody2D plyr, ItemStack stack, Vector2 usePos)
	{
		return AimUse(plyr, stack, usePos);
	}

	/// <summary>
	/// Reloads the gun.
	/// </summary>
	/// <param name="inventory">
	/// The inventory the gun is being reloaded in.
	/// </param>
	public void Reload(FiniteInventory inventory, GunStack stack)
	{
		int remainder = inventory.ChargeStack(new ItemStack(_maxClipAmmo - stack.CurClipAmmo, _ammoType));
		stack.CurClipAmmo = _maxClipAmmo - remainder;
	}
}
