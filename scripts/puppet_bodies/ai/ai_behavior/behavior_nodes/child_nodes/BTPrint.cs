using Godot;

[GlobalClass]
public partial class BTPrint : BTNode
{
	[Export]
	private BTVariable _msg;

	protected override void OnEnter()
	{
		GD.Print(_msg.GetVariable(BTPlyr.BB));
		SuccessfulExit();
	}
}
