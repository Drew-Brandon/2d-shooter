using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class Sight : Node2D
{
	[Export]
	private float _viewAngle = 80f;

	[Export]
	private ShapeCastInfo _rangeCast;

	[Export]
	private RayCastInfo _sightCast;

	[Export]
	private Node2D[] _ignoreNodeList;

	private Array<Rid> _ignoreRidList;

	private HashSet<Node2D> _curTargets = new();

	[Signal]
	public delegate void OnSpottedEventHandler(Node2D node);

	[Signal]
	public delegate void OnLostEventHandler(Node2D node);

	/// <summary>
	/// Checks if the sight source has a line of sight to the other specified node.
	/// </summary>
	/// <param name="node">
	/// The node to sight check.
	/// </param>
	/// <returns>
	/// Whether this sight source has a line of sight or not to the specified node.
	/// </returns>
	private bool SightCheck(Node2D node)
	{
		PhysicsRayQueryParameters2D sightQuery = _sightCast.CreateQuery(GlobalPosition, node.GlobalPosition, _ignoreRidList);
		RayCastResults sightResults = RayCastInfo.IntersectRay(this, sightQuery);
		Node2D hitNode = (Node2D)sightResults.Collider;
		return hitNode == node;
	}

	private void SightUpdate()
	{
		HashSet<Node2D> prevTargets = _curTargets;
		_curTargets = new HashSet<Node2D>();

		// Perform the range cast.
		PhysicsShapeQueryParameters2D rangeQuery = _rangeCast.CreateQuery(GlobalTransform, _ignoreRidList);
		Array<Dictionary> rangeResults = ShapeCastInfo.IntersectShape(this, rangeQuery);

		// Comb through each node in range.
		for (int i = 0; i < rangeResults.Count; i++)
		{
			Node2D node = rangeResults[i]["collider"].As<Node2D>();
			float angle = GetAngleTo(node.GlobalPosition);

			/*
			 * If the sight check was successful, then mark the node as spotted.
			 * Otherwise, mark it as lost.
			 */
			if (angle >= -_viewAngle && angle <= _viewAngle && SightCheck(node))
			{
				if (prevTargets.Contains(node))
				{
					prevTargets.Remove(node);
				}
				else
				{
					EmitSignal(SignalName.OnSpotted, node);
				}

				_curTargets.Add(node);
			}
			else if (prevTargets.Contains(node))
			{
				prevTargets.Remove(node);
				EmitSignal(SignalName.OnLost, node);
			}
		}

		// For all the left over targets from the last update, emit a signal stating they were lost.
		foreach (Node2D node in prevTargets)
		{
			EmitSignal(SignalName.OnLost, node);
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_viewAngle = Mathf.DegToRad(_viewAngle);
		_ignoreRidList = new Array<Rid>();

		for (int i = 0; i < _ignoreNodeList.Length; i++)
		{
			_ignoreRidList.Add(new Rid(_ignoreNodeList[i]));
		}

		/* 
		 * Tilemap collision appears to only generate a little after the game starts,
		 * so try to start processing after it has been fully created.
		 */
		SetPhysicsProcess(false);
		CallDeferred(MethodName.SetPhysicsProcess, true);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		SightUpdate();
	}
}
