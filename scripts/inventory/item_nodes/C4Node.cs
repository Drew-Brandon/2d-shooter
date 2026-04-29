using Godot;
using System;

public partial class C4Node : ItemNode
{
	[Export]
	private PackedScene _c4Scene = null;

	[Signal]
	public delegate void OnEquippedEventHandler(int amount);


	public override void Equip(BaseItemSlot slot)
	{
		EmitSignal(SignalName.OnEquipped, ((StackableItemSlot)slot).Amount);
	}
}
