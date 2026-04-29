using Godot;

public partial class StackableItemSlot : ItemSlotGeneric<StackableInstance>, BaseItemSlot.IMergeable
{
	public int Amount { get => Instance.Amount; }

	public int MaxAmount { get => Instance.Item.MaxAmount; }

	public StackableItemSlot(StackableInstance stack) : base(stack)
	{ }

	protected override void OnUpdateUI()
	{
		UI.SetIntLabel(Instance.Amount);
	}

	public override bool HasInstance(BaseItemInstance instance)
	{
		StackableInstance stack = instance as StackableInstance;
		return stack != null && stack.Item == Item && Amount >= stack.Amount;
	}

	public bool MergeWith(BaseItemInstance other)
	{
		StackableInstance otherStack = other as StackableInstance;

		if (otherStack != null && otherStack.Item == Item)
		{
			int newAmount = Mathf.Clamp(Instance.Amount + otherStack.Amount, 0, otherStack.Item.MaxAmount);
			otherStack.Amount -= newAmount - Instance.Amount;
			Instance.Amount = newAmount;
			UpdateUI();
			return otherStack.Amount <= 0;
		}

		return false;
	}

	public (bool, bool) ChargeWith(BaseItemInstance other)
	{
		StackableInstance otherStack = other as StackableInstance;

		if (otherStack != null)
		{
			int newAmount = Mathf.Clamp(Instance.Amount - otherStack.Amount, 0, otherStack.Item.MaxAmount);
			otherStack.Amount -= Instance.Amount - newAmount;
			Instance.Amount = newAmount;
			UpdateUI();
			return (otherStack.Amount <= 0, Instance.Amount <= 0);
		}

		return (false, false);
	}

	public void AddAmount(int toAdd)
	{
		Instance.Amount = Mathf.Clamp(Amount + toAdd, 0, MaxAmount);
	}
}
