using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class GunNode : ItemNode
{
	private bool _isReloading = false;

	private bool _onCooldown = false;

	public bool OnCooldown { get => _onCooldown; }

	/// <summary>
	/// Whether or not the gun is currently reloading.
	/// </summary>
	public bool IsReloading { get => _isReloading; }

	[Export]
	protected int ReloadTime = 2000;

	[Export]
	private int _fireRate = 500;

	[Export]
	private float _range = 4096f;

	[Export]
	private float _bulletSpread = 2f;

	[Export]
	private float _pierceDecay = 0.75f;

	private CoroutineSource _reloadSrc = new();

	[Export]
	private PackedScene _bulletTracerScene = null;

	[Export]
	private PiercingRayCastInfo _rayCastInfo = null;

	[Signal]
	public delegate void OnEquippedEventHandler(int ammo);

	[Signal]
	public delegate void OnFireEventHandler(int ammo);

	[Signal]
	public delegate void OnStartReloadEventHandler();

	[Signal]
	public delegate void OnHaltReloadEventHandler();

	[Signal]
	public delegate void OnFinishReloadEventHandler(int ammo);

	/// <summary>
	/// Fires the gun. Calling this method does not affect the current ammo clip.
	/// </summary>
	/// <param name="plyr">
	/// The player shooting the gun.
	/// </param>
	/// <param name="dmg">
	/// The amount of damage to deal.
	/// </param>
	/// <param name="targPos">
	/// The target position to shoot at.
	/// </param>
	/// <param name="targetPart">
	/// The target body part to shoot at.
	/// </param>
	protected void Fire(PlayerBody2D plyr, float dmg, Vector2 targPos, BodyPart targetPart)
	{
		// Get the start and end points for the raycast.
		Vector2 start = plyr.GlobalPosition;
		float angleTo = start.AngleToPoint(targPos);
		Vector2 dir = RandUtils.RandDir(angleTo - _bulletSpread, angleTo + _bulletSpread);
		Vector2 end = start + dir * _range;

		// Perform the raycast.
		PhysicsPiercingRayQueryParameters2D query = _rayCastInfo.CreateQuery(start, end, [plyr.GetRid()]);
		RayCastResults[] results = PiercingRayCastInfo.IntersectRay(plyr, query);

		// Create the tracer effect for the gunfire.
		Line2D tracer = _bulletTracerScene.Instantiate<Line2D>();
		GetTree().Root.AddChild(tracer);

		// Go through each result of the raycast, and check if any AI's can be damaged.
		if (results.Length > 0)
		{
			for (int i = 0; i < results.Length; i++)
			{
				AIBody2D ai = results[i].Collider as AIBody2D;

				if (ai != null)
				{
					ai.DamagePart(plyr, targetPart, dmg);
				}
				else
				{
					IDamageable damageable = results[i].Collider as IDamageable;

					if (damageable != null)
					{
						damageable.AddHealth(plyr, -dmg);
					}
				}

				// Reduce the damage every time the bullet pierces through something.
				dmg *= _pierceDecay;
			}

			tracer.Points = [start, results[results.Length - 1].Position];
		}
		else
		{
			tracer.Points = [start, end];
		}
	}

	public override void _Ready()
	{
		_bulletSpread = Mathf.DegToRad(_bulletSpread);
	}

    /// <summary>
    /// Aims and uses/fires the gun at the specified body part.
    /// </summary>
    /// <param name="plyr">
    /// The player using the gun.
    /// </param>
    /// <param name="slot">
    /// The slot that the gun is being fired with/from.
    /// </param>
    /// <param name="usePos">
    /// The position to fire at.
    /// </param>
    /// <param name="targPart">
    /// The target body part to aim for.
    /// </param>
    /// <returns>
    /// Whether or not the gun was successfuly fired.
    /// </returns>
    public bool AimUse(PlayerBody2D plyr, GunItemSlot slot, Vector2 usePos, BodyPart targPart = BodyPart.Torso)
	{
		if (CanUse)
		{
			return OnAimUse(plyr, slot, usePos, targPart);
		}

		return false;
	}

    protected virtual bool OnAimUse(PlayerBody2D plyr, GunItemSlot slot, Vector2 usePos, BodyPart targPart = BodyPart.Torso)
	{
		if (slot.ClipAmmo > 0 && !_isReloading && !_onCooldown)
		{
			Fire(plyr, slot.Damage, usePos, targPart);
			slot.ChargeClipAmmo(1);
			EmitSignal(SignalName.OnFire, slot.ClipAmmo);
			FireCooldown();
			return true;
		}

		return false;
	}

	protected override bool OnUse(PlayerBody2D plyr, PlayerInventory inventory, BaseItemSlot slot, Vector2 usePos)
	{
		return AimUse(plyr, (GunItemSlot)slot, usePos);
	}

	protected async void FireCooldown()
	{
		_onCooldown = true;
		await Task.Delay(_fireRate);
		_onCooldown = false;
	}

	/// <summary>
	/// Reloads the gun.
	/// </summary>
	/// <param name="inventory">
	/// The inventory that the gun is from.
	/// </param>
	/// <param name="slot">
	/// The slot that the gun is from.
	/// </param>
	public void Reload(FiniteInventory inventory, GunItemSlot slot)
	{
		// If we were already reloading, then stop reloading.
		if (_isReloading)
		{
			_isReloading = false;
			_reloadSrc.IsCancelled = true;
			_reloadSrc = new CoroutineSource();
            EmitSignal(SignalName.OnHaltReload);
		}
		else
		{
			_isReloading = true;
			int neededAmount = slot.MaxClipAmmo - slot.ClipAmmo;
			StackableInstance neededAmmo = new StackableInstance(neededAmount, slot.AmmoType);
            _reloadSrc.StartCoroutine(ReloadRoutine(inventory, slot, neededAmmo));
			EmitSignal(SignalName.OnStartReload);
		}
	}

	private IEnumerable ReloadRoutine(FiniteInventory inventory, GunItemSlot slot, StackableInstance neededAmmo)
	{
		foreach (int ms in OnReloadRoutine(inventory, slot, neededAmmo))
		{
			yield return ms;
		}

		_isReloading = false;
        EmitSignal(SignalName.OnFinishReload, slot.ClipAmmo);
    }

	protected virtual IEnumerable OnReloadRoutine(FiniteInventory inventory, GunItemSlot slot, StackableInstance neededAmmo)
	{
        yield return ReloadTime;
        int prevAmount = neededAmmo.Amount;
        inventory.ChargeInstance(neededAmmo);

        if (neededAmmo.Amount != prevAmount)
        {
            slot.ReloadClip(prevAmount - neededAmmo.Amount);
        }
    }

	private IEnumerable HolsterCooldown()
	{
		CanUse = false;
		yield return _fireRate;
		CanUse = true;
	}

	public override void Equip(BaseItemSlot slot)
	{
		CoroutineSource.StartSovereignCoroutine(HolsterCooldown());
		EmitSignal(SignalName.OnEquipped, ((GunItemSlot)slot).ClipAmmo);
	}

    public override void Unequip(BaseItemSlot slot)
    {
		_reloadSrc.IsCancelled = true;
    }
}
