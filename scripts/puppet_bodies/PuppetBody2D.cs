using Godot;

/// <summary>
/// Represents a CharacterBody2D that can be controlled either by a player or AI.
/// </summary>
public partial class PuppetBody2D : CharacterBody2D, IDamageable, ISaveable
{
	[Export]
	private float _mass = 70f;

	[Export]
	private string _footstepData = "footstep_audio";

	[Export]
	private TileMapLayerPlus _tileMap = null;

	[ExportGroup("Health and Damage")]
	private bool _isAlive = true;

	/// <summary>
	/// Whether the character is alive or not.
	/// </summary>
	public bool IsAlive { get => _isAlive; }

	[Export]
	private bool _canDie = true;

	/// <summary>
	/// Whether the character can die or not.
	/// </summary>
	public bool CanDie { get => _canDie; set => _canDie = value; }

	[Export]
	private float _maxHealth = 50f;

	/// <summary>
	/// The maximum health of the character.
	/// </summary>
	public float MaxHealth { get => _maxHealth; }

	[Export]
	private float _curHealth = 50f;

	/// <summary>
	/// The current health of the character.
	/// </summary>
	public float CurHealth { get => _curHealth; }

	[Export]
	private float _bloodSplatReach = 8f;

	[Export]
	private PackedScene _bloodSplat = null;

	[Export]
	private ShapeCastInfo _puppetCast = null;

	[ExportGroup("Movement")]
	private float _curSpeed;

	/// <summary>
	/// The current speed of the character.
	/// </summary>
	public float CurSpeed { get => _curSpeed * CurSpeedFactor; }
	
	private Vector2 _curDir;

	/// <summary>
	/// The current direction that the character moves in.
	/// </summary>
	protected Vector2 CurDir { get => _curDir; set => _curDir = value; }

	[Export]
	private bool _isFrozen = false;

	/// <summary>
	/// Whether or not the character is frozen and thus cannot move.
	/// </summary>
	public bool IsFrozen { get => _isFrozen; set => _isFrozen = value; }

	[Export]
	private float _walkSpeed = 100f;
	
	/// <summary>
	/// The walking speed of this character.
	/// </summary>
	public float WalkSpeed { get => _walkSpeed; }

	[Export]
	private float _runSpeed = 150f;

	/// <summary>
	/// The running speed of this character.
	/// </summary>
	public float RunSpeed { get => _runSpeed; }

	[Export]
	private float _velWeight = 100f;

	[Export]
	private float _pushPower = 4f;

	[Export]
	private float _puppetPushPower = 4f;

	[Export]
	private float _footstepSpeed = 0.1f;

	[Export]
	private AudioStreamPlayer2D _footstepPlayer;

	private float _curSpeedFactor = 1f;

	/// <summary>
	/// The scale applied to the current speed.
	/// </summary>
	protected float CurSpeedFactor { get => _curSpeedFactor; set => _curSpeedFactor = value; }

	/// <summary>
	/// Triggered when the health of the character is changed.
	/// </summary>
	/// <param name="health">
	/// The new amount of health.
	/// </param>
	[Signal]
	public delegate void OnHealthUpdatedEventHandler(float health);

	/// <summary>
	/// Triggered when the character is damaged.
	/// </summary>
	/// <param name="health">
	/// The new health of the character.
	/// </param>
	[Signal]
	public delegate void OnDamagedEventHandler(float health);

	/// <summary>
	/// Triggered when the character is healed.
	/// </summary>
	/// <param name="health">
	/// The new health of the character.
	/// </param>
	[Signal]
	public delegate void OnHealedEventHandler(float health);

	/// <summary>
	/// Triggered when the character dies.
	/// </summary>
	[Signal]
	public delegate void OnDeathEventHandler();

	private void InternKill()
	{
		_isAlive = false;
		_curHealth = 0f;
		OnKill();
		EmitSignal(SignalName.OnDeath);
	}

