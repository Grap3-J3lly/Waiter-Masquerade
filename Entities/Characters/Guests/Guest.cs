using System;
using Godot;
using Godot.Collections;

public partial class Guest : CharacterBody3D
{
	// --------------------------------
    //			VARIABLES	
    // --------------------------------
	[Export]
	private Node3D hand;

	[Export]
	private Mask mask;

	[Export]
	private MeshInstance3D bodyMesh;

	public enum CloakElements
	{
		Back = 0,
		Front = 1,
		Outline = 2
	}
	[Export]
	private Dictionary<CloakElements, Array<Texture2D>> cloaks = new Dictionary<CloakElements, Array<Texture2D>>();

	// --------------------------------
    //			PROPERTIES	
    // --------------------------------

	public Mask Mask { get => mask; }

	// --------------------------------
	//		STANDARD FUNCTIONS	
    // --------------------------------

	public override void _Ready()
	{
		mask.Setup();
		AssignCloak();
	}

	public void TakeDrink(Drink drink)
	{
		drink.Reparent(hand, keepGlobalTransform: false);
		drink.Position = Vector3.Zero;
		drink.DisableInteractions();
		drink.BeginDespawning();
	}

	private void AssignCloak()
	{
		Random rand = new Random();
		int cloakIndex = rand.Next(cloaks[(CloakElements)0].Count);

		Mesh localMesh = (Mesh)bodyMesh.Mesh.Duplicate();
		Material localMaterial = (Material)((Material)localMesh.Get("material")).Duplicate();
		
		localMaterial.Set("shader_parameter/front_texture", cloaks[CloakElements.Front][cloakIndex]);
		localMaterial.Set("shader_parameter/back_texture", cloaks[CloakElements.Back][cloakIndex]);
		localMesh.Set("material", localMaterial);
		bodyMesh.Mesh = localMesh;
	}
}
