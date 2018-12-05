using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Anima2D;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The types of inputs player Controllers will check for.
/// </summary>
public enum PlayerInput
{
	None = -1,
	Left = 0,
	Right = 1,
	Up = 2,
	Down = 3,
	Jump = 4,
	Interact = 5,
	Swap = 6,
	Pause = 7,
	Submit = 8,
	Cancel = 9,
}

/// <summary>
/// What kind of input should be checked for
///</summary>
public enum InputType
{
	Pressed = 0,
	Held = 1,
	Released = 2,
}

public class DBInputManager : MonoBehaviour
{

	#region Static

	#region Fields

	public static DBInputManager instance;

	/// <summary>
	/// Gets the main player.
	/// </summary>
	public static PlayerController MainPlayer
	{
		get
		{
			if (controllers.Count > 0)
			{
				return players.Keys.FirstOrDefault(p => players[p] == controllers[0]);
			}
			return null;
		}
	}

	/// <summary>
	/// Gets whether the main player is on keyboard. 
	/// </summary>
	public static bool MainIsKeyboard
	{
		get
		{
			return controllers[0].Device == null;
		}
	}

	/// <summary>
	/// All of the available player controllers. 
	/// </summary>
	public static List<ControllerActions> controllers = new List<ControllerActions>();

	/// <summary>
	/// All of the available players and their associated controller actions
	/// </summary>
	public static Dictionary<PlayerController, ControllerActions> players = new Dictionary<PlayerController, ControllerActions>();


	#endregion

	#region Functions

	/// <summary>
	/// Adds a character to the list of playable characters.
	/// Optionally sets the character as the controlled one.
	/// Should be called in awake
	/// </summary>
	public static void Register(PlayerController pc, int initial)
	{
		players[pc] = null;
		if (initial >= 0 && initial < controllers.Count)
		{
			players[pc] = controllers[initial];
		}
	}

	/// <summary>
	/// Remove a character from the list of playable characters.
	/// </summary>
	public static void Unregister(PlayerController pc)
	{
		players.Remove(pc);
	}

	/// <summary>
	/// Gets whether the given input was pressed this frame. 
	/// </summary>
	/// <returns><c>true</c>, if input was received, <c>false</c> otherwise.</returns>
	/// <param name="pc">The player that's receiving input.</param>
	/// <param name="input">The input to be receieved.</param>
	/// <param name="moveInput">Whether or not the player must be accepting move input.</param>
	public static bool GetInput(PlayerController pc, PlayerInput input, InputType type, bool moveInput = true)
	{
		if (players[pc] != null && (!moveInput || pc.AcceptingMoveInput))
		{
			return GetInput(players[pc], input, type);
		}
		return false;
	}

