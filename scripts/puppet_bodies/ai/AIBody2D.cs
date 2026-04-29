using Godot;
using System.Collections;

/// <summary>
/// The body controlled by an AI.
/// </summary>
public partial class AIBody2D : PuppetBody2D, IInteractable, ISleeper
{
	private float _aimFadeProgress = 0f;

	[Export]
	private float _aimFadeTime = 2f;

	[Export]
	private float[] _bodyPartsDmgMult = new float[(int)BodyPart.All];

	/// <summary>
	/// The target position that this AI will move to.
	/// </summary>
	public Vector2 TargetPosition { get => _navAgent.TargetPosition; set => _navAgent.TargetPosition = value; }

	private PlayerBody2D _curPlyr = null;

	[Export]
	private NavigationAgent2D _navAgent = null;

	[ExportGroup("Blackboard")]
	[Export]
	private StringName _searchingState = "searching";

	[Export]
	private StringName _chasingState = "chasing";

	[Export]
	private StringName _targNode = "targ_node";

	[Export]
	private AIBlackboard _blackboard = null;

	[Export]
	private BTPlayer _btPlayer = null;

	[ExportGroup("Cripple")]
	[Export]
	private int _cripplSpeedCooldown = 2000;

	[Export]
	private float _crippleSpeedFactor = 0.6f;

	private CoroutineSource _crippleSrc = new();

	[ExportGroup("Attack")]
	[Export]
	private int _attackCooldown = 2000;

	[Export]
	private float _damage = 20f;

	private CoroutineSource _attackSrc = new();

	[Export]
	private RayCastInfo _attackLOSCast = null;

	[Export]
	private ShapeCastInfo _attackRangeCast = null;

	[Signal]
	public delegate void OnTargetReachedEventHandler();

	[Signal]
	public delegate void OnNavigationFinishedEventHandler();

	private IEnumerable Attack()
	{
		while (_curPlyr != null)
		{
			_curPlyr.AddHealth(this, -_damage);
			yield return _attackCooldown;
		}
	}

	private IEnumerable CrippleSpeedCooldown()
	{
		CurSpeedFactor = _crippleSpeedFactor;
		yield return _cripplSpeedCooldown;
		CurSpeedFactor = 1f;
	}

	private void AttackUpdate()
	{
		PhysicsShapeQueryParameters2D rangeQuery = _attackRangeCast.CreateQuery(GlobalTransform, [GetRid()]);
		ShapeCastResults[] rangeResults = ShapeCastInfo.IntersectShape(this, rangeQuery);

		for (int i = 0; i < rangeResults.Length; i++)
		{
			PlayerBody2D plyr = rangeResults[i].Collider as PlayerBody2D;

			if (plyr != null)
			{
				PhysicsRayQueryParameters2D rayQuery = _attackLOSCast.CreateQuery(GlobalPosition, plyr.GlobalPosition, [GetRid()]);
				RayCastResults losResults = RayCastInfo.IntersectRay(this, rayQuery);

				if (losResults.Collider == plyr)
				{
					if (_curPlyr == null)
					{
						_curPlyr = plyr;

						if (!_attackSrc.IsRunning)
						{
							_attackSrc.StartCoroutine(Attack());
						}
					}
					
					return;
				}
			}
		}

		if (_curPlyr != null)
		{
			_curPlyr = null;
		}
	}
	
	/// <summary>
	/// Called when a body part is damaged.
	/// </summary>
	/// <param name="bodyPart">
	/// The body part that was damaged.
	/// </param>
	protected virtual void OnPartDamaged(BodyPart bodyPart)
	{
		if (bodyPart == BodyPart.LeftLeg || bodyPart == BodyPart.RightLeg)
		{
			Cripple();
		}
	}

	public override void _Ready()
	{
		base._Ready();
		_navAgent.TargetReached += () => EmitSignal(SignalName.OnTargetReached);
		_navAgent.NavigationFinished += () => EmitSignal(SignalName.OnNavigationFinished);
	}

	public override void _ExitTree()
	{
		_curPlyr = null;
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
		
		if (_blackboard.InState(_chasingState))
		{
			AttackUpdate();
		}
	}

	public void Cripple()
	{
		if (_crippleSrc.IsRunning)
		{
			_crippleSrc.IsCancelled = true;
			_crippleSrc = new CoroutineSource();
		}

		_crippleSrc.StartCoroutine(CrippleSpeedCooldown());
	}

	protected override void HealthUpdated(Node2D instigator)
	{
		if (!_blackboard.InState(_chasingState))
		{
			_blackboard.SetState(_searchingState);
			_blackboard.SetNode(_targNode, instigator);
			_btPlayer.Restart();
		}
	}

	/// <summary>
	/// Damages the AI at the specified body part.
	/// </summary>
	/// <param name="bodyPart">
	/// The body part to damage.
	/// </param>
	/// <param name="amount">
	/// The amount of damage to deal.
	/// </param>
	public void DamagePart(Node2D instigator, BodyPart bodyPart, float amount)
	{
		if (bodyPart > BodyPart.None && IsAlive)
		{
			AddHealth(instigator, -amount * _bodyPartsDmgMult[(int)bodyPart]);
			OnPartDamaged(bodyPart);
		}
	}

	public bool Interact(PlayerBody2D plyr)
	{
		_aimFadeProgress = 0f;
		plyr.StartAim(this);
		return true;
	}

	public void StopInteract(PlayerBody2D plyr)
	{
		plyr.StopAim();
	}

	public bool InteractUpdate(float delta, PlayerBody2D plyr)
	{
		ShapeCastResults[] results = plyr.LOSPointCast(GlobalPosition);

		if (results != null)
		{
			for (int i = 0; i < results.Length; i++)
			{
				if (results[i].Collider == this)
				{
					return false;
				}
			}
		}

		_aimFadeProgress += delta;
		return _aimFadeProgress >= _aimFadeTime;
	}

	public void Sleep()
	{
		Visible = false;
		ProcessMode = ProcessModeEnum.Disabled;
		_btPlayer.Stop();
	}

	public void Awake()
	{
		Visible = true;
		ProcessMode = ProcessModeEnum.Inherit;
		_btPlayer.Play();
	}
}
