using Godot;
using Godot.Collections;
using System;
using System.Runtime.CompilerServices;

public partial class AudioManager : Node
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    #region MUSIC
    // Music 
    [ExportGroup("Music")]
    [Export]
    private AudioStreamPlayer musicAudioPlayer;
    private AudioStreamPlaybackPolyphonic musicPlaybackController;

    public enum MusicType
    {
        Background,
        InMenu
    }

    [Export]
    public Dictionary<MusicType, AudioStream> musicLibrary = new Dictionary<MusicType, AudioStream>();
    #endregion

    #region SFX
    // SFX 
    [ExportGroup("SFX")]
    [Export]
    private AudioStreamPlayer sfxAudioPlayer;
    private AudioStreamPlaybackPolyphonic sfxPlaybackController;

    public enum SFXType
    {
        None,
        OnHit,
        OnDestroy,
        UI_Interact,
        TriggerDefaultPowerup,
        ItemInteract_One,
        ItemInteract_Two,
        Guess_Correct,
        Guess_Wrong

    }

    [Export]
    public Dictionary<SFXType, AudioStream> sfxLibrary = new Dictionary<SFXType, AudioStream>();
    #endregion

    #region Player Audio
    // Player Group
    [ExportGroup("Player Audio")]
    [Export]
    private AudioStreamPlayer playerAudioPlayer;
    private AudioStreamPlaybackPolyphonic playerPlaybackController;

    public enum PlayerAudioType
    {
        Footstep,
    }

    [Export]
    public Dictionary<PlayerAudioType, AudioStream> playerAudioLibrary = new Dictionary<PlayerAudioType, AudioStream>();
    #endregion

    #region NPC
    //NPC Group
    [ExportGroup("NPC")]
    [Export]
    private AudioStreamPlayer npcAudioPlayer;
    private AudioStreamPlaybackPolyphonic npcPlaybackController;

    public enum NPCAudioType
    {
        Guess_Correct_Bark1 = 0,
        Guess_Correct_Bark2 = 1,
        Guess_Correct_Bark3 = 2,
        Guess_Wrong_Bark1 = 3,
        Guess_Wrong_Bark2 = 4,
        Guess_Wrong_Bark3 = 5,
    }

    [Export]
    public Dictionary<NPCAudioType, AudioStream> npcLibrary = new Dictionary<NPCAudioType, AudioStream>();
    #endregion
    
    #region Ambience
    // Ambience Group
    [ExportGroup("Ambience")]
    [Export]
    private AudioStreamPlayer ambienceAudioPlayer;
    private AudioStreamPlaybackPolyphonic ambiencePlaybackController;
    private long playerAudioStreamID;

    public enum AmbienceType
    {
        IdleChatter,
    }

    [Export]
    public Dictionary<AmbienceType, AudioStream> ambienceLibrary = new Dictionary<AmbienceType, AudioStream>();
    #endregion

    // --------------------------------
    //			CONSTANTS	
    // --------------------------------
    
    private const int CONST_MusicAudioBusIndex = 1;
    private const int CONST_SfxAudioBusIndex = 2;
    private const int CONST_PlayerAudioBusIndex = 3;
    private const int CONST_AmbienceAudioBusIndex = 4;

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public static AudioManager Instance { get; private set; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
    {
        base._Ready();
        Instance = this;

        CallDeferred("Setup");
    }

    // --------------------------------
    //		    SETUP LOGIC	
    // --------------------------------

    private void Setup()
    {
        AssignDefaultVolumeLevel();

        SetupPlaybackController(musicAudioPlayer, out musicPlaybackController);
        SetupPlaybackController(sfxAudioPlayer, out sfxPlaybackController);
        SetupPlaybackController(playerAudioPlayer, out playerPlaybackController);
        SetupPlaybackController(ambienceAudioPlayer, out ambiencePlaybackController);
        SetupPlaybackController(npcAudioPlayer, out npcPlaybackController);

        //play BG music
        PlayMusic_Global(MusicType.Background);
    }

    private void SetupPlaybackController(AudioStreamPlayer player, out AudioStreamPlaybackPolyphonic playbackController)
    {
        player.Stream = new AudioStreamPolyphonic();
        player.Play();
        playbackController = player.GetStreamPlayback() as AudioStreamPlaybackPolyphonic;
    }

    private void AssignDefaultVolumeLevel()
    {
        double masterValue = (double)SaveSystem.GetDataItem("Settings", "masterVolume", defaultValue: 0.0f);
        AudioServer.SetBusVolumeDb((int)SettingsManager.AudioSettings.Master, (float)Mathf.LinearToDb(masterValue));

        double musicValue = (double)SaveSystem.GetDataItem("Settings", "musicVolume", defaultValue: 0.0f);
        AudioServer.SetBusVolumeDb((int)SettingsManager.AudioSettings.Music, (float)Mathf.LinearToDb(musicValue));

        double sfxValue = (double)SaveSystem.GetDataItem("Settings", "sfxVolume", defaultValue: 0.0f);
        AudioServer.SetBusVolumeDb((int)SettingsManager.AudioSettings.SFX, (float)Mathf.LinearToDb(sfxValue));
    }

    // --------------------------------
    //		    AUDIO LOGIC	
    // --------------------------------

    public void PlayMusic_Global(MusicType type)
    {
        GD.Print($"AudioManager.cs: Playing Music: {type}");
        
        if(musicAudioPlayer != null )
        {
            musicPlaybackController.PlayStream(musicLibrary[type]);
        }
    }

    public void PlaySFX_Global(SFXType type)
    {
        GD.Print($"AudioManager.cs: Playing SFX: {type}");
        
        if(sfxAudioPlayer != null )
        {
            sfxPlaybackController.PlayStream(sfxLibrary[type]);
        }
    }

    public void PlayPlayerAudio_Global(PlayerAudioType type)
    {
        GD.Print($"AudioManager.cs: Playing Player Audio: {type}");
        
        if(playerAudioPlayer != null )
        {
            playerAudioStreamID = playerPlaybackController.PlayStream(playerAudioLibrary[type]);
        }
    }
    public void StopPlayerAudio_Global(PlayerAudioType type)
    {
        GD.Print($"AudioManager.cs: Stopping Player Audio");
        
        if(playerAudioPlayer != null )
        {
            playerPlaybackController.StopStream(playerAudioStreamID);
        }
    }

    public void PlayNPCAudio_Global(NPCAudioType type)
    {
        GD.Print($"AudioManager.cs: Playing NPC Audio: {type}");
        
        if(npcAudioPlayer != null )
        {
            npcPlaybackController.PlayStream(npcLibrary[type]);
        }
    }

    public void PlayAmbience_Global(AmbienceType type)
    {
        GD.Print($"AudioManager.cs: Playing Ambience: {type}");
        
        if(ambienceAudioPlayer != null )
        {
            ambiencePlaybackController.PlayStream(ambienceLibrary[type]);
        }
    }
}
