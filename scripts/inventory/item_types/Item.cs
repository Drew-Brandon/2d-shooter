using Godot;

/// <summary>
/// Represents an inventory item that can be used for a multitude of purposes.
/// </summary>
[GlobalClass]
public partial class Item : Resource
{
	[Export]
	private int _maxAmount = 16;

	/// <summary>
	/// The maximum amount of items that can be in a stack.
	/// </summary>
	public int MaxAmount { get => _maxAmount; }

	[Export]
	private StringName _name = "";

	/// <summary>
	/// The name of this item.
	/// </summary>
	public StringName Name { get => _name; }

	[Export]
	private Texture2D _icon;

	/// <summary>
	/// The icon used for this item.
	/// </summary>
	public Texture2D Icon { get => _icon; }
}
