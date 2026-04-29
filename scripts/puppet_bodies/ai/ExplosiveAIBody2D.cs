using Godot;

public partial class ExplosiveAIBody2D : AIBody2D
{
	[Export]
	private Explosive _explosive;

	protected override void OnKill()
	{
		_explosive.Explode();
		QueueFree();
	}
}
