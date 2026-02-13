using System;
using Godot;
using Godot.Collections;

public partial class Drink : Area3D
{
	// --------------------------------
	//		VARIABLES	
    // --------------------------------

	private Guest assignedGuest;

	[Export]
	private MeshInstance3D drinkMesh;
	[Export]
	private Node3D placard;
	[Export]
	private Node3D maskPlacardPlacement;
	private Mesh localMesh;
	[Export]
	private Material drinkMaterial;
	private Material localMaterial;

	[Export]
	private Array<Texture2D> drinkOptions = new Array<Texture2D>();

	[Export]
	private float timeToDespawn = 5f;
	private float despawnTimer;
	private bool startDespawn = false;

	// --------------------------------
	//			PROPERTIES	
    // --------------------------------
	
	public Guest AssignedGuest { get => assignedGuest; set => assignedGuest = value; }
	public Node3D Placard { get => placard; }

	// --------------------------------
	//		STANDARD FUNCTIONS	
    // --------------------------------

	public override void _Ready()
	{
		Setup();
	}

	public override void _Process(double delta)
	{
		if(startDespawn && despawnTimer > 0)
		{
			despawnTimer -= (float)delta;
			if(despawnTimer <= 0)
			{
				GD.Print($"Drink.cs: Despawn Finished");
				QueueFree();
			}
		}
	}

	// --------------------------------
	//		SETUP FUNCTIONS	
    // --------------------------------

	private void Setup()
	{
		despawnTimer = timeToDespawn;
		localMaterial = (Material)drinkMaterial.Duplicate(deep: true);
		localMesh = (Mesh)drinkMesh.Mesh.Duplicate(deep: true);

		localMesh.Set("material", localMaterial);
		drinkMesh.Mesh = localMesh;
		SelectDrinkOption();
	}

	private void SelectDrinkOption()
	{
		Random rand = new Random();
		int drinkIndex = rand.Next(drinkOptions.Count);

		localMaterial.Set("albedo_texture", drinkOptions[drinkIndex]);
	}

	public void PickGuest(Array<Guest> guestOptions)
	{
		Random rand = new Random();
		int guestIndex = rand.Next(guestOptions.Count);
		assignedGuest = guestOptions[guestIndex];
		GD.Print($"Drink.cs: Picked Guest at Index {guestIndex}");
		GD.Print($"Drink.cs: Assigned Guest: {assignedGuest}");

		Mask guestMask = (Mask)assignedGuest.Mask.Duplicate();
		maskPlacardPlacement.AddChild(guestMask);
		guestMask.Position = Vector3.Zero;
		guestMask.Scale = new Vector3(.25f, .25f, .25f);
	}

	// --------------------------------
	//		POST-GUEST LOGIC	
    // --------------------------------

	public void BeginDespawning()
	{
		placard.Visible = false;
		startDespawn = true;
		despawnTimer = timeToDespawn;
	}

	public void DisableInteractions()
	{
		CollisionLayer = 0;
		CollisionMask = 0;
	}
}
