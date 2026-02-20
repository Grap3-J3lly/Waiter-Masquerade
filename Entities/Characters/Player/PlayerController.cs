using System;
using Godot;

public partial class PlayerController : CharacterBody3D
{
	// --------------------------------
	//		    VARIABLES	
	// --------------------------------
	private GameManager gameManager;
	private AudioManager audioManager;

	[Export]
	private float speed = 5.0f;
	[Export]
	private Camera3D mainCam;
	[Export]
	private Vector2 mouseSensitivity = new Vector2(.002f, .2f);
	[Export]
	private Vector2 mouseClampAngles = new Vector2(-45.0f, 45.0f);
	[Export]
	private RayCast3D raycast;
	[Export]
	private Node3D theHand;
	private Drink heldDrink;
	private int guesses = 0;
	private bool isFootstepStreamPlaying;
	private Random random;
	private Guest previousGuest;

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
		audioManager = AudioManager.Instance;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		random = new Random();
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
			// mainCam.RotateX(-mouseMotion.Relative.Y * MouseSensitivity);
			
			float pitchChange = -mouseMotion.Relative.Y * mouseSensitivity.Y;
			Vector3 currentRotation = mainCam.RotationDegrees;
			currentRotation.X += pitchChange;
			currentRotation.X = Mathf.Clamp(currentRotation.X, mouseClampAngles.X, mouseClampAngles.Y);
			mainCam.RotationDegrees = currentRotation;
			RotateY(-mouseMotion.Relative.X * mouseSensitivity.X);
		}
	}

	// --------------------------------
	//		INPUT LOGIC	
	// --------------------------------

	private void HandleInteractions()
	{
		if(previousGuest != null)
		{
			previousGuest.ToggleOutline(false);
		}
		
		if(raycast.IsColliding())
		{
			GodotObject raycastObject = raycast.GetCollider();
			Guest potentialGuest = raycastObject as Guest;

			if(potentialGuest != null)
			{
				potentialGuest.ToggleOutline(true);
				previousGuest = potentialGuest;
			}
		}

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

		////audio stream handling
		//if player is moving and footsteps are not playing
		if(velocity != Vector3.Zero && isFootstepStreamPlaying == false)
		{
			//start playing footsteps
			audioManager.PlayPlayerAudio_Global(AudioManager.PlayerAudioType.Footstep);
			isFootstepStreamPlaying = true;
		}
		//if player is not moving and the audio is playing
		else if (velocity == Vector3.Zero && isFootstepStreamPlaying == true)
		{
			//stop playing footsteps
			audioManager.StopPlayerAudio_Global(AudioManager.PlayerAudioType.Footstep);
			isFootstepStreamPlaying = false;
		}
	}

	// --------------------------------
	//			INTERACTIONS	
	// --------------------------------

	private void HandleDrinkInteraction(Drink drink)
	{
		if(heldDrink != null)
		{
			return;
		}
		drink.Reparent(theHand, keepGlobalTransform:false);
		drink.Position = Vector3.Zero;
		drink.RotationDegrees = new Vector3(0, 180, 0);
		heldDrink = drink;
		AudioManager.Instance.PlaySFX_Global(AudioManager.SFXType.ItemInteract_One);
		AudioManager.Instance.PlaySFX_Global(AudioManager.SFXType.ItemInteract_Two);
		gameManager.ResetDrinkTimer();
	}

	private void HandleGuestInteraction(Guest guest)
	{
		GD.Print($"PlayerController.cs: Selected Guest: {guest}, AssignedGuest: {heldDrink.AssignedGuest}");

		if(heldDrink.AssignedGuest == guest)
		{
			guest.TakeDrink(heldDrink);

			if(guesses > 0)
			{
				--guesses;
			}
			gameManager.IncreaseScore();
			heldDrink = null;

			//play correct guess audio
			audioManager.PlaySFX_Global(AudioManager.SFXType.Guess_Correct);
			//int rand = 
			audioManager.PlayNPCAudio_Global((AudioManager.NPCAudioType)random.Next(0,3));
		}
		else
		{
			++guesses;

			//play wrong guess audio
			audioManager.PlaySFX_Global(AudioManager.SFXType.Guess_Wrong);
			audioManager.PlayNPCAudio_Global((AudioManager.NPCAudioType)random.Next(2,6));
		}
		gameManager.HandleGameOver();
	}
}
