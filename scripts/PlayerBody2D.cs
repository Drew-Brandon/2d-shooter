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

	private UseableItem _equippedItem = null;

	private InventorySlot _equippedSlot = null;

	private Control _equippedDisplay = null;

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

	private void Equip(int index)
	{
		if (_equippedDisplay != null)
		{
			_equippedDisplay.QueueFree();
			_equippedDisplay = null;
		}

		_equippedSlot = _inventory.GetSlot(index);
		_equippedItem = _equippedSlot.CurItem as UseableItem;

		if (_equippedItem != null)
		{
			_equippedDisplay = _equippedItem.Equip(this);
		}
	}

	/// <summary>
	/// Performs a raycast towards the mouse's position,
	/// and scans for any nodes around the mouse if there was no obstruction.
	/// </summary>
	/// <returns>
	/// The results of the cast.
	/// This will be null if the cast was obstructed.
	/// </returns>
	private ShapeCastResults[] MouseCast()
	{
		Vector2 mousePos = GetGlobalMousePosition();
		PhysicsRayQueryParameters2D sightQuery = _mouseCastSight.CreateQuery(GlobalPosition, mousePos, [GetRid()]);
		RayCastResults sightResults = RayCastInfo.IntersectRay(this, sightQuery);

		if (sightResults.Collider == null)
		{
            Transform2D transform = new(0f, mousePos);
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
		ShapeCastResults[] castResults = MouseCast();

		if (castResults == null)
		{
			return;
		}

		for (int i = 0; i < castResults.Length; i++)
		{
            IInteractable interactable = castResults[i].Collider as IInteractable;

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
		GunItem gun = _equippedItem as GunItem;

		if (gun != null)
		{
			gun.AimUse(this, _equippedSlot.Stack, _bodyAim.Target.GlobalPosition, bodyPart);
		}
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

		if (Input.IsActionJustPressed("equip_0"))
		{
			Equip(0);
		}
		else if (Input.IsActionJustPressed("equip_1"))
        {
            Equip(1);
        }
		else if (Input.IsActionJustPressed("equip_2"))
        {
            Equip(2);
        }

        if (Input.IsActionJustPressed("interact"))
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

        if (Input.IsActionJustPressed("reload") && _equippedItem is GunItem)
        {
            ((GunItem)_equippedItem).Reload(_inventory, (GunStack)_equippedSlot.Stack);
        }

        if (Input.IsActionJustPressed("use") && !_isAiming && _equippedItem != null)
		{
			_equippedItem.Use(this, _equippedSlot.Stack, GetGlobalMousePosition());
		}

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

	/// <summary>
	/// Damages the player.
	/// </summary>
	/// <param name="amount">
	/// The amount of damage to deal.
	/// </param>
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

	/// <summary>
	/// Adds the specified UI to the player's UI.
	/// </summary>
	/// <param name="newUI">
	/// The UI to add.
	/// </param>
	/// <returns>
	/// The UI added to the player.
	/// </returns>
	public Control AddUI(PackedScene newUI)
	{
		Control clone = newUI.Instantiate<Control>();
		_mainUI.AddChild(clone);
		return clone;
	}

	/// <summary>
	/// Adds the specified UI to the player's UI. 
    /// </summary>
    /// <typeparam name="T">
	/// The type of the UI to add. Must be descendant of the Control class.
	/// </typeparam>
    /// <param name="newUI">
	/// The UI to add.
	/// </param>
    /// <returns>
	/// The UI added to the player.
	/// </returns>
    public T AddUI<T>(PackedScene newUI) where T : Control
	{
		return AddUI(newUI) as T;
	}
}
