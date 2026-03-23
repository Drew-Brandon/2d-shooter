using Godot;

/// <summary>
/// A UI element that represents an inventory slot.
/// </summary>
public partial class InventorySlotUI : Control
{
	private InventorySlot _slot;

	/// <summary>
	/// The slot that this UI is bound to.
	/// </summary>
	public InventorySlot Slot { get => _slot; set => _slot = value; }

	[Export]
	private Label _intLabel;

	[Export]
	private TextureRect _iconRect;

	[Export]
	private Control _previewNode;

	/// <summary>
	/// Clears the integer label to just be empty text.
	/// </summary>
	public void ClearIntLabel()
	{
		_intLabel.Text = "";
	}

	/// <summary>
	/// Sets the integer label to be the specified integer value.
	/// </summary>
	/// <param name="newInt">
	/// The integer value to set the label to.
	/// </param>
	public void SetIntLabel(int newInt)
	{
		_intLabel.Text = newInt.ToString();
	}

	/// <summary>
	/// Sets the texture to the specified item icon.
	/// </summary>
	/// <param name="icon">
	/// The icon of the item to display.
	/// </param>
	public void SetIcon(Texture2D icon)
	{
		_iconRect.Texture = icon;
	}

	public override Variant _GetDragData(Vector2 atPosition)
	{
		if (_slot.CurAmount > 0)
		{
			Control preview = _previewNode.Duplicate() as Control;
			SetDragPreview(preview);
			return this;
		}

		return new Variant();
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		InventorySlotUI src = data.As<Node>() as InventorySlotUI;
		return src != null;
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		InventorySlotUI src = data.As<InventorySlotUI>();
		src._slot.ExchangeSlot(_slot);
	}
}
