using Godot;
using System;

public partial class GunItemSlot : ItemSlotGeneric<GunInstance>
{
	public int ClipAmmo { get => Instance.ClipAmmo; }

	public int MaxClipAmmo { get => Instance.Item.MaxClipAmmo; }

	public float Damage { get => Instance.Item.Damage; }

	public StackableItem AmmoType { get => Instance.Item.AmmoType; }

	public GunItemSlot(GunInstance instance) : base(instance)
	{ }

	protected override void OnUpdateUI()
	{
		UI.SetIntLabel(ClipAmmo);
	}

	public void ChargeClipAmmo(int amount)
	{
		Instance.ClipAmmo = Mathf.Clamp(ClipAmmo - amount, 0, MaxClipAmmo);
		UpdateUI();
	}

	public void ReloadClip(int amount)
	{
		Instance.ClipAmmo = Mathf.Clamp(ClipAmmo + amount, 0, MaxClipAmmo);
		UpdateUI();
	}
}
