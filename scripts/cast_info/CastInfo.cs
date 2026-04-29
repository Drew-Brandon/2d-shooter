using Godot;
using Godot.Collections;

/// <summary>
/// Stores information regarding a general casting.
/// </summary>
public abstract partial class CastInfo : Resource
{
	[Export]
	private bool _collideWithAreas = false;

	/// <summary>
	/// Whether or not the casting can collide with areas.
	/// </summary>
	public bool CollideWithAreas { get => _collideWithAreas; }

	[Export]
	private bool _collideWithBodies = true;

	/// <summary>
	/// Whether or not the casting can collide with bodies.
	/// </summary>
	public bool CollideWithBodies { get => _collideWithBodies; }

	[Export(PropertyHint.Layers2DPhysics)]
	private uint _collisionMask = 0;

	/// <summary>
	/// The layers that can be casted for.
	/// </summary>
	public uint CollisionMask { get => _collisionMask; }
}
