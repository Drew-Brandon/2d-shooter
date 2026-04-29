using Godot;
using System;

public partial class DraggableControl : Control
{
	public override Variant _GetDragData(Vector2 atPosition)
	{
		return new Variant();
	}
}
