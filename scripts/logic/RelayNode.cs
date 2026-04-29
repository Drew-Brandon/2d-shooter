using Godot;

[GlobalClass]
public partial class RelayNode : Node, ISaveable
{
	[Export]
	private bool _retriggerOnSave = true;

	private int _triggeredCount = 0;
	public int TriggeredCount { get => _triggeredCount; }

	[Export]
	private int _maxTriggerCount = -1;

	[Signal]
	public delegate void OnTriggeredEventHandler();

	public void Trigger()
	{
		if (_maxTriggerCount != -1 && _triggeredCount >= _maxTriggerCount)
		{
			return;
		}

		_triggeredCount++;
		EmitSignal(SignalName.OnTriggered);
	}

	public NodeSave GetSave()
	{
		return new RelaySave(_triggeredCount);
	}

	public void LoadSave(NodeSave save)
	{
		RelaySave relaySave = save as RelaySave;
		
		if (relaySave == null)
		{
			return;
		}

		_triggeredCount = relaySave.TriggeredCount;
		
		if (_retriggerOnSave)
		{
			for (int i = 0; i < _triggeredCount; i++)
			{
				EmitSignal(SignalName.OnTriggered);
			}
		}
	}
}
