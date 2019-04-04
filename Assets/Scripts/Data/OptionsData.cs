using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// All settings related data. 
/// </summary>
[Serializable]
public class OptionsData
{
	/// <summary>
	/// The quality level preset string. 
	/// </summary>
	public string presetString;
	/// <summary>
	/// Whether fullscreen is active. 
	/// </summary>
	public bool fullscreen;
	/// <summary>
	/// The index of the current texture quality. 
	/// </summary>
	public int texture;
	/// <summary>
	/// The antialiasing setting value.
	/// </summary>
	public int antialias;
	/// <summary>
	/// The current vsync count. 
	/// </summary>
	public int vsync;
	/// <summary>
	/// The index of the current resolution.
	/// </summary>
	public int resolutionIndex;
	/// <summary>
	/// The master volume value. 
	/// </summary>
	public float master;
	/// <summary>
	/// The music volume value. 
	/// </summary>
	public float music;
	/// <summary>
	/// The soundfx volume value. 
	/// </summary>
	public float soundfx;

	public OptionsData()
	{
		presetString = QualitySettings.names[QualitySettings.GetQualityLevel()];
		fullscreen = Screen.fullScreen = true;
		QualitySettings.SetQualityLevel(QualitySettings.names.ToList().IndexOf(presetString));
		texture = QualitySettings.masterTextureLimit;
		antialias = QualitySettings.antiAliasing;
		vsync = QualitySettings.vSyncCount;
		for (int i = 0; i < Screen.resolutions.Length; ++i)
		{
			if (Screen.resolutions[i].Equals(Screen.currentResolution))
			{
				resolutionIndex = i;
			}
		}
		AudioMixer mix = Resources.Load<AudioMixer>("Audio/Mixer");
		if (mix != null)
		{
			mix.GetFloat("MasterVolume", out master);
			mix.GetFloat("MusicVolume", out music);
			mix.GetFloat("SFXVolume", out soundfx);
		}
	}

	public void SetOptions()
	{
		QualitySettings.SetQualityLevel(QualitySettings.names.ToList().IndexOf(presetString));
		Screen.fullScreen = fullscreen;
		List<Resolution> resolutions = new List<Resolution>(Screen.resolutions);
		Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height,
			Screen.fullScreen);
		QualitySettings.masterTextureLimit = texture;
		QualitySettings.antiAliasing = antialias;
		QualitySettings.vSyncCount = vsync;
		AudioMixer mix = Resources.Load<AudioMixer>("Audio/Mixer");
		mix.SetFloat("MasterVolume", master);
		mix.SetFloat("MusicVolume", music);
		mix.SetFloat("SFXVolume", soundfx);
	}
}