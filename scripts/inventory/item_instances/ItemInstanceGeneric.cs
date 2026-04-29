using Godot;
using System;

public partial class ItemInstanceGeneric<T> : BaseItemInstance where T : BaseItem
{
	public T Item { get => (T)InternalItem; set => InternalItem = value; }

	public ItemInstanceGeneric() : base()
	{ }

	public ItemInstanceGeneric(T item) : base(item)
	{ }

	public override BaseItemSlot GetSlot()
	{
		ItemInstanceGeneric<T> instance = new(Item);
		return new ItemSlotGeneric<ItemInstanceGeneric<T>>(instance);
	}

	public override BaseItemSlot LoadIntoSlot()
	{
		ItemInstanceGeneric<T> instance = new(Item);
		return new ItemSlotGeneric<ItemInstanceGeneric<T>>(instance);
	}
}
