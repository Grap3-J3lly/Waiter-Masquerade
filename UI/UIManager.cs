using Godot;
using Godot.Collections;
using System;

public partial class UIManager : CanvasLayer
{
	public static UIManager Instance  { get; private set; }
	
	[Export]
	private RichTextLabel scoreText;
	[Export]
	private RichTextLabel timerText;
	[Export]
	private RichTextLabel moodText;

	[Export]
	private TextureRect moodImage;

	[Export]
	private Array<Texture2D> moodImages = new Array<Texture2D>();

	public string ScoreText { get => scoreText.Text; set => scoreText.Text = value; }
	public string TimerText { get => timerText.Text; set => timerText.Text = value; }
	public string MoodText { get => moodText.Text; set => moodText.Text = value; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void UpdateMoodUI(GameManager.GuestMood currentMood)
	{
		MoodText = currentMood.ToString();
		moodImage.Texture = moodImages[(int)currentMood];
	}
}
