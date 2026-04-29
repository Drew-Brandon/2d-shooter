using Godot;

public partial class BodyPartButton : BaseButton
{
	private bool _mouseOver = false;

	[Export]
	private BodyPart _bodyPart;
	public BodyPart BodyPart { get => _bodyPart; }

	public Vector2 RelativePosition { get => Position; }

	[Signal]
	public delegate void OnHitEventHandler(int bodyPart);
}
