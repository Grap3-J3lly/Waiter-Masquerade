using Godot;
using System;

[GlobalClass]
public partial class GeneralButton : Button
{
    public override void _Ready()
    {
        base._Ready();
        Pressed += OnPress;
    }

    public void OnPress()
    {
        AudioManager.Instance.PlaySFX_Global(AudioManager.SFXType.UI_Interact);
    }
}
