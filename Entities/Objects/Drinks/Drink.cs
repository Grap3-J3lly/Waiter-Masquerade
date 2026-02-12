using System;
using Godot;
using Godot.Collections;

public partial class Drink : Area3D
{
	private Guest assignedGuest;

	[Export]
	private Material drinkMaterial;

	[Export]
	private Array<Texture2D> drinkOptions = new Array<Texture2D>();

	public Guest AssignedGuest { get => assignedGuest; set => assignedGuest = value; }

	public override void _Ready()
	{
		Setup();
	}

	public override void _Process(double delta)
	{
	}

	private void Setup()
	{
		SelectDrinkOption();
	}

	private void SelectDrinkOption()
	{
		Random rand = new Random();
		int drinkIndex = rand.Next(drinkOptions.Count);

		drinkMaterial.Set("albedo_texture", drinkOptions[drinkIndex]);
	}

	public void PickGuest(Array<Guest> guestOptions)
	{
		Random rand = new Random();
		int guestIndex = rand.Next(guestOptions.Count);
		assignedGuest = guestOptions[guestIndex];
		GD.Print($"Drink.cs: Picked Guest at Index {guestIndex}");
		GD.Print($"Drink.cs: Assigned Guest: {assignedGuest}");
	}
}
