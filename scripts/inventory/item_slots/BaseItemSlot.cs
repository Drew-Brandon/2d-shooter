using Godot;

public partial class BaseItemSlot
{
	private ItemSlotUI _ui;
	protected ItemSlotUI UI { get => _ui; }

	protected BaseItemInstance InternalInstance = null;
	public BaseItemInstance Instance { get => InternalInstance; }
	public BaseItem Item { get => InternalInstance.GenericItem; }

	public BaseItemSlot(BaseItemInstance instance)
	{
		InternalInstance = instance;
	}

	public virtual bool HasInstance(BaseItemInstance instance)
	{
		return Item == instance.GenericItem;
	}

	protected virtual void OnUpdateUI()
	{

	}

	public void UpdateUI()
	{
		if (UI != null)
		{
			OnUpdateUI();
		}
	}

	public void ClearUI()
	{
		if (_ui != null)
		{
			_ui.ClearUI();
			_ui = null;
		}
	}

	public void SetUI(ItemSlotUI ui)
	{
		_ui = ui;
		_ui.SetIcon(InternalInstance.GenericItem.Icon);
		OnUpdateUI();
	}

	public void Free()
	{
		InternalInstance.Free();
		InternalInstance = null;
	}

	public partial interface IMergeable
	{
		public bool MergeWith(BaseItemInstance other)
		{
			return false;
		}

		public bool MergeWith(BaseItemSlot other)
		{
			bool isDone = MergeWith(other.InternalInstance);
			other.UpdateUI();
			return isDone;
		}

		public (bool, bool) ChargeWith(BaseItemInstance other)
		{
			return (false, false);
		}
	}
}
