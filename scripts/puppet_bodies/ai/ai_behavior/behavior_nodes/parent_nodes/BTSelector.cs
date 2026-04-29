using Godot;

[GlobalClass]
public partial class BTSelector : BTParentNode
{
	protected override void OnChildSuccess()
	{
		if (NextLoop())
		{
			SuccessfulExit();
		}
		else
		{
			Reset();
		}
	}

	protected override void OnChildFailure()
	{
		if (AtLastChild)
		{
			FailedExit();
		}
		else
		{
			NextChild();
		}
	}
}
