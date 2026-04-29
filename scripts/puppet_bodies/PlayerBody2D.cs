using Godot;
using System.Collections;
using System.Threading;

/// <summary>
/// The body controlled by the player.
/// </summary>
public partial class PlayerBody2D : PuppetBody2D
{
	private bool _isAiming = false;

	private bool _canHeal = true;

	private bool _canUse = true;

	private bool _canToggleInven = true;

	private int _curWeaponIndex = -1;

	private CoroutineSource _healSrc = new();

	[Export]
	private Node2D _swivel = null;

	[Export]
	private Texture2D _crosshair = null;

	[Export]
	private Control _mainUI = null;

	private bool _isRecovering = false;

	private CancellationTokenSource _cts = new();

	private IInteractable _interactable = null;

	[Export]
	private CanvasLayer _uiLayer = null;

	[Export]
	private SaveSource _saveSrc = null;

	[ExportGroup("Aiming")]
	[Export]
	private float _aimSpeedNerf = 0.2f;

	[Export]
	private BodyAim _bodyAim = null;

	[ExportGroup("Health")]
	[Export]
	private int _healTime = 1000;

	[Export]
	private HealthItem _defHealthItem = null;

	[ExportGroup("Mouse Casting")]
	[Export]
	private RayCastInfo _mouseCastSight = null;

	[Export]
	private ShapeCastInfo _mouseCastArea = null;

	[ExportGroup("Inventory")]
	private bool _inventoryOpen = false;
	public bool InventoryOpen { get => _inventoryOpen; }

	[Export]
	private PlayerInventory _inventory = null;
	public PlayerInventory Inventory { get => _inventory; }

	[Export]
	private InventoryUI _inventoryUI = null;

	[Export]
	private Node _useablesParent = null;

	[Signal]
	public delegate void OnStartHealingEventHandler();

	[Signal]
	public delegate void OnHaltHealingEventHandler();

	/// <summary>
	/// Performs a raycast towards the mouse's position,
    /// and scans for any nodes around the mouse if there was no obstruction.
    /// </summary>
    /// <returns>
    /// The results of the cast.
    /// This will be null if the cast was obstructed.
    /// </returns>
    public ShapeCastResults[] LOSPointCast(Vector2 pos)
	{
		PhysicsRayQueryParameters2D sightQuery = _mouseCastSight.CreateQuery(GlobalPosition, pos, [GetRid()]);
		RayCastResults sightResults = RayCastInfo.IntersectRay(this, sightQuery);

		if (sightResults.Collider == null)
		{
			Transform2D transform = new(0f, pos);
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
			_canHeal = false;
			_canToggleInven = false;
			_bodyAim.Show();
			_bodyAim.SetProcess(true);
			_bodyAim.Target = ai;
			CurSpeedFactor = _aimSpeedNerf;
		}
	}

	/// <summary>
	/// Stop aiming at the current target AI.
	/// </summary>
	public void StopAim()
	{
		_isAiming = false;
        _canHeal = true;
        _canToggleInven = true;
        _bodyAim.Target = null;
		_bodyAim.Hide();
		_bodyAim.SetProcess(false);
		CurSpeedFactor = 1f;
    }
	
	private void StopInteract()
	{
		Node col = (Node)_interactable;
		col.TreeExiting -= StopInteract;
        _interactable.StopInteract(this);
		_interactable = null;
	}

	private void Interact()
	{
		ShapeCastResults[] castResults = LOSPointCast(GetGlobalMousePosition());

		if (castResults == null)
		{
			return;
		}

		for (int i = 0; i < castResults.Length; i++)
		{
			IInteractable interactable = castResults[i].Collider as IInteractable;
			
            if (interactable != null)
			{
				bool lockOn = interactable.Interact(this);

				if (lockOn)
				{
					Node col = (Node)castResults[i].Collider;
					col.TreeExiting += StopInteract;
					_interactable = interactable;
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
		_inventory.AimUseSlot(target, bodyPart);
	}

	public void ToggleInventory(bool toggle)
	{
        _inventoryOpen = toggle;
		IsFrozen = toggle;
		_canUse = !toggle;
		_canHeal = !toggle;
        _inventoryUI.Visible = toggle;

		if (!toggle)
		{
			_inventory.StopDrag();
		}
    }

	private IEnumerable Heal()
	{
		StackableInstance neededHealth = new(1, _defHealthItem);

		if (_inventory.HasInstance(neededHealth))
		{
			IsFrozen = true;
			_canUse = false;
			_canToggleInven = false;
			EmitSignal(SignalName.OnStartHealing);

            yield return _healTime;
            _inventory.ChargeInstance(neededHealth);

			if (neededHealth.Amount != 1)
			{
				AddHealth(this, _defHealthItem.HealAmount);
			}

			IsFrozen = false;
			_canUse = true;
			_canToggleInven = true;
        }
	}

	public override void _Ready()
	{
		base._Ready();
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

		if (_canToggleInven && Input.IsActionJustPressed("toggle_inventory"))
		{
			ToggleInventory(!_inventoryOpen);
		}

		if (_canHeal && Input.IsActionJustPressed("heal") && CurHealth < MaxHealth)
		{
			if (_healSrc.IsRunning)
			{
				IsFrozen = false;
				_healSrc.IsCancelled = true;
                EmitSignal(SignalName.OnHaltHealing);
            }
            else
			{
                _healSrc.StartCoroutine(Heal());
            }
        }

        if (_canUse)
        {
            for (int i = 0; i < _inventory.HotbarSize; i++)
            {
                if (Input.IsActionJustPressed("equip_" + i.ToString()))
                {
                    _inventory.Equip(i);
                }
            }

            if (Input.IsActionJustPressed("use") && !_isAiming)
            {
                _inventory.UseSlot(GetGlobalMousePosition());
            }

            if (Input.IsActionJustPressed("reload"))
            {
                _inventory.Reload();
            }
        }

        if (_interactable == null)
        {
			if (Input.IsActionJustPressed("interact") && _canUse)
			{
                Interact();
            }
        }
		else if (_interactable.InteractUpdate((float)delta, this) || Input.IsActionJustPressed("interact"))
        {
			StopInteract();
        }
    }

	public override void _PhysicsProcess(double delta)
	{
		float deltaF = (float)delta;

		if (!IsFrozen)
		{
            if (_isAiming)
            {
                _swivel.LookAt(_bodyAim.Target.GlobalPosition);
            }
            else
            {
                _swivel.LookAt(GetGlobalMousePosition());
            }
        }

		CurDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		MoveUpdate(deltaF);
    }

    protected override void OnKill()
    {
        Callable.From(() => _saveSrc.Load()).CallDeferred();
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

	public ItemNode AddUseable(PackedScene scene)
	{
        ItemNode node = scene.Instantiate<ItemNode>();
		_useablesParent.AddChild(node);
		return node;
	}
}
