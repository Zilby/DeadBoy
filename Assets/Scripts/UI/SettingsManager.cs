using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;
using InControl;

/// <summary>
/// Manages our settings in the options menu.
/// </summary>
public class SettingsManager : MonoBehaviour
{

	[Header("Interactables")]
	/// <summary>
	/// The fullscreen toggle. 
	/// </summary>
	public Toggle fullscreen;
	/// <summary>
	/// The presets dropdown.
	/// </summary>
	public TMP_Dropdown presets;
	/// <summary>
	/// The resolution dropdown.
	/// </summary>
	public TMP_Dropdown resolution;
	/// <summary>
	/// The texture quality dropdown. 
	/// </summary>
	public TMP_Dropdown texture;
	/// <summary>
	/// The antialiasing dropdown.
	/// </summary>
	public TMP_Dropdown antialias;
	/// <summary>
	/// The vsync dropdown.
	/// </summary>
	public TMP_Dropdown vsync;
	/// <summary>
	/// The player 1 dropdown.
	/// </summary>
	public TMP_Dropdown player1;
	/// <summary>
	/// The player 2 dropdown.
	/// </summary>
	public TMP_Dropdown player2;
	/// <summary>
	/// The master volume slider. 
	/// </summary>
	public Slider master;
	/// <summary>
	/// The music volume slider. 
	/// </summary>
	public Slider music;
	/// <summary>
	/// The soundfx volume slider. 
	/// </summary>
	public Slider soundfx;
	/// <summary>
	/// The apply button. 
	/// </summary>
	public Button apply;

	/// <summary>
	/// List of all available resolutions.
	/// </summary>
	private List<Resolution> resolutions;

	/// <summary>
	/// The main audio mixer. 
	/// </summary>
	private AudioMixer mix;

	/// <summary>
	/// The preset strings. 
	/// </summary>
	private List<string> preStrings = new List<string> { "Low", "Medium", "High", "Custom" };

	/// <summary>
	/// True if updating the settings to the given preset. 
	/// </summary>
	private bool updatingPresets = false;

	/// <summary>
	/// True if updating one of the individual quality settings. 
	/// </summary>
	private bool updatingQualitySetting = false;

	void Start()
	{
		mix = Resources.Load<AudioMixer>("Audio/Mixer");
		foreach (string s in preStrings)
		{
			presets.options.Add(new TMP_Dropdown.OptionData(s));
		}

		resolutions = new List<Resolution>(Screen.resolutions);
		List<string> resStrings = new List<string>();
		foreach (Resolution r in resolutions)
		{
			if (!resStrings.Contains(r.ToString()))
			{
				resStrings.Add(r.ToString());
			}
		}
		foreach (string r in resStrings)
		{
			resolution.options.Add(new TMP_Dropdown.OptionData(r));
		}

		List<string> textTranslations = new List<string> { "Low", "Medium", "High" };
		foreach (string s in textTranslations)
		{
			texture.options.Add(new TMP_Dropdown.OptionData(s));
		}

		List<string> antiTranslations = new List<string> { "None", "Low", "Medium", "High" };
		foreach (string s in antiTranslations)
		{
			antialias.options.Add(new TMP_Dropdown.OptionData(s));
		}

		List<string> vsyncTranslations = new List<string> { "Don't Sync", "Every V-Blank", "Every 2nd V-Blank" };
		foreach (string s in vsyncTranslations)
		{
			vsync.options.Add(new TMP_Dropdown.OptionData(s));
		}

		InitializeControllerLists();

		fullscreen.onValueChanged.AddListener(delegate
		{
			FullToggle();
		});

		presets.onValueChanged.AddListener(delegate
		{
			PreChange();
		});

		resolution.onValueChanged.AddListener(delegate
		{
			ResChange();
		});

		texture.onValueChanged.AddListener(delegate
		{
			TextChange();
		});

		antialias.onValueChanged.AddListener(delegate
		{
			AntiChange();
		});

		vsync.onValueChanged.AddListener(delegate
		{
			VsyncChange();
		});

		player1.onValueChanged.AddListener(delegate
		{
			PlayerChange(0, player1);
		});

		player2.onValueChanged.AddListener(delegate
		{
			PlayerChange(1, player2);
		});

		master.onValueChanged.AddListener(delegate
		{
			MasterChange();
		});

		music.onValueChanged.AddListener(delegate
		{
			MusicChange();
		});

		soundfx.onValueChanged.AddListener(delegate
		{
			SoundfxChange();
		});

		apply.onClick.AddListener(Apply);

		fullscreen.isOn = Screen.fullScreen;//Utils.Fullscreen;
		presets.value = preStrings.IndexOf(SaveManager.saveData.options.presetString);
		resolution.value = SaveManager.saveData.options.resolutionIndex;
		master.value = SaveManager.saveData.options.master;
		music.value = SaveManager.saveData.options.music;
		soundfx.value = SaveManager.saveData.options.soundfx;
		presets.RefreshShownValue();
		resolution.RefreshShownValue();
		texture.RefreshShownValue();
		antialias.RefreshShownValue();
		vsync.RefreshShownValue();
	}

	private void Update()
	{
		InitializeControllerLists();
	}

	/// <summary>
	/// Toggles full screen mode.
	/// </summary>
	public void FullToggle()
	{
		Screen.fullScreen = SaveManager.saveData.options.fullscreen = fullscreen.isOn;
	}

