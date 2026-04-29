using Godot;

[GlobalClass]
public partial class ValueEqual : Node
{
	[Export]
	private Variant _value = new();

	[Signal]
	public delegate void OnValueNotEquivalentEventHandler();

	[Signal]
	public delegate void OnValueEquivalentEventHandler();

	public void CheckValue(Variant toCheck)
	{
		if (_value.Obj != null && toCheck.Obj != null && _value.Obj.Equals(toCheck.Obj))
		{
			EmitSignal(SignalName.OnValueEquivalent);
		}
		else
		{
			EmitSignal(SignalName.OnValueNotEquivalent);
		}
	}
}
