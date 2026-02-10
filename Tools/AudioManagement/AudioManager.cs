using Godot;
using Godot.Collections;
using System;

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
        ItemInteract_Two
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

    #region Ambience
    // Ambience Group
    [ExportGroup("Ambience")]
    [Export]
    private AudioStreamPlayer ambienceAudioPlayer;
    private AudioStreamPlaybackPolyphonic ambiencePlaybackController;

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

        // Start AudioStreamPlayer, make it actively listen for new sounds added to the Polyphonic.
        // sfxAudioPlayer.Stream = new AudioStreamPolyphonic();
        // sfxAudioPlayer.Play();
        // sfxPlaybackController = sfxAudioPlayer.GetStreamPlayback() as AudioStreamPlaybackPolyphonic;

        SetupPlaybackController(musicAudioPlayer, out musicPlaybackController);
        SetupPlaybackController(sfxAudioPlayer, out sfxPlaybackController);
        SetupPlaybackController(playerAudioPlayer, out playerPlaybackController);
        SetupPlaybackController(ambienceAudioPlayer, out ambiencePlaybackController);
    }

    private void SetupPlaybackController(AudioStreamPlayer player, out AudioStreamPlaybackPolyphonic playbackController)
    {
        player.Stream = new AudioStreamPolyphonic();
        player.Play();
        playbackController = player.GetStreamPlayback() as AudioStreamPlaybackPolyphonic;
    }

    private void AssignDefaultVolumeLevel()
    {
        double musicValue = (double)SaveSystem.GetDataItem("Settings", "musicVolume", defaultValue: 0.0f);
        AudioServer.SetBusVolumeDb(CONST_MusicAudioBusIndex, (float)Mathf.LinearToDb(musicValue));

        double sfxValue = (double)SaveSystem.GetDataItem("Settings", "sfxVolume", defaultValue: 0.0f);
        AudioServer.SetBusVolumeDb(CONST_SfxAudioBusIndex, (float)Mathf.LinearToDb(sfxValue));

        double playerValue = (double)SaveSystem.GetDataItem("Settings", "playerAudioVolume", defaultValue: 0.0f);
        AudioServer.SetBusVolumeDb(CONST_PlayerAudioBusIndex, (float)Mathf.LinearToDb(playerValue));

        double ambienceValue = (double)SaveSystem.GetDataItem("Settings", "ambienceVolume", defaultValue: 0.0f);
        AudioServer.SetBusVolumeDb(CONST_AmbienceAudioBusIndex, (float)Mathf.LinearToDb(ambienceValue));
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
            playerPlaybackController.PlayStream(playerAudioLibrary[type]);
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
