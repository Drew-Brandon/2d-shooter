using Godot;
using Godot.Collections;

/// <summary>
/// Stores information regarding casting with a ray.
/// </summary>
[GlobalClass]
public partial class RayCastInfo : CastInfo
{
	[Export]
	private bool _hitBackFaces = false;

	/// <summary>
	/// Whether or not the ray can hit back faces.
	/// </summary>
	public bool HitBackFaces { get => _hitBackFaces; }

	[Export]
	private bool _hitFromInside = false;

	/// <summary>
	/// Whether or not the ray can hit colliders from the inside.
	/// </summary>
	public bool HitFromInside { get => _hitFromInside; }

	/// <summary>
	/// Creates a query that can be used to test for collisions.
	/// </summary>
	/// <param name="from">
	/// The position that the ray will start from.
	/// </param>
	/// <param name="to">
	/// The position that the ray will end at.
	/// </param>
	/// <param name="exclude">
	/// What to exclude in the casting.
	/// </param>
	/// <returns>
	/// A query used to cast with.
	/// </returns>
	public PhysicsRayQueryParameters2D CreateQuery(Vector2 from, Vector2 to, Array<Rid> exclude = null)
	{
		PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(from, to, CollisionMask, exclude);
		query.CollideWithAreas = CollideWithAreas;
		query.CollideWithBodies = CollideWithBodies;
		query.HitFromInside = HitFromInside;
		return query;
	}

	/// <summary>
	/// Conducts an intersection with the specified query.
	/// </summary>
	/// <param name="node">
	/// The node that belongs to the world space to do the intersection in.
	/// </param>
	/// <param name="query">
	/// The query to do the intersection with.
	/// </param>
	/// <returns>
	/// The result of the intersection.
	/// </returns>
	public static RayCastResults IntersectRay(Node2D node, PhysicsRayQueryParameters2D query)
	{
		Dictionary results = node.GetWorld2D().DirectSpaceState.IntersectRay(query);
		return results.Count > 0 ? new RayCastResults(results) : new RayCastResults();
	}
}
