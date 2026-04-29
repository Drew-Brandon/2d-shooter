using Godot;

[GlobalClass]
public partial class BTMoveToPosition : BTNode
{
	[Export]
	private BTVariable _pos;

	protected override void OnEnter()
	{
		BTPlyr.AI.TargetPosition = _pos.GetVariable(BTPlyr.BB).As<Vector2>();
		WaitTillFinished();
	}

	protected override void OnStop()
	{
		BTPlyr.AI.TargetPosition = BTPlyr.AI.GlobalPosition;
	}

	private async void WaitTillFinished()
	{
		await ToSignal(BTPlyr.AI, AIBody2D.SignalName.OnNavigationFinished);

		if (Active)
		{
			SuccessfulExit();
		}
	}
}
