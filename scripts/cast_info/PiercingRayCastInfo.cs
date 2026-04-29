using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class PiercingRayCastInfo : RayCastInfo
{
	[Export]
	private int _maxPierceCount = 0;

	[Export(PropertyHint.Layers2DPhysics)]
	private uint _pierceMask = 0;

	public uint PierceMask { get => _pierceMask; }

	public new PhysicsPiercingRayQueryParameters2D CreateQuery(Vector2 from, Vector2 to, Godot.Collections.Array<Rid> exclude = null)
	{
		PhysicsPiercingRayQueryParameters2D query = PhysicsPiercingRayQueryParameters2D.Create(from, to, CollisionMask, exclude);
		query.CollideWithAreas = CollideWithAreas;
		query.CollideWithBodies = CollideWithBodies;
		query.HitFromInside = HitFromInside;
		query.MaxPierceCount = _maxPierceCount;
		query.PierceMask = _pierceMask;
		return query;
	}

	public static RayCastResults[] IntersectRay(Node2D node, PhysicsPiercingRayQueryParameters2D query)
	{
		List<RayCastResults> resultsList = new();
		PhysicsPiercingRayQueryParameters2D curQuery = PhysicsPiercingRayQueryParameters2D.Create(
			query.From,
			query.To,
			query.CollisionMask,
			query.Exclude
		);

		curQuery.CollideWithAreas = query.CollideWithAreas;
		curQuery.CollideWithBodies = query.CollideWithBodies;
		curQuery.HitFromInside = query.HitFromInside;
		curQuery.MaxPierceCount = query.MaxPierceCount;
		curQuery.PierceMask = query.PierceMask;

		for (int i = 0; i != query.MaxPierceCount; i++)
		{
			RayCastResults results = RayCastInfo.IntersectRay(node, curQuery);
			CollisionObject2D col = results.Collider as CollisionObject2D;
			resultsList.Add(results);

			if (col == null || (col.CollisionLayer | query.PierceMask) == 0)
			{
				break;
			}

			curQuery.From = results.Position;
			curQuery.Exclude.Add(results.ColliderRid);
		}

		return resultsList.ToArray();
	}
}
