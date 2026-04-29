using Godot;

public partial class HoverDisplay : Control
{
	[Export]
	private Vector2 _mouseOffset = new Vector2(16f, 16f);

	[Export]
	private Label _label;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = GetGlobalMousePosition() + _mouseOffset;
	}

	public void SetText(string txt)
	{
		_label.Text = txt;
	}
}
