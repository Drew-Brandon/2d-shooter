using Godot;
using System;

public partial class PatrolPath : Node2D
{
	[Export]
	private Vector2[] _points;

	public Vector2 GetPoint(int index)
	{
		return _points[index];
	}
}
