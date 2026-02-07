using Godot;
using System;

public partial class Drink : Area3D
{
	[Export]
	private Guest assignedGuest;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public bool IsAssignedGuest(Guest selectedGuest) 
	{
		return selectedGuest == assignedGuest;
	}
}
