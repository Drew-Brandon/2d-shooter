using Godot;

/// <summary>
/// The structure representing the results of a shape cast.
/// </summary>
public partial struct ShapeCastResults
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
	/// The collider that was hit by the cast.
	/// Defaults to null.
	/// </summary>
	public GodotObject Collider = null;

	/// <summary>
	/// Initalizes a new RayCastResults object.
	/// </summary>
	public ShapeCastResults()
	{ }
}
