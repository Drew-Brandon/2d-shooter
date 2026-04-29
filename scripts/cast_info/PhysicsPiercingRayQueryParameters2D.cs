using Godot;
using Godot.Collections;
using System;

public partial class PhysicsPiercingRayQueryParameters2D : PhysicsRayQueryParameters2D
{
	public int MaxPierceCount = 0;

	public uint PierceMask = 0;

	public static new PhysicsPiercingRayQueryParameters2D Create(Vector2 from, Vector2 to, uint collisionMask, Array<Rid> exclude = null)
	{
		PhysicsPiercingRayQueryParameters2D query = new();
		query.From = from;
		query.To = to;
		query.CollisionMask = collisionMask;
		query.Exclude = exclude;

		return query;
	}
}
