using Godot;

public partial class RigidBody2DSave : Node2DSave
{
	[Export]
	private Vector2 _linearVel = Vector2.Zero;
	public Vector2 LinearVel { get => _linearVel; }

	[Export]
	private float _angularVel = 0f;
	public float AngularVel { get => _angularVel; }

	public RigidBody2DSave()
	{ }

	public RigidBody2DSave(Transform2D transform, Vector2 linearVel, float angularVel) : base(transform)
	{
		_linearVel = linearVel;
		_angularVel = angularVel;
	}
}
