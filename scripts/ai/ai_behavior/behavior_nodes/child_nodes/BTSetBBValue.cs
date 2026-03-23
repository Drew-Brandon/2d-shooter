using Godot;

[GlobalClass]
public partial class BTSetBBValue : BTNode
{
	[Export]
	private BTVariable _value;

	protected override void OnEnter()
	{
		_value.SetVariable(BTPlyr.BB, _value.Val);
		SuccessfulExit();
	}
}
