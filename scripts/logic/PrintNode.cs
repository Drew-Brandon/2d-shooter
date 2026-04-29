using Godot;

/// <summary>
/// A node purely meant for debugging.
/// Primary purpose is to be connected to signals in the editor.
/// </summary>
[GlobalClass]
public partial class PrintNode : Node
{
	/// <summary>
	/// Prints out the specified message.
	/// </summary>
	/// <param name="msg">
	/// The message to display.
	/// </param>
	public void Print(string msg)
	{
		GD.Print(msg);
	}
}
