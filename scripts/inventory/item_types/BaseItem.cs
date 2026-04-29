using Godot;

/// <summary>
/// Represents an inventory item that can be used for a multitude of purposes.
/// </summary>
[GlobalClass]
public partial class BaseItem : Resource
{
	[Export]
	private StringName _displayName = "";

	/// <summary>
	/// The name of this item.
	/// </summary>
	public StringName DisplayName { get => _displayName; }

	[Export]
	private Texture2D _icon;

	/// <summary>
	/// The icon used for this item.
	/// </summary>
	public Texture2D Icon { get => _icon; }

	[Export]
	private PackedScene _itemScene = null;

	public PackedScene ItemScene { get => _itemScene; }

	public ItemNode Equip(PlayerBody2D plyr)
	{
		return plyr.AddUseable(_itemScene);
	}

	public override string ToString()
	{
		return _displayName;
	}
}
