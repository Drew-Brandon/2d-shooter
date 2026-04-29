using Godot;

public partial class Node2DSave : NodeSave
{
	[Export]
	private Transform2D _transform = new();
	public Transform2D Transform { get => _transform; }

	public Node2DSave()
	{ }

	public Node2DSave(Transform2D transform)
	{
		_transform = transform;
	}
}
