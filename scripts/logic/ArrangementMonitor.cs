using Godot;

[GlobalClass]
public partial class ArrangementMonitor : Node
{
	private bool _isArranged = false;

	[Export]
	private float _angleMargin = 10f;

	[Export]
	private float _distMargin = 8f;

	[Export]
	private float[] _targetAngle = null;

	[Export]
	private Vector2[] _targetPos = null;

	[Export]
	private Node2D[] _nodes = null;

	[Signal]
	public delegate void OnCorrectlyArrangedEventHandler();

	public override void _Ready()
	{
		_angleMargin = Mathf.DegToRad(_angleMargin);
	}

	private void ArrangementCheck()
	{
		for (int i = 0; i < _nodes.Length; i++)
		{
			float dist = _nodes[i].GlobalPosition.DistanceTo(_targetPos[i]);
			float angleDisp = Mathf.Abs(_nodes[i].GlobalRotation - _targetAngle[i]);

			if (dist > _distMargin && angleDisp > _angleMargin)
			{
				return;
			}
		}

		if (!_isArranged)
		{
			_isArranged = true;
			EmitSignal(SignalName.OnCorrectlyArranged);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		ArrangementCheck();
	}
}
