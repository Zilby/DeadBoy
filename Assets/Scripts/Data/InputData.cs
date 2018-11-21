using System;
using System.Collections.Generic;
using InControl;
using UnityEngine;

/// <summary>
/// All input related data. 
/// </summary>
[Serializable]
public class InputData
{
	public KeyCode[][] KeyBindings;
	public InputControlType[][] ControllerBindings;

	public InputData()
	{
		SetKeyboardDefaults();
		SetControllerDefaults();
	}

	/// <summary>
	/// Sets the defaults for keybindings.
	/// </summary>
	public void SetKeyboardDefaults()
	{
		KeyBindings = new KeyCode[Enum.GetValues(typeof(PlayerInput)).Length][];
		KeyBindings[(int)PlayerInput.Left] = new KeyCode[] { KeyCode.A, KeyCode.LeftArrow };
		KeyBindings[(int)PlayerInput.Right] = new KeyCode[] { KeyCode.D, KeyCode.RightArrow };
		KeyBindings[(int)PlayerInput.Up] = new KeyCode[] { KeyCode.W, KeyCode.UpArrow };
		KeyBindings[(int)PlayerInput.Down] = new KeyCode[] { KeyCode.S, KeyCode.DownArrow };
		KeyBindings[(int)PlayerInput.Jump] = new KeyCode[] { KeyCode.Space };
		KeyBindings[(int)PlayerInput.Interact] = new KeyCode[] { KeyCode.F, KeyCode.J };
		KeyBindings[(int)PlayerInput.Swap] = new KeyCode[] { KeyCode.E, KeyCode.Tab };
		KeyBindings[(int)PlayerInput.Pause] = new KeyCode[] { KeyCode.P, KeyCode.Escape };
		KeyBindings[(int)PlayerInput.Submit] = new KeyCode[] { KeyCode.Return };
		KeyBindings[(int)PlayerInput.Cancel] = new KeyCode[] { KeyCode.Escape };
	}


	/// <summary>
	/// Sets the defaults for controller keybindings.
	/// </summary>
	public void SetControllerDefaults()
	{
		ControllerBindings = new InputControlType[Enum.GetValues(typeof(PlayerInput)).Length][];
		ControllerBindings[(int)PlayerInput.Left] = new InputControlType[] { InputControlType.LeftStickLeft, InputControlType.DPadLeft };
		ControllerBindings[(int)PlayerInput.Right] = new InputControlType[] { InputControlType.LeftStickRight, InputControlType.DPadRight };
		ControllerBindings[(int)PlayerInput.Up] = new InputControlType[] { InputControlType.LeftStickUp, InputControlType.DPadUp };
		ControllerBindings[(int)PlayerInput.Down] = new InputControlType[] { InputControlType.LeftStickDown, InputControlType.DPadDown };
		ControllerBindings[(int)PlayerInput.Jump] = new InputControlType[] { InputControlType.Action1 };
		ControllerBindings[(int)PlayerInput.Interact] = new InputControlType[] { InputControlType.Action3 };
		ControllerBindings[(int)PlayerInput.Swap] = new InputControlType[] { InputControlType.Action4 };
		ControllerBindings[(int)PlayerInput.Pause] = new InputControlType[] { InputControlType.Command };
		ControllerBindings[(int)PlayerInput.Submit] = new InputControlType[] { InputControlType.Action1 };
		ControllerBindings[(int)PlayerInput.Cancel] = new InputControlType[] { InputControlType.Action2 };
	}
}