using Godot;
using Godot.Collections;
using System.Threading;

/// <summary>
/// The body controlled by an AI.
/// </summary>
public partial class AIBody2D : PuppetBody2D, IInteractable
{
	[Export]
	private float _damage = 20f;

	[Export]
	private float[] _bodyPartsDmgMult = new float[(int)BodyPart.All];

	[Export]
	private NavigationAgent2D _navAgent;

	/// <summary>
	/// The target position that this AI will move to.
	/// </summary>
	public Vector2 TargetPosition { get => _navAgent.TargetPosition; set => _navAgent.TargetPosition = value; }

	[Export]
	private ShapeCastInfo _attackCastInfo;

	private CancellationTokenSource _cts = new();

	private PlayerBody2D _plyr;

	[Export]
	private float _crippleSpeedFactor = 0.6f;

	[Export]
	private double _cripplSpeedCooldown = 2d;

	private bool _isSpeedCrippled = false;

	[Export]
	private double _attackCooldown = 2d;

	[Export]
	private AudioStreamPlayer2D _dmgPlayer;

	[Signal]
	public delegate void OnDeathEventHandler();

	[Signal]
	public delegate void OnTargetReachedEventHandler();

	[Signal]
	public delegate void OnNavigationFinishedEventHandler();

	private async void Attack()
	{
		while (_plyr != null)
		{
			_plyr.Damage(_damage);
			await Coroutines.Wait(this, _attackCooldown);
		}
	}

	private async void CrippleSpeedCooldown(CancellationTokenSource cts)
	{
		await Coroutines.Wait(this, _cripplSpeedCooldown);
		
		if (!cts.IsCancellationRequested)
		{
			CurSpeedFactor = 1f;
			_isSpeedCrippled = false;
		}
	}

	public override void _Ready()
	{
		base._Ready();
		_navAgent.TargetReached += () => EmitSignal(SignalName.OnTargetReached);
		_navAgent.NavigationFinished += () => EmitSignal(SignalName.OnNavigationFinished);
	}

	private void AttackUpdate()
	{
		PhysicsShapeQueryParameters2D query = _attackCastInfo.CreateQuery(GlobalTransform, [GetRid()]);
		Array<Dictionary> results = ShapeCastInfo.IntersectShape(this, query);

		for (int i = 0; i < results.Count; i++)
		{
			PlayerBody2D plyr = results[i]["collider"].As<Node2D>() as PlayerBody2D;

			if (plyr != null)
			{
				if (_plyr == null)
				{
					_plyr = plyr;
					Attack();
				}
				
				return;
			}
		}

		if (_plyr != null)
		{
			_plyr = null;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		float deltaF = (float)delta;
		Vector2 nextPos = _navAgent.GetNextPathPosition();
		CurDir = (nextPos - GlobalPosition).Normalized();

		if (!_navAgent.IsNavigationFinished())
		{
			Rotation = Mathf.Atan2(Velocity.Y, Velocity.X);
			MoveUpdate(deltaF);
		}
		
		AttackUpdate();
	}

	public void Kill()
	{
		QueueFree();
		EmitSignal(SignalName.OnDeath);
	}

	public void DamagePart(BodyPart bodyPart, float amount)
	{
		if (bodyPart > BodyPart.None)
		{
			_dmgPlayer.Play();
			CurHealth = CurHealth - amount * _bodyPartsDmgMult[(int)bodyPart];
			EmitSignal(SignalName.OnHealthUpdated, CurHealth);

			if (CurHealth <= 0f)
			{
				Kill();
			}
			else if (bodyPart == BodyPart.LeftLeg || bodyPart == BodyPart.RightLeg)
			{
				if (_isSpeedCrippled)
				{
					_cts.Cancel();
					_cts = new();
				}

				CurSpeedFactor = _crippleSpeedFactor;
				CrippleSpeedCooldown(_cts);
			}
		}
	}

	public bool Interact(PlayerBody2D plyr)
	{
		plyr.StartAim(this);
		return true;
	}

	public void StopInteract(PlayerBody2D plyr)
	{

	}
}
