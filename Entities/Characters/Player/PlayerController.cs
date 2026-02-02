using Godot;
using System;
using System.Runtime.Serialization.Formatters;

public partial class PlayerController : CharacterBody3D
{
	// --------------------------------
	//		    VARIABLES	
	// --------------------------------
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
	
	// --------------------------------
   	//		STANDARD LOGIC	
   	// --------------------------------
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleCharacterMovement(delta);
		HandleInteractions();

		if(Input.IsActionJustPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
		}

	}

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

		if(@event is InputEventMouseMotion mouseMotion)
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
			GD.Print("Grabbed Drink");
			Area3D drink = (Area3D)raycast.GetCollider();

			if(drink != null)
			{
				drink.Reparent(theHand, keepGlobalTransform:false);
				drink.Position = new Vector3(0,0,0);
			}
		}

	}

	// --------------------------------
	//		MOVEMENT LOGIC	
	// --------------------------------
	
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
}
