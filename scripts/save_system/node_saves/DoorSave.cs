using Godot;

public partial class DoorSave : RigidBody2DSave
{
	[Export]
	private bool _isLocked = false;
	public bool IsLocked { get => _isLocked; }

	public DoorSave()
	{ }

	public DoorSave(Transform2D transform, Vector2 linearVel, float angularVel, bool isLocked) : base(transform, linearVel, angularVel)
	{
		_isLocked = isLocked;
	}
}