	/// <summary>
	/// Moves the character in their current direction at their current speed.
	/// This method also makes the character push RigidBody2Ds out of the way.
	/// This should be called in the _PhysicsProcess.
	/// </summary>
	/// <param name="delta">
	/// The delta time to use for the update.
	/// </param>
	protected void MoveUpdate(float delta)
	{
		// Get the target velocity, ease into it, and move the character.
		Vector2 targVel;

		if (_isFrozen)
		{
			targVel = Vector2.Zero;
		}
		else
		{
			targVel = CurDir * _curSpeed * CurSpeedFactor;
		}

		Velocity = Velocity.Lerp(targVel, _velWeight * delta);

		Vector2 prevPos = GlobalPosition;
		MoveAndSlide();
		Vector2 disp = GlobalPosition - prevPos;

		if (_tileMap != null && disp.Length() >= _footstepSpeed)
		{
			TileData tileData = _tileMap.GetCellTileDataGlobal(GlobalPosition);

			if (tileData != null && tileData.HasCustomData(_footstepData))
			{
				AudioStream audio = tileData.GetCustomData(_footstepData).AsGodotObject() as AudioStream;

				if (audio != null && audio != _footstepPlayer.Stream)
				{
					_footstepPlayer.Stream = audio;
				}
			}

			_footstepPlayer.PitchScale = _curSpeed / _walkSpeed;

			if (!_footstepPlayer.Playing)
			{
				_footstepPlayer.Play();
			}
		}
		else if (_footstepPlayer.Playing)
		{
			_footstepPlayer.Stop();
		}

		// Push colliding RigidBody2Ds out of the way.
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision2D col = GetSlideCollision(i);
			RigidBody2D rb = col.GetCollider() as RigidBody2D;

			if (rb != null)
			{
				// Credit to https://www.youtube.com/watch?v=Uh9PSOORMmA
				Vector2 pushDir = -col.GetNormal();
				Vector2 pushPos = col.GetPosition();
				float velDiff = Mathf.Max(0f, Velocity.Dot(pushDir) - rb.LinearVelocity.Dot(pushDir));
				float massRatio = Mathf.Min(1f, _mass / rb.Mass);
				float adjPushPower = massRatio * _pushPower;
				rb.ApplyImpulse(pushDir * velDiff * adjPushPower, pushPos - rb.GlobalPosition);
			}
		}

		/*PhysicsShapeQueryParameters2D query = _puppetCast.CreateQuery(Transform, [GetRid()]);
		ShapeCastResults[] results = ShapeCastInfo.IntersectShape(this, query);

		for (int i = 0; i < results.Length; i++)
		{
			PuppetBody2D _
		}*/
	}

	/// <summary>
	/// Called when the character is killed.
	/// </summary>
	protected virtual void OnKill()
	{
		QueueFree();
	}

	public override void _Ready()
	{
		_curHealth = _maxHealth;
		_curSpeed = _walkSpeed;
	}

	/// <summary>
	/// Kills the character.
	/// </summary>
	public void Kill()
	{
		if (_isAlive)
		{
			InternKill();
		}
	}

	private void SpreadBlood()
	{
		Node2D bloodSplat = _bloodSplat.Instantiate<Node2D>();
		GetParent().AddChild(bloodSplat);

		float angle = RandUtils.RandRangef(0, Mathf.Pi * 2f);
		bloodSplat.GlobalPosition = GlobalPosition + Vector2.FromAngle(angle) * _bloodSplatReach;
		bloodSplat.GlobalRotation = RandUtils.RandRangef(0f, Mathf.Pi * 2f);
	}

	protected virtual void HealthUpdated(Node2D instigator)
	{

	}

	public void AddHealth(Node2D instigator, float amount)
	{
		if (_isAlive)
		{
			_curHealth = Mathf.Clamp(_curHealth + amount, 0f, _maxHealth);
			HealthUpdated(instigator);
			EmitSignal(SignalName.OnHealthUpdated, _curHealth);

			if (amount < 0f)
			{
				if (_canDie)
				{
					SpreadBlood();
					EmitSignal(SignalName.OnDamaged, _curHealth);

					if (_curHealth <= 0f)
					{
						InternKill();
					}
				}
			}
			else if (amount > 0f)
			{
				EmitSignal(SignalName.OnHealed, _curHealth);
			}
		}
	}

	/// <summary>
	/// Changes the current speed to the walking speed of this character.
	/// </summary>
	public void Walk()
	{
		_curSpeed = WalkSpeed;
	}

	/// <summary>
	/// Changes the current speed to the running speed of this character.
	/// </summary>
	public void Run()
	{
		_curSpeed = RunSpeed;
	}

	public NodeSave GetSave()
	{
		return new Puppet2DSave(Transform, _curHealth);
	}

	public void LoadSave(NodeSave save)
	{
		Puppet2DSave puppetSave = save as Puppet2DSave;

		if (puppetSave == null)
		{
			return;
		}

		Transform = puppetSave.Transform;
		_curHealth = puppetSave.Health;

		if (_curHealth <= 0f)
		{
			Kill();
		}
	}
}
