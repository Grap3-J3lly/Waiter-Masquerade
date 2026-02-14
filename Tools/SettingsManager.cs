using System;
using Godot;
using Godot.Collections;

public partial class SettingsManager : Control
{
    
    [Export]
    private Dictionary<AudioSettings, HSlider> audioSliderByType = new Dictionary<AudioSettings, HSlider>();

    public enum AudioSettings
    {
        Master = 0,
        Music = 1,
        SFX = 2
    }

    public static SettingsManager Instance { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        Instance = this;

        CallDeferred("Setup");
    }

    private void Setup()
    {
        HSlider slider;
        if(audioSliderByType[AudioSettings.Master] != null)
        {
            slider = audioSliderByType[AudioSettings.Master];
            slider.ValueChanged += OnSliderChange_Master;
        }
        if(audioSliderByType[AudioSettings.Music] != null)
        {
            slider = audioSliderByType[AudioSettings.Music];
            slider.ValueChanged += OnSliderChange_Music;
        }
        if(audioSliderByType[AudioSettings.SFX] != null)
        {
            slider = audioSliderByType[AudioSettings.SFX];
            slider.ValueChanged += OnSliderChange_SFX;
        }

        InitialSliderValueAssignment();
    }

    private void InitialSliderValueAssignment()
    {
        double masterValue = (double)SaveSystem.GetDataItem("Settings", "masterVolume", defaultValue: 0.0f);
        double musicValue = (double)SaveSystem.GetDataItem("Settings", "musicVolume", defaultValue: 0.0f);
        double sfxValue = (double)SaveSystem.GetDataItem("Settings", "sfxVolume", defaultValue: 0.0f);

        audioSliderByType[AudioSettings.Master].SetValueNoSignal(masterValue);
        audioSliderByType[AudioSettings.Music].SetValueNoSignal(musicValue);
        audioSliderByType[AudioSettings.SFX].SetValueNoSignal(sfxValue);
    }

    public void OnSliderChange_Master(double value)
    {
        GD.Print($"SettingsManager.cs: Changing Master Volume to: {value}");

        float volume = (float)Mathf.LinearToDb(value);
        AudioServer.SetBusVolumeDb((int)AudioSettings.Master, volume);
        AudioManager.Instance.PlaySFX_Global(AudioManager.SFXType.UI_Interact);

        SaveSystem.AddDataItem("Settings", "masterVolume", value);
        SaveSystem.SaveData("Settings");
    }

    public void OnSliderChange_Music(double value)
    {
        GD.Print($"SettingsManager.cs: Changing Music Volume to: {value}");

        float volume = (float)Mathf.LinearToDb(value);
        AudioServer.SetBusVolumeDb((int)AudioSettings.Music, volume);
        AudioManager.Instance.PlaySFX_Global(AudioManager.SFXType.UI_Interact);

        SaveSystem.AddDataItem("Settings", "musicVolume", value);
        SaveSystem.SaveData("Settings");
    }

    public void OnSliderChange_SFX(double value)
    {
        GD.Print($"SettingsManager.cs: Changing SFX Volume to: {value}");

        float volume = (float)Mathf.LinearToDb(value);
        AudioServer.SetBusVolumeDb((int)AudioSettings.SFX, volume);
        AudioManager.Instance.PlaySFX_Global(AudioManager.SFXType.UI_Interact);

        SaveSystem.AddDataItem("Settings", "sfxVolume", value);
        SaveSystem.SaveData("Settings");
    }
}
