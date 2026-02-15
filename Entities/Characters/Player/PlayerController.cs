using Godot;

public partial class PlayerController : CharacterBody3D
{
	// --------------------------------
	//		    VARIABLES	
	// --------------------------------
	private GameManager gameManager;

	[Export]
	public float speed = 5.0f;
	[Export]
	private Camera3D mainCam;
	[Export]
	public float MouseSensitivity = .002f;
	[Export]
	private RayCast3D raycast;
	[Export]
	private Node3D theHand;
	private Drink heldDrink;
	private int guesses = 0;

	// --------------------------------
	//		    PROPERTIES	
	// --------------------------------
	
	public int Guesses { get => guesses; }

	// --------------------------------
   	//		STANDARD LOGIC	
   	// --------------------------------
	public override void _Ready()
	{
		gameManager = GameManager.Instance;
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _PhysicsProcess(double delta)
	{
		if(!gameManager.GameStopped && !gameManager.GamePaused)
		{
			HandleCharacterMovement(delta);
			HandleInteractions();
		}

		
	}

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
		if(@event.IsActionPressed("ui_cancel"))
		{
			// Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
			
			if(!gameManager.GameStopped)
			{
				gameManager.Pause(!gameManager.GamePaused);
			}
		}
    }

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

		if(@event is InputEventMouseMotion mouseMotion && !gameManager.GameStopped && !gameManager.GamePaused)
		{
			RotateY(-mouseMotion.Relative.X * MouseSensitivity);
			mainCam.RotateX(-mouseMotion.Relative.Y * MouseSensitivity);
		}
	}

	// --------------------------------
	//		INPUT LOGIC	
	// --------------------------------

	private void HandleInteractions()
	{
		if(Input.IsActionJustPressed("primary") && raycast.IsColliding())
		{
			GodotObject raycastObject = raycast.GetCollider();

			Drink potentialDrink = raycastObject as Drink;
			Guest potentialGuest = raycastObject as Guest;

			if(potentialDrink != null)
			{
				HandleDrinkInteraction(potentialDrink);
			}
			if(potentialGuest != null && heldDrink != null)
			{
				HandleGuestInteraction(potentialGuest);
			}
		}

	}
	
	private void HandleCharacterMovement(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	// --------------------------------
	//			INTERACTIONS	
	// --------------------------------

	private void HandleDrinkInteraction(Drink drink)
	{
		drink.Reparent(theHand, keepGlobalTransform:false);
		drink.Position = Vector3.Zero;
		drink.Rotation = new Vector3(0, 180, 0);
		heldDrink = drink;
		AudioManager.Instance.PlaySFX_Global(AudioManager.SFXType.ItemInteract_One);
		gameManager.ResetDrinkTimer();
	}

	private void HandleGuestInteraction(Guest guest)
	{
		GD.Print($"PlayerController.cs: Selected Guest: {guest}, AssignedGuest: {heldDrink.AssignedGuest}");

		if(heldDrink.AssignedGuest == guest)
		{
			guest.TakeDrink(heldDrink);
			guesses--;
			gameManager.IncreaseScore();
			heldDrink = null;
		}
		else
		{
			++guesses;
		}
		gameManager.HandleGameOver();
	}
}
