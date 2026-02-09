using Godot;
using System;

public partial class GameManager : Node
{
	// --------------------------------
    //			VARIABLES	
    // --------------------------------

	public static GameManager Instance;
	private MenuManager menuManager;
	private UIManager uiManager;

	[Export]
	private PlayerController player;

	[Export]
	private int guessAttempts = 3;
	
	[Export(PropertyHint.None, "suffix:s")]
	private float gameDuration = 60f;
	private float timeRemaining;
	private TimeSpan time;
	
	[Export]
	private int finalScreenIndex = 3;

	private bool gamePaused = false;
	private bool gameStopped = false;

	public enum CrowdMood
	{
		SATISFACTORY,
		ADEQUATE,
		NEEDS_IMPROVEMENT,
		DISSATISFIED
	}
	private CrowdMood currentMood = CrowdMood.SATISFACTORY; // Need to tie mood to guesses remaining
	private int score = 0;

	// --------------------------------
    //			PROPERTIES	
    // --------------------------------

	public bool GamePaused { get => gamePaused; set => gamePaused = value; }
	public bool GameStopped { get => gameStopped; set => gameStopped = value; }

	public CrowdMood CurrentMood { get => currentMood; }
	public int Score { get => score; }

	// --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		timeRemaining = gameDuration;
		CallDeferred("Setup");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		time = TimeSpan.FromSeconds(timeRemaining);
		string formattedTime = string.Format("{0:D2}:{1:D2}", (int)time.TotalMinutes, time.Seconds);

		if(uiManager != null && timeRemaining > 0 && !gameStopped && !gamePaused)
		{
			uiManager.TimerText = formattedTime;
			timeRemaining -= (float)delta;

			if(timeRemaining <= 0)
			{
				HandleGameOver();
			}
		}
	}

	// --------------------------------
    //		MISC FUNCTIONS	
    // --------------------------------

	private void Setup()
	{
		uiManager = UIManager.Instance;
		menuManager = MenuManager.Instance;
	}

	public void HandleGameOver()
	{
		bool gameWon = false;
		if(player.Guesses >= guessAttempts)
		{
			GD.Print($"GameManager.cs: Game Over. You Lose");
			gameStopped = true;
		}
		else if(timeRemaining <= 0)
		{
			GD.Print($"GameManager.cs: Game Over. You Win");
			gameStopped = true;
			gameWon = true;
		}
		else
		{
			// GD.Print($"GameManager.cs: Game Not Over Yet");
			return;
		}
		menuManager.OpenWinLosePauseScreen(gameStopped, gameWon);
	}

	public void Pause(bool pausingGame)
	{
		gamePaused = pausingGame;
		if(gamePaused)
		{
			menuManager.OpenWinLosePauseScreen(gameStopped);
		}
		else
		{
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
			menuManager.CloseMenus();
		}
	}
}
