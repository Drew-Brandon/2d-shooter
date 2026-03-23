using Godot;

[GlobalClass]
public partial class BTParentNode : BTNode
{
	private int _curChild = 0;
	protected bool AtLastChild { get => _curChild == GetChildCount() - 1; }

	private int _curLoop = 0;

	[Export]
	private int _loopAmount = 0;

	protected bool NextLoop()
	{
		_curLoop = (_curLoop + 1) % (_loopAmount + 1);
		return _curLoop == 0;
	}

	protected void NextChild()
	{
		_curChild += 1;
		BTNode child = GetChild<BTNode>(_curChild);
		child.Enter();
	}

	protected virtual void OnChildSuccess()
	{
		
	}

	protected virtual void OnChildFailure()
	{

	}

	public override void _Ready()
	{
		base._Ready();

		for (int i = 0; i < GetChildCount(); i++)
		{
			BTNode child = GetChild<BTNode>(i);
			child.OnNodeSuccess += OnChildSuccess;
			child.OnNodeFailure += OnChildFailure;
		}
	}

	public override void _Process(double delta)
	{
		if (CheckQuery())
		{
			OnProcess((float)delta);
		}
		else
		{
			GetChild<BTNode>(_curChild).Stop();
			FailedExit();
		}
	}

	public override void SetPlayer(BTPlayer btPlyr)
	{
		base.SetPlayer(btPlyr);

		for (int i = 0; i < GetChildCount(); i++)
		{
			BTNode child = GetChild<BTNode>(i);
			child.SetPlayer(btPlyr);
		}
	}

	protected override void OnStop()
	{
		BTNode child = GetChild<BTNode>(_curChild);
		child.Stop();
		_curChild = 0;
		_curLoop = 0;
	}

	protected override void OnEnter()
	{
		_curLoop = 0;
		Reset();
	}

	protected void Reset()
	{
		_curChild = 0;
		BTNode child = GetChild<BTNode>(_curChild);
		child.Enter();
	}
}
