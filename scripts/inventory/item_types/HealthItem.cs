using Godot;
using System;

[GlobalClass]
public partial class HealthItem : StackableItem
{
	[Export]
	private float _healAmount = 40f;
	public float HealAmount { get => _healAmount; }

	public override string ToString()
	{
		return base.ToString() + string.Format("\nHeal Amount: {0}", Mathf.FloorToInt(_healAmount));
	}
}
