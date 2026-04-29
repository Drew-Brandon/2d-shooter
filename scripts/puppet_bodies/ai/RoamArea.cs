using Godot;
using System;

public partial class RoamArea : Node2D
{
	[Export]
	private Vector2 _size;

	public Vector2 NextPoint()
	{
		return RandUtils.RandVector(GlobalPosition, GlobalPosition + _size);
	}
}
