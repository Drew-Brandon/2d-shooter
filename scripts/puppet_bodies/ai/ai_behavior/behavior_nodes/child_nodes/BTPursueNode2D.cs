using Godot;

[GlobalClass]
public partial class BTPursueNode2D : BTNode
{
	[Export]
	private BTVariable _node;

	protected override void OnProcess(float delta)
	{
		Node2D node = _node.GetVariable(BTPlyr.BB).As<Node2D>();
		BTPlyr.AI.TargetPosition = node.GlobalPosition;
	}

	protected override void OnStop()
	{
		BTPlyr.AI.TargetPosition = BTPlyr.AI.GlobalPosition;
	}
}
