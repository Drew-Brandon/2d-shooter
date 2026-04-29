using Godot;

[GlobalClass]
public partial class GunItem : BaseItem
{
	[Export]
	private int _maxClipAmmo = 10;

	/// <summary>
	/// The maximum amount of ammo that can be in a single clip.
	/// </summary>
	public int MaxClipAmmo { get => _maxClipAmmo; }

	[Export]
	private float _damage = 10f;

	public float Damage { get => _damage; }

	[Export]
	private StackableItem _ammoType = null;

	/// <summary>
	/// The type of ammo to use for the gun.
	/// </summary>
	public StackableItem AmmoType { get => _ammoType; }

	public override string ToString()
	{
		return base.ToString() + string.Format("\nClip Size: {0}\nDamage: {1}\nAmmo Type: {2}",
			DisplayName, _maxClipAmmo, _damage, _ammoType.DisplayName);
	}
}
