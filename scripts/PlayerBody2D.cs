using Godot;
using Godot.Collections;
using System.Threading;

/// <summary>
/// The body controlled by the player.
/// </summary>
public partial class PlayerBody2D : PuppetBody2D
{
	private bool _isAiming = false;

	private int _curWeaponIndex = -1;

	[ExportGroup("Health and Damage")]
	[Export]
	private double _recoveryTime = 1d;

	[Export]
	private float _dmgSpeedNerf = 0.5f;

	[Export]
	private StringName _dmgSpeedNerfRoutine = "damage_speed_nerf";

	[ExportGroup("Mouse Casting")]
	[Export]
	private RayCastInfo _mouseCastSight;

	[Export]
	private ShapeCastInfo _mouseCastArea;

	[ExportGroup("Inventory")]
	private bool _inventoryOpen = false;

	[Export]
	private FiniteInventory _inventory;

	[Export]
	private InventoryUI _inventoryUI;

	[ExportGroup("Misc")]

	[Export]
	private BodyAim _bodyAim;

	[Export]
	private Node2D _swivel;

	[Export]
	private Texture2D _crosshair;

	[Export]
	private Control _mainUI;

	private bool _isAlive = true;

	[Export]
	private bool _canDie = true;

	private bool _isRecovering = false;

	private CancellationTokenSource _cts = new();

	/// <summary>
	/// Performs a raycast towards the mouse's position,
	/// and scans for any nodes around the mouse if there was no obstruction.
	/// </summary>
	/// <returns>
	/// The results of the cast.
	/// This will be null if the cast was obstructed.
	/// </returns>
	private Array<Dictionary> MouseCast()
	{
		Vector2 mousePos = GetGlobalMousePosition();
		PhysicsRayQueryParameters2D sightQuery = _mouseCastSight.CreateQuery(GlobalPosition, mousePos, [GetRid()]);
		RayCastResults sightResults = RayCastInfo.IntersectRay(this, sightQuery);

		if (sightResults.Collider != null)
		{
            Transform2D transform = new(0f, GetGlobalMousePosition());
            PhysicsShapeQueryParameters2D query = _mouseCastArea.CreateQuery(transform);
            return ShapeCastInfo.IntersectShape(this, query);
        }

		return null;
    }

	/// <summary>
	/// Start aiming at the specified target AI.
	/// </summary>
	/// <param name="ai">
	/// The ai to aim for.
	/// </param>
	public void StartAim(AIBody2D ai)
	{
		if (!_inventoryOpen)
		{
            _isAiming = true;
            _bodyAim.Show();
            _bodyAim.SetProcess(true);
            _bodyAim.Target = ai;
        }
	}

	/// <summary>
	/// Stop aiming at the current target AI.
	/// </summary>
	public void StopAim()
	{
		_isAiming = false;
		_bodyAim.Target = null;
		_bodyAim.Hide();
		_bodyAim.SetProcess(false);
	}
	
	private void Interact()
	{
		Array<Dictionary> castResults = MouseCast();

		if (castResults == null)
		{
			return;
		}

		for (int i = 0; i < castResults.Count;i++)
		{
            IInteractable interactable = castResults[i]["collider"].As<Node2D>() as IInteractable;

			if (interactable != null)
			{
				bool success = interactable.Interact(this);

				if (success)
				{
					break;
				}
			}
        }
    }

	/// <summary>
	/// Forces the player to attack with their currently equipped weapon.
	/// </summary>
	/// <param name="target">
	/// The position to attack.
	/// </param>
	/// <param name="bodyPart">
	/// The body part to target/attack.
	/// </param>
	public void Attack(Vector2 target, BodyPart bodyPart = BodyPart.Torso)
	{
		
	}

	public override void _Ready()
	{
		base._Ready();
		Input.SetCustomMouseCursor(_crosshair);
		_inventory.SetUI(_inventoryUI);
	}

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("sprint"))
        {
            Run();
        }
        else
        {
            Walk();
        }

        #region Weapon Use
		
        #endregion

        #region Interaction
        if (Input.IsActionJustPressed("use"))
        {
            if (_isAiming)
			{
				StopAim();
			}
			else
			{
				Interact();
			}
        }
        #endregion

        if (Input.IsActionJustPressed("toggle_inventory"))
        {
			_inventoryOpen = !_inventoryOpen;
            _inventoryUI.Visible = !_inventoryUI.Visible;
        }
    }

    public override void _PhysicsProcess(double delta)
	{
		float deltaF = (float)delta;

		if (_isAiming)
		{
			_swivel.LookAt(_bodyAim.Target.GlobalPosition);
		}
		else
		{
			_swivel.LookAt(GetGlobalMousePosition());
		}

		CurDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		MoveUpdate(deltaF);
	}

	public void Damage(float amount)
	{
		if (_isAlive)
		{
			CurHealth -= amount;
			EmitSignal(SignalName.OnHealthUpdated, CurHealth);

			if (CurHealth <= 0f && _canDie)
			{
				_isAlive = false;
				GetTree().ReloadCurrentScene();
			}
			else
			{
				if (_isRecovering)
				{
					_cts.Cancel();
					_cts = new();
				}

				_isRecovering = true;
				CurSpeedFactor = _dmgSpeedNerf;
				DamageSpeedNerf(_cts);
			}
		}
	}

	private async void DamageSpeedNerf(CancellationTokenSource cts)
	{
		await Coroutines.Wait(this, _recoveryTime);

		if (!cts.Token.IsCancellationRequested)
		{
			CurSpeedFactor = 1f;
			_isRecovering = true;
		}
	}

	public Control AddUI(PackedScene newUI)
	{
		Control clone = newUI.Instantiate<Control>();
		_mainUI.AddChild(clone);
		return clone;
	}

	public InventoryUI OpenContainerUI(PackedScene newUI, FiniteInventory inventory)
	{
		InventoryUI ui = newUI.Instantiate<InventoryUI>();
		_mainUI.AddChild(ui);
		inventory.SetUI(ui);
		return ui;
	}
}
