using Godot;
using Godot.Collections;

/// <summary>
/// A UI element that represents an inventory slot.
/// </summary>
public partial class ItemSlotUI : Control
{
	private bool _isDragging = false;

	private FiniteInventory _inventory = null;
	public FiniteInventory Inventory { get => _inventory; set => _inventory = value; }

	[Export]
	private InventoryUIStatus _status = null;

	[Export]
	private PackedScene _hoverLabel = null;

	private HoverDisplay _curHoverLabel = null;

	[Export]
	private Control _slotRoot = null;

	[Export]
	private Label _intLabel = null;

	[Export]
	private TextureRect _iconRect = null;

	[Export]
	private PackedScene _previewScene = null;

	private TextureRect _curPreview = null;

	private int SlotIndex()
	{
		return _slotRoot.GetIndex();
	}

	private void OnStartHover()
	{
		if (!_status.IsDragging && _inventory.SlotValid(SlotIndex()))
		{
			string txt = _inventory.GetItemString(SlotIndex());
			_curHoverLabel = _hoverLabel.Instantiate<HoverDisplay>();
			GetCanvasLayerNode().GetChild(0).AddChild(_curHoverLabel);
			_curHoverLabel.SetText(txt);
		}
	}

	private void OnEndHover()
	{
		if (_curHoverLabel != null)
		{
			_curHoverLabel.QueueFree();
			_curHoverLabel = null;
		}
	}

	public override void _Ready()
	{
		MouseEntered += OnStartHover;
		MouseExited += OnEndHover;
	}

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

	public bool IsDragging()
	{
		return _curPreview != null;
	}

	public void ClearUI()
	{
		if (IsDragging())
		{
			GetViewport().GuiCancelDrag();
		}

		ClearIntLabel();
		SetIcon(null);
	}

	public override Variant _GetDragData(Vector2 atPosition)
	{
		if (_inventory.SlotValid(SlotIndex()))
		{
			if (_curHoverLabel != null)
			{
				_curHoverLabel.QueueFree();
				_curHoverLabel = null;
			}

			_status.IsDragging = true;
			_curPreview = _previewScene.Instantiate<TextureRect>();
			_curPreview.Texture = _iconRect.Texture;
			SetDragPreview(_curPreview);
			return this;
		}

		return new Variant();
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		return true;
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		_status.IsDragging = false;
		ItemSlotUI ui = data.As<ItemSlotUI>();
		ui._curPreview = null;
		FiniteInventory.ExchangeSlots(ui.Inventory, ui.SlotIndex(), _inventory, SlotIndex());
		OnStartHover();
	}
}
