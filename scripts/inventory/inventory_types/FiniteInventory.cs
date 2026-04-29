using Godot;
using System;

/// <summary>
/// Represents an inventory with a finite/fixed size.
/// </summary>
public partial class FiniteInventory : Node, ISaveable
{
	private bool _loaded = false;

	private int _filledSlots = 0;

	[Export]
	private int _size = 16;
	public int Size { get => _size; }

	[Export]
	private BaseItemInstanceResource[] _startInstances = null;

	protected BaseItemSlot[] Slots = null;

	protected ItemSlotUI[] SlotsUI = null;

	[Signal]
	public delegate void OnEmptiedEventHandler();

	[Signal]
	public delegate void OnInstanceRemovedEventHandler();

	[Signal]
	public delegate void OnInstanceAddedEventHandler();

	[Signal]
	public delegate void OnFilledEventHandler();

	private void DecrementSlot()
	{
		_filledSlots--;
		EmitSignal(SignalName.OnInstanceRemoved);

		if (_filledSlots == 0)
		{
			EmitSignal(SignalName.OnEmptied);
		}
	}

	private void IncrementSlot()
	{
		EmitSignal(SignalName.OnInstanceAdded);

		if (_filledSlots == 0)
		{
			EmitSignal(SignalName.OnFilled);
		}

		_filledSlots++;
	}

	private void AddStartInstances()
	{
		if (_startInstances != null)
		{
			for (int i = 0; i < _startInstances.Length; i++)
			{
				AddInstance(_startInstances[i].GetInstance());
			}

			Array.Clear(_startInstances);
		}
	}

	public override void _Ready()
	{
		Slots = new BaseItemSlot[_size];
		SlotsUI = null;

		if (!_loaded)
		{
			AddStartInstances();
		}
	}
	
	protected virtual void OnSlotChanged(int index)
	{

	}

	public void EmptySlot(int index)
	{
		Slots[index].Free();
		Slots[index].ClearUI();
		Slots[index] = null;
		OnSlotChanged(index);
		DecrementSlot();
	}

	/// <summary>
	/// Adds the specified item instance to the inventory.
	/// </summary>
	/// <param name="toAdd">
	/// The instance to add.
	/// </param>
	public void AddInstance(BaseItemInstance toAdd)
	{
		for (int i = 0; i < _size; i++)
		{
			BaseItemSlot.IMergeable mergeable;

			// If the slot is null/empty, then assign it to store the added instance.
			if (Slots[i] == null)
			{
				Slots[i] = toAdd.GetSlot();

				if (SlotsUI != null)
				{
					Slots[i].SetUI(SlotsUI[i]);
				}

				IncrementSlot();

				// If the slot is not mergeable, then the instance has in effect already been added.
				mergeable = Slots[i] as BaseItemSlot.IMergeable;

				if (mergeable == null)
				{
					OnSlotChanged(i);
					break;
				}
			}
			else
			{
				mergeable = Slots[i] as BaseItemSlot.IMergeable;

				if (mergeable == null)
				{
					continue;
				}
			}

			// If the slot is mergeable, than attempt to merge it with the instance.
			bool isDone = mergeable.MergeWith(toAdd);
			OnSlotChanged(i);

			if (isDone)
			{
				break;
			}
		}
	}

