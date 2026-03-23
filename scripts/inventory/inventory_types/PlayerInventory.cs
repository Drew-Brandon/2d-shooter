using Godot;
using System;

public partial class PlayerInventory : FiniteInventory<InventorySlot>
{
	public void Equip(PlayerBody2D plyr, int index)
	{
		Slots[index].Equip(plyr);
	}

	public void Unequip(int index)
	{
		Slots[index].Unequip();
	}

	public void Use(PlayerBody2D plyr, int index)
	{
		Slots[index].Use(plyr, this);
	}

	public void Reload(int index)
	{
		Slots[index].Reload(this);
	}
}
