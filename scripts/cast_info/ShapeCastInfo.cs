using Godot;
using Godot.Collections;

/// <summary>
/// Stores information regarding casting with a shape.
/// </summary>
[GlobalClass]
public partial class ShapeCastInfo : CastInfo
{
	[Export]
	private Shape2D _shape;

	/// <summary>
	/// The shape to do the casting with.
	/// </summary>
	public Shape2D Shape { get => _shape; }

	/// <summary>
	/// Creates a query that can be used to test for collisions.
	/// </summary>
	/// <param name="transform">
	/// The transform of the shape.
	/// </param>
	/// <param name="exclude">
	/// What to exclude in the casting.
	/// </param>
	/// <returns>
	/// A query used to cast with.
	/// </returns>
	public PhysicsShapeQueryParameters2D CreateQuery(Transform2D transform, Array<Rid> exclude = null)
	{
		PhysicsShapeQueryParameters2D query = new();
		query.CollideWithAreas = CollideWithAreas;
		query.CollideWithBodies = CollideWithBodies;
		query.CollisionMask = CollisionMask;
		query.Transform = transform;
		query.Exclude = exclude;
		query.Shape = _shape;
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
	/// The results of the intersection.
	/// </returns>
	public static ShapeCastResults[] IntersectShape(Node2D node, PhysicsShapeQueryParameters2D query)
	{
		Array<Dictionary> results1 = node.GetWorld2D().DirectSpaceState.IntersectShape(query);

		if (results1 == null)
		{
			return null;
		}

		ShapeCastResults[] results2 = new ShapeCastResults[results1.Count];
		
		for (int i = 0; i < results1.Count; i++)
		{
            results2[i] = new ShapeCastResults
            {
                Shape = results1[i]["shape"].AsInt32(),
                ColliderRid = results1[i]["rid"].AsRid(),
                Collider = results1[i]["collider"].AsGodotObject()
            };
        }

		return results2;
	}
}
