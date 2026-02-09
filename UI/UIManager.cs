using Godot;
using System;

public partial class UIManager : CanvasLayer
{
	public static UIManager Instance  { get; private set; }
	
	[Export]
	private RichTextLabel timerText;

	public string TimerText { get => timerText.Text; set => timerText.Text = value; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	
}
