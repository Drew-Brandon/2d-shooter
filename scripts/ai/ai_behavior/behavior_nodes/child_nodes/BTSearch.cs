using Godot;
using System;

[GlobalClass]
public partial class BTSearch : BTNode
{
	[Export]
	private float _minSearchRadius;

	[Export]
	private float _maxSearchRadius;

	[Export]
	private BTVariable _targPos;

	protected override void OnEnter()
	{
		float angle = RandUtils.RandRangef(0f, 2 * Mathf.Pi);
		float radius = RandUtils.RandRangef(_minSearchRadius, _maxSearchRadius);
		Vector2 pos = BTPlyr.AI.GlobalPosition + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
		_targPos.SetVariable(BTPlyr.BB, pos);
		SuccessfulExit();
	}
}
