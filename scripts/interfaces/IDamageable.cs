using Godot;

/// <summary>
/// Provides an object with the ability to 
/// </summary>
public partial interface IDamageable
{
	/// <summary>
	/// Adds the specified amount of health to the object.
	/// </summary>
	/// <param name="instigator">
	/// The node that is causing the effect.
	/// </param>
	/// <param name="amount">
	/// The amount of health to add.
	/// </param>
	public void AddHealth(Node2D instigator, float amount);
}
