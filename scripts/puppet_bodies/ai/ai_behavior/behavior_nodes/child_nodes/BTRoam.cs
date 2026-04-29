using Godot;
using System;

[GlobalClass]
public partial class BTRoam : BTNode
{
	[Export]
	private BTVariable _targPos;

	[Export]
	private BTVariable _area;

	protected override void OnEnter()
	{
		RoamArea area = _area.GetVariable(BTPlyr.BB).As<RoamArea>();
		Vector2 point = area.NextPoint();
		_targPos.SetVariable(BTPlyr.BB, point);
		SuccessfulExit();
	}
}
