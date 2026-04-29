using Godot;
using System.Collections;

public partial class ShotgunNode : GunNode
{
	[Export]
	private int _rayCount = 4;

	[Signal]
	public delegate void OnShellInsertEventHandler(int amount);

	protected override bool OnAimUse(PlayerBody2D plyr, GunItemSlot slot, Vector2 usePos, BodyPart targPart = BodyPart.Torso)
	{
		if (slot.ClipAmmo > 0 && !IsReloading && !OnCooldown)
		{
			for (int i = 0; i < _rayCount; i++)
			{
				Fire(plyr, slot.Damage, usePos, targPart);
			}

			slot.ChargeClipAmmo(1);
			EmitSignal(SignalName.OnFire, slot.ClipAmmo);
			FireCooldown();
			return true;
		}

		return false;
	}

	protected override IEnumerable OnReloadRoutine(FiniteInventory inventory, GunItemSlot slot, StackableInstance neededAmmo)
	{
		for (int i = 0; i < neededAmmo.Amount; i++)
		{
			StackableInstance singleShell = new(1, neededAmmo.Item);

			if (!inventory.HasInstance(singleShell))
			{
				break;
			}

			yield return ReloadTime;

			inventory.ChargeInstance(singleShell);

			if (singleShell.Amount != 1)
			{
				slot.ReloadClip(1);
				EmitSignal(SignalName.OnShellInsert, slot.ClipAmmo);
			}
			else
			{
				break;
			}
		}
	}
}
