using Godot;
using System;

public partial class RelaySave : NodeSave
{
	[Export]
	private int _triggeredCount = 0;
	public int TriggeredCount { get => _triggeredCount; }

	public RelaySave()
	{ }

	public RelaySave(int triggeredCount)
	{
		_triggeredCount = triggeredCount;
	}
}
