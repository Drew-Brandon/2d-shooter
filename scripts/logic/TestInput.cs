using Godot;

[GlobalClass]
public partial class TestInput : Node
{
	[Export]
	private string[] _actions = null;

	[Signal]
	public delegate void OnActionJustPressedEventHandler(string action);

	public override void _Process(double delta)
	{
		if (_actions != null)
		{
			for (int i = 0; i < _actions.Length; i++)
			{
				if (Input.IsActionJustPressed(_actions[i]))
				{
					EmitSignal(SignalName.OnActionJustPressed, _actions[i]);
				}
			}
		}
	}
}
