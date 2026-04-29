using Godot;
using System;

public partial class InventoryUI : Control
{
	[Export]
	private string _slotPathStr = "";

	private NodePath _slotPath = null;

	[Export]
	private Control _slotContainer;

	public override void _Ready()
	{
		_slotPath = new NodePath(_slotPathStr);
	}

	public ItemSlotUI GetSlot(int index)
	{
		return _slotContainer.GetChild(index).GetNode<ItemSlotUI>(_slotPath);
	}
}
