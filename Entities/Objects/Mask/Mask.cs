using System;
using Godot;
using Godot.Collections;


public partial class Mask : Node3D
{
	[Export]
	private Array<Texture2D> maskOutlines = new Array<Texture2D>();
	[Export]
	private Array<Texture2D> maskFills = new Array<Texture2D>();
	[Export]
	private Array<Texture2D> maskEmbellishments = new Array<Texture2D>();
	[Export]
	private Array<Texture2D> maskAccessory = new Array<Texture2D>();

	// Mask Outline and Mask Fill are tied to each other in a 1-to-1 relationship, can't swap interchangably
	// Mask Fill can have it's color changed

	public enum MaskElements 
	{
		Outline = 0,
		Embellishment = 1,
		Fill = 2,
		Acessory = 3
	}

	[Export]
	private Dictionary<MaskElements, Godot.Collections.Array> maskMeshMats = new Dictionary<MaskElements, Godot.Collections.Array>();
	
	private Array<MeshInstance3D> meshInstances = new Array<MeshInstance3D>();
	private Array<Mesh> localMeshes = new Array<Mesh>();
	private Array<Material> localMaterials = new Array<Material>();

	
	public override void _Ready()
	{
		
	}

	public override void _Process(double delta)
	{
	}

	public void Setup()
	{
		GenerateLocalElements();
		GenerateMask();
	}

	private void GenerateLocalElements()
	{
		for(int index = 0; index < Enum.GetValues<MaskElements>().Length; index++)
		{
			Godot.Collections.Array maskElements = maskMeshMats[(MaskElements)index];

			if(maskElements.Count > 2)
			{
				GD.PrintErr($"Mask.cs: Mask Elements are too long, shorten to size 2");
			}
			// Index 0 - MeshInstance + Mesh
			GD.Print($"Mask.cs: Mask Element @ 0: {maskElements[0].GetType()}");
			MeshInstance3D meshInstance = GetNode<MeshInstance3D>((NodePath)maskElements[0]);
			GD.Print($"Mask.cs: MeshInstance Local Val: {meshInstance}");
			if(meshInstance == null)
			{
				GD.PrintErr($"Mask.cs: Mesh Instance is invalid for {(MaskElements)index}");
			}
			meshInstances.Add(meshInstance);
			Mesh newMesh = (Mesh)meshInstance.Mesh.Duplicate();
			meshInstance.Mesh = newMesh;
			localMeshes.Add(newMesh);
			
			
			// Index 1 - Material
			Material material = (Material)maskElements[1];
			if(material == null)
			{
				GD.PrintErr($"Mask.cs: Material is invalid");
			}
			Material newMat = (Material)material.Duplicate();
			meshInstance.Mesh.Set("material", newMat);
			localMaterials.Add(newMat);
		}

		GD.Print($"Mask.cs: Local Mesh Size: {localMeshes.Count} Local Materials Size: {localMaterials.Count} Mesh Instances Size: {meshInstances.Count}");
	}

	private void GenerateMask()
	{
		Random rand = new Random();
		int outlineFillIndex = rand.Next(maskOutlines.Count);
		int embelIndex = rand.Next(maskEmbellishments.Count);
		int accessIndex = rand.Next(maskAccessory.Count);
		Color fillColor = new Color(rand.NextSingle(), rand.NextSingle(), rand.NextSingle());
		
		// Outline
		localMaterials[(int)MaskElements.Outline].Set("albedo_texture", maskOutlines[outlineFillIndex]);
		// Embellishment
		localMaterials[(int)MaskElements.Embellishment].Set("albedo_texure", maskEmbellishments[embelIndex]);
		// Fill
		localMaterials[(int)MaskElements.Fill].Set("albedo_texture", maskFills[outlineFillIndex]);
		localMaterials[(int)MaskElements.Fill].Set("albedo_color", fillColor);
		// Accessory
		localMaterials[(int)MaskElements.Acessory].Set("albedo_texture", maskAccessory[accessIndex]);
	}
}
