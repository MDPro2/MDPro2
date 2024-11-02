//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Plays the specified sound.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Play Sound")]
public class UIPlaySound : MonoBehaviour
{
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		Custom,
		OnEnable,
		OnDisable,
	}
	public enum VolType
	{
		SE,
		Voice,
		BGM
	}

	public AudioClip audioClip;
	public Trigger trigger = Trigger.OnClick;
	public VolType type = VolType.SE;
	[Range(0f, 1f)] public float volume = 1f;
	[Range(0f, 2f)] public float pitch = 1f;

	bool mIsOver = false;

	bool canPlay
	{
		get
		{
			if (!enabled) return false;
			UIButton btn = GetComponent<UIButton>();
			return (btn == null || btn.isEnabled);
		}
	}

	void OnEnable ()
	{
        if (trigger == Trigger.OnEnable)
			NGUITools.PlaySound(audioClip, volume, pitch);
	}

	void OnDisable ()
	{
        if (type == VolType.SE)
            volume = Program.I().setting.seVol;
        else if (type == VolType.Voice)
            volume = Program.I().setting.voiceVol;
        else
            volume = Program.I().setting.bgmVol;
        if (trigger == Trigger.OnDisable)
			NGUITools.PlaySound(audioClip, volume, pitch);
	}

	void OnHover (bool isOver)
	{
		if (trigger == Trigger.OnMouseOver)
		{
			if (mIsOver == isOver) return;
			mIsOver = isOver;
		}
		if(type == VolType.SE)
			volume = Program.I().setting.seVol;
		else if(type == VolType.Voice)
			volume = Program.I().setting.voiceVol;
		else 
			volume = Program.I().setting.bgmVol;
		if (canPlay && ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut)))
			NGUITools.PlaySound(audioClip, volume, pitch);
	}

	void OnPress (bool isPressed)
	{
		if (trigger == Trigger.OnPress)
		{
			if (mIsOver == isPressed) return;
			mIsOver = isPressed;
		}

        if (type == VolType.SE)
            volume = Program.I().setting.seVol;
        else if (type == VolType.Voice)
            volume = Program.I().setting.voiceVol;
        else
            volume = Program.I().setting.bgmVol;

        if (canPlay && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease)))
			NGUITools.PlaySound(audioClip, volume, pitch);
	}

	void OnClick ()
	{
        if (type == VolType.SE)
            volume = Program.I().setting.seVol;
        else if (type == VolType.Voice)
            volume = Program.I().setting.voiceVol;
        else
            volume = Program.I().setting.bgmVol;
        if (canPlay && trigger == Trigger.OnClick)
			NGUITools.PlaySound(audioClip, volume, pitch);
	}

	void OnSelect (bool isSelected)
	{
		if (canPlay && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
			OnHover(isSelected);
	}

	public void Play ()
	{
        if (type == VolType.SE)
            volume = Program.I().setting.seVol;
        else if (type == VolType.Voice)
            volume = Program.I().setting.voiceVol;
        else
            volume = Program.I().setting.bgmVol;
        NGUITools.PlaySound(audioClip, volume, pitch);
	}
}
