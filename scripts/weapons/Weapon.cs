using Godot;

/// <summary>
/// Represents a weapon that the can be used.
/// </summary>
public partial class Weapon : Node2D
{
	[Export]
	protected float _dmg = 5f;

	/// <summary>
	/// Triggers the weapon to attack a specific target location and body part.
	/// </summary>
	/// <param name="target">
	/// The target location to attack.
	/// </param>
	/// <param name="bodyPart">
	/// The body part to attack.
	/// </param>
	public virtual void Attack(Vector2 target, BodyPart bodyPart = BodyPart.Torso)
	{

	}
}
