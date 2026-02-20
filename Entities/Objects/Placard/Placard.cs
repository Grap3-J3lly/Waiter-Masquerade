using Godot;
using System;

[GlobalClass]
public partial class Placard : MeshInstance3D
{
	[Export]
	private RichTextLabel tableNumLabel;
	[Export]
	private int tableNum = 1;

	public override void _Ready()
	{
		tableNumLabel.Text = tableNum.ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
