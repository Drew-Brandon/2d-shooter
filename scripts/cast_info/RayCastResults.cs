using Godot;

/// <summary>
/// A structure representing the results of a raycast.
/// </summary>
public partial struct RayCastResults
{
	/// <summary>
	/// The shape index of the object that was hit by the cast.
	/// Defaults to -1.
	/// </summary>
	public int Shape = -1;

	/// <summary>
	/// The rid of the Collider that was hit by the cast.
	/// Defaults to base rid value.
	/// </summary>
	public Rid ColliderRid = new Rid();

	/// <summary>
	/// The position in global coordinates where the cast hit the object.
	/// Defaults to Vector2.Zero.
	/// </summary>
	public Vector2 Position = Vector2.Zero;

	/// <summary>
	/// The normal of the surface the cast hit.
	/// Defaults to Vector2.Zero.
	/// </summary>
	public Vector2 Normal = Vector2.Zero;

	/// <summary>
	/// The collider that was hit by the cast.
	/// Defaults to null.
	/// </summary>
	public GodotObject Collider = null;

	/// <summary>
	/// Initalizes a new RayCastResults object.
	/// </summary>
	public RayCastResults()
	{ }
}
