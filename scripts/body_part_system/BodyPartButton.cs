using Godot;

public partial class BodyPartButton : BaseButton
{
	[Export]
	private BodyPart _bodyPart;
	public BodyPart BodyPart { get => _bodyPart; }

	[Signal]
	public delegate void OnHitEventHandler(int bodyPart);

	private void OnPress()
	{
		EmitSignal(SignalName.OnHit, (int)_bodyPart);
	}

	public override void _Ready()
	{
		Pressed += OnPress;
	}

	public bool ContainsPos(Vector2 pos)
	{
		return false;
	}
}
