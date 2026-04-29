using Godot;

/// <summary>
/// A specific type of inventory meant to be used by players.
/// </summary>
public partial class PlayerInventory : FiniteInventory
{
	private int _equippedIndex = -1;

	/// <summary>
	/// The index that is currently equipped.
	/// </summary>
	public int EquippedIndex { get => _equippedIndex; }

	private BaseItemSlot _equippedSlot;

	private ItemNode _itemNode = null;

	[Export]
	private int _hotbarSize = 3;

	/// <summary>
	/// The number of slots in the inventory that makeup the hotbar,
	/// which is the group of slots that can be equipped.
	/// </summary>
	public int HotbarSize { get => _hotbarSize; }

	[Export]
	private PlayerBody2D _plyr = null;

	/// <summary>
	/// Equips the item slot of the specified index.
	/// </summary>
	/// <param name="index">
	/// The index of the slot to equip.
	/// </param>
	public void Equip(int index)
	{
		// If the slot is already equipped, then do not bother.
		if (index == _equippedIndex && _equippedSlot == Slots[_equippedIndex])
		{
			return;
		}

		// Unequip the previous slot.
		if (_itemNode != null)
		{
			_itemNode.Unequip(Slots[_equippedIndex]);
			_itemNode.QueueFree();
		}

		// Equip the new slot.
		_equippedIndex = index;

		if (Slots[_equippedIndex] != null)
		{
			_equippedSlot = Slots[_equippedIndex];

			// Create a brand new item node if this slot's item has a reference to one.
			if (_equippedSlot.Item.ItemScene != null)
			{
				_itemNode = _equippedSlot.Item.Equip(_plyr);
				_itemNode.Equip(Slots[_equippedIndex]);
			}
			else
			{
				_itemNode = null;
			}
		}
		else
		{
            _equippedSlot = null;
			_itemNode = null;
		}
	}

	/// <summary>
	/// Uses the equipped slot on the target position.
	/// </summary>
	/// <param name="target">
	/// The position to use the slot on.
	/// </param>
	public void UseSlot(Vector2 target)
	{
		// Make sure a useable slot is equipped.
		if (_equippedSlot != null && _equippedSlot.Item.ItemScene != null)
		{
			_itemNode.Use(_plyr, this, Slots[_equippedIndex], target);
		}
	}

    /// <summary>
    /// Aims the use of the specified gun slot to target a body part.
    /// </summary>
    /// <param name="target">
    /// The position to use the slot on.
    /// </param>
    /// <param name="bodyPart">
	/// The body part to target.
	/// </param>
    public void AimUseSlot(Vector2 target, BodyPart bodyPart)
	{
		// Make sure the currently equipped item is a gun.
		GunNode gun = _itemNode as GunNode;

		if (gun != null)
		{
			gun.AimUse(_plyr, (GunItemSlot)Slots[_equippedIndex], target, bodyPart);
		}
	}

	/// <summary>
	/// Reloads the currently equipped gun slot.
	/// </summary>
	public void Reload()
	{
        // Make sure the currently equipped item is a gun.
        GunNode gun = _itemNode as GunNode;

		if (gun != null)
		{
			gun.Reload(this, (GunItemSlot)Slots[_equippedIndex]);
		}
	}
}
