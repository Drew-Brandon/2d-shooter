using Godot;
using System;

public partial class ItemSlotGeneric<T> : BaseItemSlot where T : BaseItemInstance
{
	protected T Instance { get => (T)InternalInstance; set => InternalInstance = value; }

	public ItemSlotGeneric(T instance) : base(instance)
	{ }
}
