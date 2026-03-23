using Godot;
using Godot.Collections;
using System.Threading;

public partial class Gun : Weapon
{
	private bool _isReloading = false;

	[Export]
	private int _curClip = 0;

	[Export]
	private int _clipSize = 8;

	[Export]
	private float _range = 2048f;

	[Export]
	private double _reloadTime = 2d;

	private CancellationTokenSource _cts = new();

	[Export]
	private RayCastInfo _rayCast;

	[Export]
	private AudioStreamPlayer _firePlayer;

	[Export]
	private AudioStreamPlayer _reloadPlayer;

	[Export]
	private FiniteInventory _plyrInventory;

	[Export]
	private Item _ammoItem;

	/// <summary>
	/// Triggered when the current clip has a change in how much ammo it contains.
	/// </summary>
	/// <param name="clip">
	/// The amount of ammo left in the clip.
	/// </param>
	[Signal]
	public delegate void OnClipUpdateEventHandler(int clip);

	/// <summary>
	/// Triggered when the weapon is equipped.
	/// </summary>
	/// <param name="clipSize">
	/// The size of the current clip.
	/// </param>
	[Signal]
	public delegate void OnEquipEventHandler(int clipSize);

	/// <summary>
	/// Triggered when the total ammo available for the gun has changed.
	/// </summary>
	/// <param name="totAmmo">
	/// The total ammo available for the gun.
	/// </param>
	[Signal]
	public delegate void OnTotalAmmoUpdateEventHandler(int totAmmo);

	/// <summary>
	/// Cooldown function for reloading.
	/// </summary>
	private async void ReloadCooldown(CancellationTokenSource cts)
	{
		await ToSignal(GetTree().CreateTimer(_reloadTime), SceneTreeTimer.SignalName.Timeout);
		
		if (!cts.IsCancellationRequested)
		{
			int toRemove = _clipSize - _curClip;
			int remainder = _plyrInventory.RemoveStack(new ItemStack(toRemove, _ammoItem));
			_curClip += toRemove - remainder;
			EmitSignal(SignalName.OnClipUpdate, _curClip);
			_isReloading = false;
		}
	}

	/// <summary>
	/// Triggers the gun to raycast towards a specific position.
	/// </summary>
	/// <param name="to">
	/// The position to raycast towards.
	/// </param>
	/// <param name="bodyPart">
	/// The body part to target.
	/// </param>
	protected void RayCast(Vector2 to, BodyPart bodyPart = BodyPart.Torso)
	{
		Vector2 impactPos;
		PhysicsRayQueryParameters2D query = _rayCast.CreateQuery(GlobalPosition, to);
		Dictionary queryResults = RayCastInfo.IntersectRay(this, query);
		
		if (queryResults.Count > 0)
		{
			// Check if the hit target is an AI. If it is, then damage it.
			Node collider = queryResults["collider"].As<Node>();
			impactPos = queryResults["position"].AsVector2();
			AIBody2D ai = collider as AIBody2D;

			if (ai != null)
			{
				ai.DamagePart(bodyPart, _dmg);
			}
		}
		else
		{
			impactPos = to;
		}
	}

	public override void _Ready()
	{
		EmitSignal(SignalName.OnClipUpdate, _curClip);
		EmitSignal(SignalName.OnEquip, _clipSize);
	}

	public override void Attack(Vector2 target, BodyPart bodyPart = BodyPart.Torso)
	{
		if (_curClip > 0 && !_isReloading)
		{
			_firePlayer.Play();
			Vector2 to = GlobalPosition + (target - GlobalPosition).Normalized() * _range;
			RayCast(to, bodyPart);
			_curClip--;
			EmitSignal(SignalName.OnClipUpdate, _curClip);
		}
	}

	/// <summary>
	/// Reloads the gun.
	/// </summary>
	public void Reload()
	{
		if (_isReloading)
		{
			_cts.Cancel();
			_cts = new();
		}
		else if (_curClip < _clipSize)
		{
			_reloadPlayer.Play();
			ReloadCooldown(_cts);
		}

		_isReloading = !_isReloading;
	}
}
