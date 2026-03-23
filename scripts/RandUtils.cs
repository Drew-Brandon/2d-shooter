using Godot;
using System;

/// <summary>
/// A series of utilities meant for the random generation of values.
/// </summary>
public static partial class RandUtils
{
	/// <summary>
	/// Returns a randomized float value in the specified range.
	/// </summary>
	/// <param name="min">
	/// The minimum value that the float can be.
	/// </param>
	/// <param name="max">
	/// The maximum value that the float can be.
	/// </param>
	/// <returns>
	/// The randomized float.
	/// </returns>
	public static float RandRangef(float min, float max)
	{
		return GD.Randf() * (max - min) + min;
	}

	/// <summary>
	/// Returns a randomized vector value in the specified range.
	/// </summary>
	/// <param name="min">
	/// The minimum value that the vector can be.
	/// </param>
	/// <param name="max">
	/// The maximum value that the vector can be.
	/// </param>
	/// <returns>
	/// The randomized vector.
	/// </returns>
	public static Vector2 RandVector(Vector2 min, Vector2 max)
	{
		return new Vector2(RandRangef(min.X, max.X), RandRangef(min.Y, max.Y));
	}
}
