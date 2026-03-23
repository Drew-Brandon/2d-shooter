using Godot;

[GlobalClass]
public partial class BTSequence : BTParentNode
{
	protected override void OnChildSuccess()
	{
		if (AtLastChild)
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
		else
		{
			NextChild();
		}
	}

	protected override void OnChildFailure()
	{
		FailedExit();
	}
}