	/// <summary>
	/// Charges the inventory with the specified item instance.
	/// </summary>
	/// <param name="toCharge">
	/// The instacne to charge the inventory with.
	/// </param>
	public bool ChargeInstance(BaseItemInstance toCharge)
	{
		for (int i = 0; i < _size; i++)
		{
			if (Slots[i] == null || Slots[i].Item != toCharge.GenericItem)
			{
				continue;
			}

			BaseItemSlot.IMergeable mergeable = Slots[i] as BaseItemSlot.IMergeable;

			// Make sure the slot is mergeable, as that is the only kind that can be charged.
			if (mergeable != null)
			{
				var (isDone, slotEmpty) = mergeable.ChargeWith(toCharge);
				OnSlotChanged(i);

				if (slotEmpty)
				{
					EmptySlot(i);
				}

				if (isDone)
				{
					return true;
				}
			}
			else
			{
				EmptySlot(i);
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Checks whether or not the instance is contained within the inventory.
	/// This acts like ChargeInstance, only without instances being removed.
	/// </summary>
	/// <param name="instance">
	/// The instance to check for.
	/// </param>
	/// <returns>
	/// Whethe the inventory contains the instance or not.
	/// </returns>
	public bool HasInstance(BaseItemInstance instance)
	{
		for (int i = 0; i < _size; i++)
		{
			if (Slots[i] != null && Slots[i].HasInstance(instance))
			{
				return true;
			}
		}

		return false;
	}

	public NodeSave GetSave()
	{
		BaseItemInstance[] instances = new BaseItemInstance[Slots.Length];

		for (int i = 0; i < Slots.Length; i++)
		{
			if (Slots[i] == null)
			{
				instances[i] = null;
			}
			else
			{
				instances[i] = Slots[i].Instance;
			}
		}

		return new InventorySave(instances);
	}

	public void LoadSave(NodeSave save)
	{
		_loaded = true;
		InventorySave invenSave = save as InventorySave;

		if (invenSave == null)
		{
			return;
		}

		for (int i = 0; i < invenSave.GetInstanceCount(); i++)
		{
			if (invenSave.GetInstance(i) == null)
			{
				if (Slots[i] != null)
				{
					EmptySlot(i);
				}
			}
			else
			{
				Slots[i] = invenSave.GetInstance(i).LoadIntoSlot();

				if (SlotsUI != null)
				{
					Slots[i].SetUI(SlotsUI[i]);
				}
			}
		}
	}

	/// <summary>
	/// Checks if the slot at the specified index is valid or not.
	/// </summary>
	/// <param name="index">
	/// The index to check.
	/// </param>
	/// <returns>
	/// Whether or not the slot is valid.
	/// </returns>
	public bool SlotValid(int index)
	{
		return Slots[index] != null;
	}

	public string GetItemString(int index)
	{
		return Slots[index].Item.ToString();
	}

	/// <summary>
	/// Sets the UI for this inventory.
	/// </summary>
	/// <param name="ui">
	/// The UI to set to.
	/// </param>
	public void SetUI(InventoryUI ui)
	{
		SlotsUI = new ItemSlotUI[_size];

		for (int i = 0; i < _size; i++)
		{
			SlotsUI[i] = ui.GetSlot(i);
			SlotsUI[i].Inventory = this;

			if (Slots[i] != null)
			{
				Slots[i].SetUI(SlotsUI[i]);
			}
		}
	}

	/// <summary>
	/// Clears the UI for this inventory.
	/// </summary>
	public void ClearUI()
	{
		for (int i = 0; i < SlotsUI.Length; i++)
		{
			if (Slots[i] != null)
			{
				Slots[i].ClearUI();
			}
		}

		SlotsUI = null;
	}

	/// <summary>
	/// Stops the dragging process for any slots in this inventory.
	/// Usually useful for when an inventory is closed.
	/// </summary>
	public void StopDrag()
	{
		for (int i = 0; i < SlotsUI.Length; i++)
		{
			if (SlotsUI[i].IsDragging())
			{
				SlotsUI[i].GetViewport().GuiCancelDrag();
				break;
			}
		}
	}

	/// <summary>
	/// Exchanges slots between the two inventories.
	/// </summary>
	/// <param name="from">
	/// The inventory to move the slot from.
	/// </param>
	/// <param name="fromIndex">
	/// The index of the slot to move.
	/// </param>
	/// <param name="to">
	/// The inventory to move the slot to.
	/// </param>
	/// <param name="toIndex">
	/// The index of the slot to move to.
	/// </param>
	public static void ExchangeSlots(FiniteInventory from, int fromIndex, FiniteInventory to, int toIndex)
	{
		// If the slots are the same, then don't bother to do the rest.
		BaseItemSlot fromSlot = from.Slots[fromIndex], toSlot = to.Slots[toIndex];

		if (fromSlot == toSlot)
		{
			return;
		}

		/*
		 * If the slot being exchanged to is mergeable,
		 * then merge it with the slot being exchanged if possible.
		 * Otherwise, swap the slots.
		 */
		BaseItemSlot.IMergeable mergeable = toSlot as BaseItemSlot.IMergeable;

		if (fromSlot != null && toSlot != null && mergeable != null && fromSlot.Item == toSlot.Item)
		{
			bool isDone = mergeable.MergeWith(fromSlot);

			if (isDone)
			{
				from._filledSlots--;
				fromSlot.ClearUI();
				from.Slots[fromIndex] = null;
			}
		}
		else
		{
			from.Slots[fromIndex] = toSlot;

			if (toSlot == null)
			{
				if (fromSlot != null)
				{
					from.DecrementSlot();
				}

				from.SlotsUI[fromIndex].ClearUI();
			}
			else
			{
				if (fromSlot == null)
				{
					from.IncrementSlot();
				}

				toSlot.SetUI(from.SlotsUI[fromIndex]);
				toSlot.UpdateUI();
			}

			to.Slots[toIndex] = fromSlot;

			if (fromSlot == null)
			{
				if (toSlot != null)
				{
					to.DecrementSlot();
				}

				to.SlotsUI[toIndex].ClearUI();
			}
			else
			{
				if (toSlot == null)
				{
					to.IncrementSlot();
				}

				fromSlot.SetUI(to.SlotsUI[toIndex]);
				fromSlot.UpdateUI();
			}
		}

		// If the player had equipped one of the slots, then equip the new slot.
		PlayerInventory plyrFrom = from as PlayerInventory;

		if (plyrFrom != null && plyrFrom.EquippedIndex == fromIndex)
		{
			plyrFrom.Equip(plyrFrom.EquippedIndex);
		}

		PlayerInventory plyrTo = to as PlayerInventory;

		if (plyrTo != null && plyrTo.EquippedIndex == toIndex)
		{
			plyrTo.Equip(plyrTo.EquippedIndex);
		}
	}
}
