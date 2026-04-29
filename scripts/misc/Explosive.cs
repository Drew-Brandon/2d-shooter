using Godot;
using System;

/// <summary>
/// Represents a node that can be exploded.
/// </summary>
public partial class Explosive : Node2D, IDamageable
{
	private bool _hasExploded = false;

	[Export]
	private float _maxDamage = 100f;

	private float _curHealth = 0f;

	[Export]
	private float _maxHealth = 20f;

	[Export]
	private float _range = 256f;

	[Export]
	private PackedScene _afterEffect;

	[Export]
	private RayCastInfo _sightCast = null;

	[Export]
	private ShapeCastInfo _rangeCast = null;

	[Export]
	private CollisionObject2D[] _ignore;

	private Godot.Collections.Array<Rid> _ignoreRid;

	public override void _Ready()
	{
		_ignoreRid = new Godot.Collections.Array<Rid>();

		for (int i = 0; i < _ignore.Length; i++)
		{
			_ignoreRid.Add(_ignore[i].GetRid());
		}

		Array.Clear(_ignore);
	}

	public void AddHealth(Node2D instigator, float amount)
	{
		_curHealth = Mathf.Clamp(_curHealth + amount, 0f, _maxHealth);

		if (_curHealth <= 0f)
		{
			Explode();
		}
	}

	/// <summary>
	/// Explodes the node.
	/// </summary>
	public void Explode()
	{
		if (_hasExploded)
		{
			return;
		}

		// Scan for all nodes within the explosion's range.
		_hasExploded = true;
		PhysicsShapeQueryParameters2D rangeQuery = _rangeCast.CreateQuery(GlobalTransform, _ignoreRid);
		ShapeCastResults[] rangeResults = ShapeCastInfo.IntersectShape(this, rangeQuery);

		for (int i = 0; i < rangeResults.Length; i++)
		{
			Node2D node = rangeResults[i].Collider as Node2D;
            IDamageable damageable = node as IDamageable;

			if (damageable != null)
			{
				// Check if the node is in the explosion's line of sight.
				PhysicsRayQueryParameters2D sightQuery = _sightCast.CreateQuery(GlobalPosition, node.GlobalPosition);
				RayCastResults sightResults = RayCastInfo.IntersectRay(this, sightQuery);

				if (sightResults.Collider == node)
				{
					float distTo = GlobalPosition.DistanceTo(node.GlobalPosition);
					damageable.AddHealth(this, (1f - distTo / _range) * -_maxDamage);
				}
			}
		}

		Node2D afterEffect = _afterEffect.Instantiate<Node2D>();
		GetTree().Root.AddChild(afterEffect);
		afterEffect.GlobalPosition = GlobalPosition;

		QueueFree();
	}
}
