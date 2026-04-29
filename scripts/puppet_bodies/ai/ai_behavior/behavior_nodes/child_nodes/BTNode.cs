using Godot;

[GlobalClass]
public partial class BTNode : Node
{
	private bool _active = false;
	public bool Active { get => _active; }

	private BTPlayer _btPlyr;
	protected BTPlayer BTPlyr { get => _btPlyr; }

	[Export]
	private BTVariable _bbQuery;
	
	[Signal]
	public delegate void OnNodeSuccessEventHandler();

	[Signal]
	public delegate void OnNodeFailureEventHandler();

	protected bool CheckQuery()
	{
		return _bbQuery == null || _bbQuery.VariableMatch(_btPlyr.BB);
	}

	public BTPlayer GetPlayer()
	{
		return _btPlyr;
	}

	public virtual void SetPlayer(BTPlayer btPlyr)
	{
		_btPlyr = btPlyr;
	}

	public override void _Ready()
	{
		SetProcess(false);
		SetPhysicsProcess(false);
	}

	public void Enter()
	{
		if (CheckQuery())
		{
			_active = true;
			SetProcess(true);
			SetPhysicsProcess(true);
			OnEnter();
		}
		else
		{
			FailedExit();
		}
	}

	public void Stop()
	{
		_active = false;
		SetProcess(false);
		SetPhysicsProcess(false);
		OnStop();
	}

	protected virtual void OnStop()
	{

	}

	protected virtual void OnEnter()
	{

	}

	public void SuccessfulExit()
	{
		_active = false;
		SetProcess(false);
		SetPhysicsProcess(false);
		OnSuccessfulExit();
		EmitSignal(SignalName.OnNodeSuccess);
	}

	public void FailedExit()
	{
		_active = false;
		SetProcess(false);
		SetPhysicsProcess(false);
		OnFailedExit();
		EmitSignal(SignalName.OnNodeFailure);
	}

	protected virtual void OnSuccessfulExit()
	{

	}

	protected virtual void OnFailedExit()
	{

	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (CheckQuery())
		{
			OnProcess((float)delta);
		}
		else
		{
			FailedExit();
		}
	}

	protected virtual void OnProcess(float delta)
	{

	}
}
