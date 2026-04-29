using Godot;
using System;

[GlobalClass]
public partial class BTSignal : BTNode
{
	[Signal]
	public delegate void OnEnterNodeEventHandler();

	protected override void OnEnter()
	{
		EmitSignal(SignalName.OnEnterNode);
		SuccessfulExit();
	}
}
