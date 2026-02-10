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

	public enum GuestMood
	{
		Satisfactory,
		Adequate,
		NeedsImprovement,
		Dissatisfied
	}
	private GuestMood currentMood = GuestMood.Satisfactory; // Need to tie mood to guesses remaining
	private int score = 0;

	// --------------------------------
    //			CONSTANTS	
    // --------------------------------

	private const int CONST_DefaultStartScore = 0;
	private const int CONST_ScoreIncreaseAmount = 100;

	// --------------------------------
    //			PROPERTIES	
    // --------------------------------

	public bool GamePaused { get => gamePaused; set => gamePaused = value; }
	public bool GameStopped { get => gameStopped; set => gameStopped = value; }

	public GuestMood CurrentMood { get => currentMood; }
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

		uiManager.MoodText = AssignMood(player.Guesses).ToString();
		score = CONST_DefaultStartScore;
		uiManager.ScoreText = score.ToString();
	}

	public void IncreaseScore()
	{
		score += CONST_ScoreIncreaseAmount;
		uiManager.ScoreText = score.ToString();
	}

	private GuestMood AssignMood(int guessesMade)
	{
		float fTotalGuesses = (float)guessAttempts;
		float fGuessesMade = (float)guessesMade;
		float ratio = fGuessesMade / fTotalGuesses;

		GD.Print($"GameManager.cs: Ratio of Guesses Made to Total Allowed: {ratio}");

		GuestMood newMood = GuestMood.Dissatisfied;

		if(ratio < 1.0f)
		{
			newMood = GuestMood.Dissatisfied;
		}
		if(ratio < .75f)
		{
			newMood = GuestMood.NeedsImprovement;
		}
		if(ratio < .5f)
		{
			newMood = GuestMood.Adequate;
		}
		if(ratio < .25f)
		{
			newMood = GuestMood.Satisfactory;
		}

		return newMood;
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
			uiManager.MoodText = AssignMood(player.Guesses).ToString();
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
