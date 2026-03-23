using Godot;
using System;

[GlobalClass]
public partial class BTGetNode2DPosition : BTNode
{
	[Export]
	private BTVariable _pos;

	[Export]
	private BTVariable _node;

	protected override void OnEnter()
	{
		Node2D node = _node.GetVariable(BTPlyr.BB).As<Node2D>();
		_pos.SetVariable(BTPlyr.BB, node.GlobalPosition);
		SuccessfulExit();
	}
}
