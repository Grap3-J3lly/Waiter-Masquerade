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
	[Export]
	private MeshInstance3D outlineMesh;

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
		ToggleOutline(false);
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

		Mesh localCloakMesh = (Mesh)bodyMesh.Mesh.Duplicate();
		Mesh localOutlineMesh = (Mesh)outlineMesh.Mesh.Duplicate();
		Material localCloakMaterial = (Material)((Material)localCloakMesh.Get("material")).Duplicate();
		Material localOutlineMaterial = (Material)((Material)localOutlineMesh.Get("material")).Duplicate();
		
		localCloakMaterial.Set("shader_parameter/front_texture", cloaks[CloakElements.Front][cloakIndex]);
		localCloakMaterial.Set("shader_parameter/back_texture", cloaks[CloakElements.Back][cloakIndex]);
		localCloakMesh.Set("material", localCloakMaterial);
		bodyMesh.Mesh = localCloakMesh;

		localOutlineMaterial.Set("shader_parameter/outline_texture", cloaks[CloakElements.Outline][cloakIndex]);
		localOutlineMesh.Set("material", localOutlineMaterial);
		outlineMesh.Mesh = localOutlineMesh;
	}

	public void ToggleOutline(bool isVisible)
	{
		outlineMesh.Visible = isVisible;
	}
}
