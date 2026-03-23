using Godot;
using System;

/// <summary>
/// Provides a class/node with the ability to be interacted with by the player.
/// </summary>
public interface IInteractable
{
	/// <summary>
	/// Interacts with the holding class/node.
	/// </summary>
	/// <param name="plyr">
	/// The player performing the interaction.
	/// </param>
	/// <returns>
	/// Whether or not the interaction was successful.
	/// </returns>
	public bool Interact(PlayerBody2D plyr);

	/// <summary>
	/// Stops an interaction with the player.
	/// </summary>
	/// <param name="plyr">
	/// The player performing the interaction.
	/// </param>
	public void StopInteract(PlayerBody2D plyr);
}
