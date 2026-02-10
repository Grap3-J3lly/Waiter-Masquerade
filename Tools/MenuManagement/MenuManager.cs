using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class MenuManager : CanvasLayer
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------
    [Export]
    private Array<Control> menus = new Array<Control>();
    private Control currentMenu;
    private Control previousMenu;

    [Export]
    private Godot.Collections.Dictionary<GeneralButton, Godot.Collections.Array> menuButtons = new Godot.Collections.Dictionary<GeneralButton, Godot.Collections.Array>();

    [Export]
    private Array<GeneralButton> levelButtons = new Array<GeneralButton>();

    // --------------------------------
    //			CONSTANTS   	
    // --------------------------------
    private const int CONST_MainMenuIndex = 0;
    private const int CONST_WinLosePauseIndex = 3;
    
    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public static MenuManager Instance { get; private set; }

    public Control CurrentMenu { get; }

    public Array<Control> Menus { get => menus; }
    public Control PreviousMenu { get => previousMenu; set => previousMenu = value; }
    

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
    {
        Instance = this;
        base._Ready();
        previousMenu = menus[CONST_MainMenuIndex];

        OpenMenu(CONST_MainMenuIndex);

        foreach(KeyValuePair<GeneralButton, Godot.Collections.Array> buttonPair in menuButtons)
        {
            GeneralButton currentButton = buttonPair.Key;
            // GD.Print($"MenuManager.cs: Setting up action for button: {currentButton.Name}");
            int currentMenuIndex = (int)buttonPair.Value[0];
            bool clearPreviousMenuValue = (bool)buttonPair.Value[1];
            
            Action buttonAction = () => PressButton_OpenMenuByIndex(currentMenuIndex, clearPreviousMenuValue);
            currentButton.Pressed += buttonAction;
        }

        if(levelButtons.Count > 0)
        {
            GD.Print($"MenuManager.cs: Levelbuttons Size: {levelButtons.Count}");
            for(int i = 0; i < levelButtons.Count; i++)
            {
                // Need to create a local copy of current index so value put into lambda doesn't change
                int currentIndex = i;
                Action buttonAction = () => { PressButton_PlayLevel(levelIndex: currentIndex); };
                levelButtons[i].Pressed += buttonAction;
            }
        }
        
    }

    // --------------------------------
    //		    MENU LOGIC	
    // --------------------------------

    // Disables all menus except one provided
    public void OpenMenu(int menuIndex, bool clearPreviousMenu = false)
    {
        CloseMenus(clearPreviousMenu);
        menus[menuIndex].Visible = true;
        currentMenu = menus[menuIndex];
    }

    public void CloseMenus(bool clearPreviousMenu = false)
    {
        foreach (Control menu in menus)
        {
            GD.Print($"MenuManager.cs: Closing Menu: {menu.Name}");
            if (menu.Visible)
            {
                previousMenu = menu;
            }
            menu.Visible = false;
        }

        if (clearPreviousMenu)
        {
            previousMenu = null;
        }
        currentMenu = null;
    }

    public void OpenWinLosePauseScreen(bool gameOver, bool gameWon = false)
    {
        WinLosePauseScreen wlpScreen = (WinLosePauseScreen)menus[CONST_WinLosePauseIndex];
        WinLosePauseScreen.ScreenMode newMode;

        if(gameOver)
        {
            newMode = gameWon ? WinLosePauseScreen.ScreenMode.Win : WinLosePauseScreen.ScreenMode.Lose;
        }
        else
        {
            newMode = WinLosePauseScreen.ScreenMode.Pause;
        }

        wlpScreen.Setup(newMode, GameManager.Instance.Score, GameManager.Instance.CurrentMood.ToString());
        OpenMenu(CONST_WinLosePauseIndex);
        Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
    }

    // --------------------------------
    //		    BUTTON LOGIC	
    // --------------------------------

    // This is probably a temporary functionality (famous last words) until I have a more robust menu management system
    public void PressButton_OpenMenuByIndex(int menuIndex, bool clearPreviousMenu = false)
    {
        GD.Print($"MenuManager.cs: Opening Menu via Button Press. MenuIndex: {menuIndex} Clear Previous Menu? {clearPreviousMenu}");
        OpenMenu(menuIndex, clearPreviousMenu);
    }

    public void PressButton_Back()
    {
        Control previousMenu = PreviousMenu;
    
        if (previousMenu != null)
        {
            OpenMenu(Menus.IndexOf(previousMenu));
        }
        else
        {
            GD.Print($"MenuManager.cs: Closing Menus in Back Button");
            CloseMenus();
        }
    }

    public void PressButton_Home()
    {
        SceneManager.LoadScene(sceneIndex: SceneManager.Instance.HomeSceneIndex);
    }

    public void PressButton_PlayLevel(int levelIndex)
    {
        // Adjusting from 0-based index to 1-based index
        ++levelIndex;
        GD.Print($"MenuManager.cs: Assigned Level Index: {levelIndex}");
        CloseMenus(clearPreviousMenu: true);
        SceneManager.LoadScene(levelIndex, unloadPreviousScene: false);
    }

    public void PressButton_ReloadCurrentLevel()
    {
        GD.Print($"MenuManager.cs: Reloading Current Level");
        SceneManager.UnloadScene(SceneManager.Instance.CurrentScene);
        SceneManager.LoadScene(SceneManager.Instance.CurrentSceneIndex, unloadPreviousScene: false);
        CloseMenus(clearPreviousMenu: true);
        // CallDeferred("CloseMenus", true);
    }

    public void PressButton_Quit()
    {
        GetTree().Quit();
    }

    public void PressButton_Pause()
    {
        GD.Print($"MenuManager.cs: Pause Button Clicked");
        GameManager gameManager = GameManager.Instance;
        gameManager.Pause(!gameManager.GamePaused);
        // Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
    }
}