	/// <summary>
	/// Gets whether the given input was pressed this frame. 
	/// </summary>
	/// <returns><c>true</c>, if input was received, <c>false</c> otherwise.</returns>
	/// <param name="input">The input to be receieved.</param>
	public static bool GetInput(ControllerActions c, PlayerInput input, InputType type)
	{
		if (c.Device == null)
		{
			foreach (KeyCode k in SaveManager.saveData.input.KeyBindings[(int)input])
			{
				if ((type == InputType.Held && Input.GetKey(k)) ||
					(type == InputType.Pressed && Input.GetKeyDown(k)) ||
					(type == InputType.Released && Input.GetKeyUp(k)))
				{
					return true;
				}
			}
		}
		else
		{
			if ((type == InputType.Held && c.actions[input].IsPressed) ||
				(type == InputType.Pressed && c.actions[input].WasPressed) ||
				(type == InputType.Released && c.actions[input].WasReleased))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Gets whether the given input was pressed this frame. 
	/// </summary>
	/// <returns><c>true</c>, if input was received, <c>false</c> otherwise.</returns>
	/// <param name="input">The input to be receieved.</param>
	public static PlayerController GetInput(PlayerInput input, InputType type, bool moveInput = false)
	{
		return players.Keys.FirstOrDefault(p => GetInput(p, input, type, moveInput));
	}

	/// <summary>
	/// Gets the name of the input button for the given player player controller's action. 
	/// </summary>
	public static string GetInputName(PlayerController pc, PlayerInput input)
	{
		if (players[pc] != null)
		{
			if (players[pc].Device == null)
			{
				return SaveManager.saveData.input.KeyBindings[(int)input][0].ToString();
			}
			else
			{
				return SaveManager.saveData.input.ControllerBindings[(int)input][0].ToString();
			}
		}
		return "None";
	}


	/// <summary>
	/// Cycles the players on input.
	/// </summary>
	private static void CyclePlayers()
	{
		PlayerController player = GetInput(PlayerInput.Swap, InputType.Pressed);
		if (player != null)
		{
			List<PlayerController> sortedPlayers = players.Keys.ToList();
			sortedPlayers.Sort(delegate (PlayerController p1, PlayerController p2)
			{
				if (p1.SORT_VALUE < p2.SORT_VALUE)
				{
					return 1;
				}
				else if (p1.SORT_VALUE > p2.SORT_VALUE)
				{
					return -1;
				}
				return 0;
			});
			PlayerController newP = sortedPlayers.FirstOrDefault(
				p => (p != player && players[p] == null &&
					  ((sortedPlayers.IndexOf(p) == sortedPlayers.IndexOf(player) + 1) ||
					   (sortedPlayers.IndexOf(p) == 0 && sortedPlayers.IndexOf(player) == sortedPlayers.Count - 1))));
			if (newP != null)
			{
				UserSwappedPlayers(newP, player);
			}
		}
	}


	/// <summary>
	/// Set individual player based on hitting their sort value number key. 
	/// </summary>
	private static void SelectPlayerByNumber()
	{
		for (int i = 0; i < Utils.keyCodes.Length; i++)
		{
			if (Input.GetKeyDown(Utils.keyCodes[i]))
			{
				PlayerController current = players.Keys.FirstOrDefault(p => (players[p] != null && players[p].Device == null));
				if (current != null)
				{
					PlayerController newP = players.Keys.FirstOrDefault(p => p.SORT_VALUE == i && current != p);
					if (newP != null)
					{
						UserSwappedPlayers(newP, current);
						break;
					}
				}
			}
		}
	}


	/// <summary>
	/// Swaps control of the old player for the new player. 
	/// </summary>
	private static void UserSwappedPlayers(PlayerController newP, PlayerController oldP)
	{
		players[newP] = players[oldP];
		players[oldP] = null;
		foreach (SpriteMeshInstance s in oldP.Sprites)
		{
			s.sortingOrder -= 1000 * oldP.SORT_VALUE;
		}
		foreach (SpriteMeshInstance s in newP.Sprites)
		{
			s.sortingOrder += 1000 * newP.SORT_VALUE;
		}
		CameraController.movingToNewPosition = true;
		oldP.indicator.Hide();
		newP.SwitchedTo();
		if (newP is DeadboyController)
		{
			Interactable.TogglePhased?.Invoke(true);
		}
		else if (oldP is DeadboyController)
		{
			Interactable.TogglePhased?.Invoke(false);
		}
	}


	/// <summary>
	/// Checks whether any player paused. 
	/// </summary>
	static void CheckPause()
	{
		if (GetInput(PlayerInput.Pause, InputType.Pressed) != null)
		{
			PauseMenu.instance.Pause();
		}
	}

	#endregion

	#endregion

	#region Functions

	protected virtual void Awake()
	{
		if (instance == null)
		{
			instance = this;
			controllers.Add(SetUpController(true, true));
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Sets up a new controller with the given characteristics. 
	/// </summary>
	public ControllerActions SetUpController(bool keyboard, bool usesEventSystem)
	{
		ControllerActions c = new ControllerActions();

		if (keyboard)
		{
			c.Device = null;
		}
		else
		{
			c.Device = InputManager.ActiveDevice;
			c.SetControllerBindings();
		}
		if (usesEventSystem)
		{
			InControlInputModule ICIM = GetComponent<InControlInputModule>();
			if (keyboard)
			{
				ICIM.SubmitAction = c.actions[PlayerInput.None];
				ICIM.CancelAction = c.actions[PlayerInput.None];
			}
			else
			{
				ICIM.SubmitAction = c.actions[PlayerInput.Submit];
				ICIM.CancelAction = c.actions[PlayerInput.Cancel];
			}
		}

		return c;
	}


	private void ToggleController()
	{
		ControllerActions c = null;
		if (InputManager.ActiveDevice.CommandWasPressed && controllers[0].Device == null)
		{
			c = SetUpController(false, true);
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			SetSelected.Select?.Invoke();
		}
		else if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Mouse0))&& controllers[0].Device != null)
		{
			c = SetUpController(true, true);
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		if (c != null) {
			ReassignController(c, 0);
		}
	}

	private void ReassignController(ControllerActions c, int index)
	{
		PlayerController player = players.Keys.FirstOrDefault(p => players[p] == controllers[index]);
		controllers[index] = c;
		if (player != null)
		{
			players[player] = c;
		}
	}


	/// <summary>
	/// General tutorial for the game. 
	/// </summary>
	public IEnumerator GeneralTutorial()
	{
		yield return new WaitForSeconds(1.0f);
		yield return MovementTutorial();
		yield return new WaitForSeconds(0.3f);
		yield return SwapTutorial();
	}

	/// <summary>
	/// Tutorial for how to move. 
	/// </summary>
	public IEnumerator MovementTutorial()
	{
		Vector3 tipOffset = new Vector3(0.0f, 2.7f, 0.0f);
		int t = ToolTips.instance.SetTooltipActive("Press " + GetInputName(MainPlayer, PlayerInput.Left) + " and " +
												   GetInputName(MainPlayer, PlayerInput.Right) + " to move", MainPlayer.transform.position + tipOffset);
		while (!GetInput(MainPlayer, PlayerInput.Left, InputType.Held) && !GetInput(MainPlayer, PlayerInput.Right, InputType.Held))
		{
			yield return null;
		}

		ToolTips.instance.SetTooltipInactive(t);

		yield return new WaitForSeconds(0.3f);

		t = ToolTips.instance.SetTooltipActive("Press " + GetInputName(MainPlayer, PlayerInput.Jump) + " to jump", MainPlayer.transform.position + tipOffset);
		while (!GetInput(MainPlayer, PlayerInput.Jump, InputType.Held))
		{
			ToolTips.instance.SetTooltipPosition(t, MainPlayer.transform.position + tipOffset);
			yield return null;
		}

		ToolTips.instance.SetTooltipInactive(t);
	}

	/// <summary>
	/// Tutorial for swapping. 
	/// </summary>
	public IEnumerator SwapTutorial()
	{
		Vector3 tipOffset = new Vector3(0.0f, 2.7f, 0.0f);
		List<PlayerController> swappedTo = new List<PlayerController>();
		Dictionary<PlayerController, int> tips = new Dictionary<PlayerController, int>();
		PlayerController lastControlled = MainPlayer;

		if (MainIsKeyboard)
		{
			foreach (PlayerController p in players.Keys)
			{
				if (p != MainPlayer)
				{
					tips[p] = ToolTips.instance.SetTooltipActive("Press " + p.SORT_VALUE + " to control " + p.Name, p.transform.position + tipOffset);
				}
				else
				{
					tips[p] = -1;
				}
			}

			while (swappedTo.Count < players.Count)
			{
				yield return null;
				if (MainPlayer != lastControlled)
				{
					if (tips[MainPlayer] >= 0)
					{
						ToolTips.instance.SetTooltipInactive(tips[MainPlayer]);
						tips[MainPlayer] = -1;
					}

					if (swappedTo.IndexOf(lastControlled) < 0)
					{
						tips[lastControlled] = ToolTips.instance.SetTooltipActive("Press " + lastControlled.SORT_VALUE + " to control " + lastControlled.Name, lastControlled.transform.position + tipOffset);
					}

					if (swappedTo.IndexOf(MainPlayer) < 0)
					{
						swappedTo.Add(MainPlayer);
					}
					lastControlled = MainPlayer;
				}
			}

			yield return new WaitForSeconds(0.3f);
		}

		int tabTip = ToolTips.instance.SetTooltipActive("Press " + GetInputName(MainPlayer, PlayerInput.Swap) + " to cycle characters", MainPlayer.transform.position + tipOffset);
		while (MainPlayer == lastControlled)
		{
			ToolTips.instance.SetTooltipPosition(tabTip, MainPlayer.transform.position + tipOffset);
			yield return null;
		}
		ToolTips.instance.SetTooltipInactive(tabTip);
	}


	void Update()
	{
		CyclePlayers();
		SelectPlayerByNumber();
		CheckPause();
		ToggleController();
	}

	#endregion
}
