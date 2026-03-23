using Godot;
using System;

public partial class PlayerInventorySlot : InventorySlot
{
	private Control _itemDisplayUI = null;

	public void Equip(PlayerBody2D plyr)
	{
		UseableItem useableItem = (UseableItem)Stack.CurItem;
		_itemDisplayUI = useableItem.Equip(plyr);
	}

	public void Unequip()
	{
		_itemDisplayUI.QueueFree();
	}

	public void Use(PlayerBody2D plyr, PlayerInventory inventory)
	{
		UseableItem useableItem = (UseableItem)Stack.CurItem;
		useableItem.Use(plyr, inventory, Stack);
	}

	public void Reload(PlayerInventory inventory)
	{
		GunItem gunItem = (GunItem)Stack.CurItem;
		GunStack guntStack = (GunStack)Stack;
		gunItem.Reload(inventory, guntStack);
	}
}
