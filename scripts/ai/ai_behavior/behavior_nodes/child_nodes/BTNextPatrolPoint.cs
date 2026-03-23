using Godot;
using System;

[GlobalClass]
public partial class BTNextPatrolPoint : BTNode
{
	[Export]
	private BTVariable _curPoint;

	[Export]
	private BTVariable _targPos;

	[Export]
	private BTVariable _patrolPath;

	protected override void OnEnter()
	{
		Node2D patrolPath = _patrolPath.GetVariable(BTPlyr.BB).As<Node2D>();
		int curPoint = _curPoint.GetVariable(BTPlyr.BB).AsInt32();
		int newPoint = (curPoint + 1) % patrolPath.GetChildCount();
		_curPoint.SetVariable(BTPlyr.BB, newPoint);

		Vector2 newPos = patrolPath.GetChild<Node2D>(newPoint).GlobalPosition;
		_targPos.SetVariable(BTPlyr.BB, newPos);
		SuccessfulExit();
	}
}
