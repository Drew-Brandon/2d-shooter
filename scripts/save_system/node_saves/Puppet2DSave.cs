using Godot;

public partial class Puppet2DSave : Node2DSave
{
	[Export]
	private float _health = 0f;
	public float Health { get => _health; }

	public Puppet2DSave()
	{ }

	public Puppet2DSave(Transform2D transform, float health) : base(transform)
	{
		_health = health;
	}
}
