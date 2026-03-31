using Godot;

public partial class BodyAim : Control
{
	private bool _inMotion = false;

	[Export]
	private PlayerBody2D _plyr;

	[Export]
	private Control _bodyParent;

	[Export]
	private BodyPartButton[] _bodyPartButtons;

	[ExportGroup("Scaling")]
	[Export]
	private float _minScale = 0.5f;

	[Export]
	private float _maxScale = 1f;

	[Export]
	private float _minScaleDist = 512f;

	[Export]
	private float _maxScaleDist = 64f;

	[ExportGroup("Motion")]
	[Export]
	private AnimationPlayer _animPlyr;

	private AIBody2D _target;

	/// <summary>
	/// The target that is currently being aimed at.
	/// </summary>
	public AIBody2D Target { get => _target; set => _target = value; }

	/// <summary>
	/// Called when a part of the body is hit.
	/// Deals damage to the current target at the specified body part.
	/// </summary>
	/// <param name="bodyPart">
	/// The body part that was hit.
	/// </param>
	private void OnHit(int bodyPart)
	{
		_plyr.Attack(_target.GlobalPosition, (BodyPart)bodyPart);

		// Stop aiming when the target dies.
		if (_target.GetHealth() <= 0f)
		{
			_plyr.StopAim();
		}
	}

	private void Attack()
	{
		for (int i = 0; i < _bodyPartButtons.Length; i++)
		{
			if (_bodyPartButtons[i].ButtonPressed)
			{
				OnHit((int)_bodyPartButtons[i].BodyPart);
				return;
			}
		}

		OnHit(-1);
	}

	/// <summary>
	/// Updates the scale of the body UI based of the players distance to the current target.
	/// </summary>
	private void UpdateScale()
	{
		float dist = _plyr.GlobalPosition.DistanceTo(_target.GlobalPosition);
		float distPerc = Mathf.Clamp((dist - _minScaleDist) / (_maxScaleDist - _minScaleDist), 0f, 1f);
		float scale = distPerc * (_maxScale - _minScale) + _minScale;

		_bodyParent.Scale = Vector2.One * scale;
	}

	public override void _Ready()
	{
		SetProcess(false);
	}

	public override void _Process(double delta)
	{
		float deltaF = (float)delta;
		UpdateScale();
		_animPlyr.SpeedScale = _target.CurSpeed / _target.WalkSpeed;

		if (Input.IsActionJustPressed("use"))
		{
			Attack();
		}
	}
}
