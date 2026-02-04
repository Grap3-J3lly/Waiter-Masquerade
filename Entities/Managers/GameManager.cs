using Godot;
using System;

public partial class GameManager : Node
{
	public static GameManager Instance;
	[Export]
	private PlayerController player;

	[Export]
	private int guessAttempts = 3;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		DetermineGameOver();
	}

	public void DetermineGameOver()
	{
		if(player.Guesses >= guessAttempts)
		{
			GD.Print($"GameManager.cs: Game Over. You Lose.");
		}
	}
}
