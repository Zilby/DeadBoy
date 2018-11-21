using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using System;

public class ControllerActions : PlayerActionSet
{
	public Dictionary<PlayerInput, PlayerAction> actions;

	public ControllerActions()
	{
		actions = new Dictionary<PlayerInput, PlayerAction>();
		foreach (PlayerInput p in Enum.GetValues(typeof(PlayerInput)))
		{
			actions[p] = CreatePlayerAction(p.ToString());
		}
	}

	/// <summary>
	/// Sets up the controller. 
	/// </summary>
	public void SetControllerBindings()
	{
		foreach (PlayerInput p in Enum.GetValues(typeof(PlayerInput)))
		{
			InputControlType[] bindings = SaveManager.saveData.input.ControllerBindings[(int)p];
			actions[p].AddDefaultBinding(Device.GetControl(bindings[0]).Target);
			for (int i = 1; i < bindings.Length; ++i)
			{
				actions[p].AddBinding(new DeviceBindingSource(Device.GetControl(bindings[i]).Target));
			}
		}
	}
}
