using Godot;
using System;

public partial class WinLosePauseScreen : Control
{
	public enum ScreenMode
	{
		Win,
		Lose,
		Pause
	}
	private ScreenMode assignedScreenMode = ScreenMode.Pause;


	[Export]
	private GeneralButton resumeButton;
	[Export]
	private GeneralButton retryButton;

	[Export]
	private RichTextLabel titleDataLabel;
	[Export]
	private RichTextLabel scoreDataLabel;
	[Export]
	private RichTextLabel moodDataLabel;

	private const string CONST_TitleText_Pause = "Game Paused";
	private const string CONST_TitleText_Lose = "Game Lost";
	private const string CONST_TitleText_Win = "Game Won";

	public override void _Ready()
	{
		Setup(ScreenMode.Pause, 0, "");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Setup(ScreenMode newScreenMode, int currentScore, string currentMood)
	{
		assignedScreenMode = newScreenMode;
		resumeButton.Visible = assignedScreenMode == ScreenMode.Pause;
		retryButton.Visible = assignedScreenMode != ScreenMode.Pause;

		scoreDataLabel.Text = currentScore.ToString();
		moodDataLabel.Text = currentMood;

		switch(assignedScreenMode)
		{
			case ScreenMode.Pause:
			titleDataLabel.Text = CONST_TitleText_Pause;
			break;
			case ScreenMode.Lose:
			titleDataLabel.Text = CONST_TitleText_Lose;
			break;
			case ScreenMode.Win:
			titleDataLabel.Text = CONST_TitleText_Win;
			break;
		}
	}
}
