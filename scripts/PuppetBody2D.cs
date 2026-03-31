using Godot;

/// <summary>
/// Represents a CharacterBody2D that can be controlled either by a player or AI.
/// </summary>
public partial class PuppetBody2D : CharacterBody2D
{
	private float _curSpeed;
	/// <summary>
	/// The current speed of the character.
	/// </summary>
	public float CurSpeed { get => _curSpeed * CurSpeedFactor; }

	/// <summary>
	/// The current direction that the character moves in.
	/// </summary>
	protected Vector2 CurDir;

	[Export]
	private float _maxHealth = 50f;

	/// <summary>
	/// The maximum health of the character.
	/// </summary>
	public float MaxHealth { get => _maxHealth; }

	/// <summary>
	/// The current health of the character.
	/// </summary>
	protected float CurHealth;

	[ExportGroup("Movement")]
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
	private float _pushForce = 16f;

	/// <summary>
	/// The scale applied to the current speed.
	/// </summary>
	protected float CurSpeedFactor = 1f;

	/// <summary>
	/// Triggered when the health of the character is changed.
	/// </summary>
	/// <param name="health">
	/// The new amount of health.
	/// </param>
	[Signal]
	public delegate void OnHealthUpdatedEventHandler(float health);

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
		Vector2 targVel = CurDir * _curSpeed * CurSpeedFactor;
		Velocity = Velocity.Lerp(targVel, _velWeight * delta);
		MoveAndSlide();

		// Push colliding RigidBody2Ds out of the way.
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision2D col = GetSlideCollision(i);
			RigidBody2D rb = col.GetCollider() as RigidBody2D;

			if (rb != null)
			{
				Vector2 dirTo = (rb.GlobalPosition - GlobalPosition).Normalized();
				rb.ApplyImpulse(dirTo * _pushForce);
			}
		}
	}

	public override void _Ready()
	{
		CurHealth = _maxHealth;
		_curSpeed = _walkSpeed;
	}

	/// <summary>
	/// Gets the health of the body.
	/// </summary>
	/// <returns>
	/// The current health of the body.
	/// </returns>
	public float GetHealth()
	{
		return CurHealth;
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
}