	/// <summary>
	/// Changes the preset.
	/// </summary>
	public void PreChange()
	{
		QualitySettings.SetQualityLevel(QualitySettings.names.ToList().IndexOf(preStrings[presets.value]));
		updatingPresets = true;
		if (updatingQualitySetting)
		{
			TextChange();
			AntiChange();
			VsyncChange();
		}
		else
		{
			texture.value = 2 - QualitySettings.masterTextureLimit;
			antialias.value = (int)Mathf.Log(QualitySettings.antiAliasing, 2f);
			vsync.value = QualitySettings.vSyncCount;
		}
		updatingPresets = false;

		SaveManager.saveData.options.presetString = preStrings[presets.value];
	}

	/// <summary>
	/// Changes the resolution.
	/// </summary>
	public void ResChange()
	{
		Screen.SetResolution(resolutions[resolution.value].width, resolutions[resolution.value].height,
			Screen.fullScreen);
		SaveManager.saveData.options.resolutionIndex = resolution.value;
	}


	/// <summary>
	/// Changes texture quality. 
	/// </summary>
	public void TextChange()
	{
		if (!updatingQualitySetting)
		{
			SetPresetToCustom();
			QualitySettings.masterTextureLimit = SaveManager.saveData.options.texture = 2 - texture.value;
		}
	}


	/// <summary>
	/// Changes the antialiasing setting. 
	/// </summary>
	public void AntiChange()
	{
		if (!updatingQualitySetting)
		{
			SetPresetToCustom();
			QualitySettings.antiAliasing = SaveManager.saveData.options.antialias = (int)Mathf.Pow(2f, antialias.value);
		}
	}


	/// <summary>
	/// Changes the vsync setting. 
	/// </summary>
	public void VsyncChange()
	{
		if (!updatingQualitySetting)
		{
			SetPresetToCustom();
			QualitySettings.vSyncCount = SaveManager.saveData.options.vsync = vsync.value;
		}
	}


	/// <summary>
	/// Sets the preset to custom if not currently updating the presets.
	/// </summary>
	private void SetPresetToCustom()
	{
		if (!updatingPresets)
		{
			updatingQualitySetting = true;
			presets.value = preStrings.IndexOf("Custom");
			updatingQualitySetting = false;
		}
	}


	/// <summary>
	/// Changes the master volume. 
	/// </summary>
	public void MasterChange()
	{
		SaveManager.saveData.options.master = master.value;
		mix.SetFloat("MasterVolume", SaveManager.saveData.options.master);
	}


	/// <summary>
	/// Changes the music volume. 
	/// </summary>
	public void MusicChange()
	{
		SaveManager.saveData.options.music = music.value;
		mix.SetFloat("MusicVolume", SaveManager.saveData.options.music);
	}


	/// <summary>
	/// Changes the sound fx volume. 
	/// </summary>
	public void SoundfxChange()
	{
		SaveManager.saveData.options.soundfx = soundfx.value;
		mix.SetFloat("SFXVolume", SaveManager.saveData.options.soundfx);
	}


	/// <summary>
	/// Changes the player's controller. 
	/// </summary>
	public void PlayerChange(int playerIndex, TMP_Dropdown dropdown)
	{
		InputDevice device = InputManager.Devices.FirstOrDefault(d => d.Name == dropdown.options[dropdown.value].text);
		ControllerActions c = DBInputManager.instance.SetUpController(device, playerIndex == 0);
		if (playerIndex == 0)
		{
			if (device == null)
			{
				DBInputManager.instance.SetupKeyboardUI();
			}
			else
			{
				DBInputManager.instance.SetupControllerUI();
			}
		}
		if (dropdown.options[dropdown.value].text == "None")
		{
			DBInputManager.instance.RemoveController(playerIndex);
		}
		else if (DBInputManager.controllers.Count > playerIndex)
		{
			DBInputManager.instance.ReassignController(c, playerIndex);
		}
		else
		{
			DBInputManager.controllers.Add(c);
		}
	}

	/// <summary>
	/// Initializes the controller lists to the current controllers. 
	/// </summary>
	private void InitializeControllerLists()
	{
		player1.ClearOptions();
		player2.ClearOptions();
		// Add all devices to lists not currently used by other players. 
		foreach (InputDevice d in InputManager.Devices)
		{
			if (!(DBInputManager.controllers.Count > 1 && DBInputManager.controllers[1].Device == d))
			{
				player1.options.Add(new TMP_Dropdown.OptionData(d.Name));
			}
			if (!(DBInputManager.controllers.Count > 0 && DBInputManager.MainController.Device == d))
			{
				player2.options.Add(new TMP_Dropdown.OptionData(d.Name));
			}
		}
		string keyboard = "Keyboard";
		string none = "None";
		// Add keyboards if no other player is currently using. 
		if (DBInputManager.controllers.Count < 2 || DBInputManager.controllers[1].Device != null)
		{
			player1.options.Add(new TMP_Dropdown.OptionData(keyboard));
		}
		if (DBInputManager.MainController.Device != null)
		{
			player2.options.Add(new TMP_Dropdown.OptionData(keyboard));
		}
		player2.options.Add(new TMP_Dropdown.OptionData(none));

		player1.value = player1.options.FindIndex(o => o.text == (DBInputManager.MainController.Device != null ? DBInputManager.MainController.Device.Name : keyboard));
		player2.value = player2.options.FindIndex(o => o.text == (DBInputManager.controllers.Count < 2 ? none
																  : (DBInputManager.controllers[1].Device != null ? DBInputManager.controllers[1].Device.Name : keyboard)));

		player1.RefreshShownValue();
		player2.RefreshShownValue();
	}

	/// <summary>
	/// Makes sure to immediately apply any given changes.
	/// </summary>
	public void Apply()
	{
		SaveManager.Save();
	}
}


