using Godot;

public partial class Door : RigidBody2D, IInteractable, ISaveable
{
	[Export]
	private bool _isLocked = false;
	public bool IsLocked { get => _isLocked; }

	[Export]
	private BaseItemInstanceResource _keyInstance = null;

	[ExportGroup("Creaking")]
	private float _creakProgress = 0f;

	[Export]
	private float _creakCooldown = 0.5f;

	[Export]
	private float _minCreakVel = 0.1f;

	[Export]
	private float _creakScale = 1f;

	[Export]
	private float _creakStart = 0.5f;

	[Export]
	private float _maxCreakVol = 4f;

	[Export]
	private AudioStreamPlayer2D _creakPlayer;

	public void SetLocked(bool val)
	{
		_isLocked = val;
		Freeze = _isLocked;
		SetPhysicsProcess(!_isLocked);
	}

	public override void _Ready()
	{
		SetLocked(_isLocked || _keyInstance != null);
	}

	public override void _PhysicsProcess(double delta)
	{
		float deltaF = (float)delta;
		float absVel = Mathf.Abs(AngularVelocity);

		if (absVel >= _minCreakVel)
		{
			_creakProgress = _creakCooldown;

			if (!_creakPlayer.Playing)
			{
				_creakPlayer.Play(_creakStart);
			}
		}
		else if (_creakPlayer.Playing)
		{
			if (_creakProgress > 0f)
			{
				_creakProgress -= deltaF;
			}
			else
			{
				_creakPlayer.Stop();
			}
		}

		_creakPlayer.VolumeLinear = Mathf.Clamp(_creakScale * (absVel - _minCreakVel), 0f, _maxCreakVol);
	}

	public bool Interact(PlayerBody2D plyr)
	{
		if (_keyInstance != null && _isLocked)
		{
			BaseItemInstance instance = _keyInstance.GetInstance();

			if (plyr.Inventory.ChargeInstance(instance))
			{
				SetLocked(false);
			}
		}

		return false;
	}

	public NodeSave GetSave()
	{
		return new DoorSave(Transform, LinearVelocity, AngularVelocity, _isLocked);
	}

	public void LoadSave(NodeSave save)
	{
		DoorSave doorSave = save as DoorSave;

		if (doorSave == null)
		{
			return;
		}

		Transform = doorSave.Transform;
		LinearVelocity = doorSave.LinearVel;
		AngularVelocity = doorSave.AngularVel;
		SetLocked(doorSave.IsLocked);
	}
}
