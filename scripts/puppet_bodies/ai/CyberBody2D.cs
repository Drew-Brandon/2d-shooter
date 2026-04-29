using Godot;

public partial class CyberBody2D : AIBody2D
{
	[Export]
	private PackedScene _headlessCorpse;

	[Export]
	private PackedScene _genericCorpse;

	protected override void OnPartDamaged(BodyPart bodyPart)
	{
		base.OnPartDamaged(bodyPart);

		if (!IsAlive)
		{
			Node2D corpse;

			if (bodyPart == BodyPart.Head)
			{
				corpse = _headlessCorpse.Instantiate<Node2D>();
			}
			else
			{
				corpse = _genericCorpse.Instantiate<Node2D>();
			}

			GetParent().AddChild(corpse);
			corpse.GlobalPosition = GlobalPosition;
			corpse.GlobalRotation = GlobalRotation;
		}
	}
}
