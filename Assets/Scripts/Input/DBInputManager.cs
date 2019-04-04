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
	Power = 10,
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
				return players.Keys.FirstOrDefault(p => players[p] == MainController);
			}
			return null;
		}
	}

	/// <summary>
	/// Gets the main controller.
	/// </summary>
	public static ControllerActions MainController
	{
		get
		{
			if (controllers.Count > 0)
			{
				return controllers[0];
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
			return MainController.Device == null;
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

	/// <summary>
	/// Whether or not input has been restricted. 
	/// </summary>
	public bool restrictInput = false;

	/// <summary>
	/// Whether or not the game was paused the previous frame. 
	/// </summary>
	private bool wasPaused = false;

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
			foreach (SpriteMeshInstance s in pc.Sprites)
			{
				s.sortingOrder += 1000 * pc.CharIDInt;
			}
		}
	}

	/// <summary>
	/// Remove a character from the list of playable characters.
	/// </summary>
	public static void Unregister(PlayerController pc)
	{
		players.Remove(pc);
	}

	public PlayerController GetPlayerController(Character c)
	{
		return players.Keys.FirstOrDefault(p => p.CharID == c);
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
		if (IsControlled(pc) && (!moveInput || pc.AcceptingMoveInput)
			&& !instance.restrictInput //(!instance.restrictInput || input == PlayerInput.Left || input == PlayerInput.Right)
			&& !(instance.wasPaused && input == PlayerInput.Jump))
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
		if (IsControlled(pc))
		{
			if (players[pc].Device == null)
			{
				return SaveManager.saveData.input.KeyBindings[(int)input][0].ToString();
			}
			else
			{
				return players[pc].Device.GetControl(SaveManager.saveData.input.ControllerBindings[(int)input][0]).Handle;
				//return SaveManager.saveData.input.ControllerBindings[(int)input][0].ToString();
			}
		}
		return "None";
	}


	/// <summary>
	/// Cycles the players on input.
	/// </summary>
	public static void CyclePlayers()
	{
		PlayerController player = GetInput(PlayerInput.Swap, InputType.Pressed);
		if (player != null)
		{
			CyclePlayers(player);
		}
	}


	/// <summary>
	/// Cycles the players on input.
	/// </summary>
	public static void CyclePlayers(PlayerController player)
	{
		List<PlayerController> sortedPlayers = players.Keys.ToList();
		sortedPlayers.Sort(delegate (PlayerController p1, PlayerController p2)
		{
			if (p1.CharIDInt < p2.CharIDInt)
			{
				return -1;
			}
			else if (p1.CharIDInt > p2.CharIDInt)
			{
				return 1;
			}
			return 0;
		});
		int newIndex = sortedPlayers.IndexOf(player) + 1;
		if (newIndex == sortedPlayers.Count) {
			newIndex = 0;
		}
		PlayerController newP = null;
		while (newIndex != sortedPlayers.IndexOf(player))
		{
			newP = sortedPlayers.FirstOrDefault(p => p != player && players[p] == null && sortedPlayers.IndexOf(p) == newIndex);
			if (newP != null)
			{
				break;
			}
				newIndex++;
			if (newIndex == sortedPlayers.Count)
			{
				newIndex = 0;
			}
		}
		if (newP != null)
		{
			UserSwappedPlayers(newP, player);
		}
	}


	/// <summary>
	/// Set individual player based on hitting their sort value number key. 
	/// </summary>
	private static void SelectPlayerByNumber()
	{
		if (!instance.restrictInput)
		{
			for (int i = 0; i < Utils.keyCodes.Length; i++)
			{
				if (Input.GetKeyDown(Utils.keyCodes[i]))
				{
					PlayerController current = players.Keys.FirstOrDefault(p => (players[p] != null && players[p].Device == null));
					if (current != null)
					{
						PlayerController newP = players.Keys.FirstOrDefault(p => p.CharIDInt == i && players[p] == null);
						if (newP != null)
						{
							UserSwappedPlayers(newP, current);
							break;
						}
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
			s.sortingOrder -= 1000 * oldP.CharIDInt;
		}
		foreach (SpriteMeshInstance s in newP.Sprites)
		{
			s.sortingOrder += 1000 * newP.CharIDInt;
		}
		CameraController.NewPositionEvent(newP);
		oldP.SwitchedFrom();
		newP.SwitchedTo();
		if (newP.Underground != oldP.Underground)
		{
			UndergroundSwapper.SwapEvent?.Invoke(newP.Underground);
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
		instance.wasPaused = PauseMenu.instance?.Paused ?? false;
	}

	/// <summary>
	/// Waits for the given key to be pressed.
	/// </summary>
	/// <param name="k">The key to be pressed.</param>
	public static IEnumerator WaitForKeypress(PlayerInput p, params PlayerInput[] ps)
	{
		while (!GetInput(MainController, p, InputType.Pressed))
		{
			foreach (PlayerInput pi in ps)
			{
				if (GetInput(MainController, pi, InputType.Pressed))
				{
					yield break;
				}
				yield return null;
			}
		}
	}

	/// <summary>
	/// Whether or not the given player is being controlled. 
	/// </summary>
	public static bool IsControlled(PlayerController p)
	{
		return players[p] != null;
	}

	#endregion

	#endregion

	#region Functions

	protected virtual void Awake()
	{
		if (instance == null)
		{
			instance = this;
			controllers.Add(SetUpController(null, true));
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
	public ControllerActions SetUpController(InputDevice device, bool usesEventSystem)
	{
		ControllerActions c = new ControllerActions();

		c.Device = device;
		if (device != null)
		{
			c.SetControllerBindings();
		}
		if (usesEventSystem)
		{
			InControlInputModule ICIM = GetComponent<InControlInputModule>();
			if (device == null)
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

	public void SetupControllerUI()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		SetSelected.Select?.Invoke();
	}

	public void SetupKeyboardUI()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	private void ToggleController()
	{
		ControllerActions c = null;
		if (InputManager.ActiveDevice.CommandWasPressed && MainController.Device == null && (controllers.Count < 2 || controllers[1].Device != InputManager.ActiveDevice))
		{
			c = SetUpController(InputManager.ActiveDevice, true);
			SetupControllerUI();
		}
		else if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Mouse0)) && MainController.Device != null && (controllers.Count < 2 || controllers[1].Device != null))
		{
			c = SetUpController(null, true);
			SetupKeyboardUI();
		}
		// If using controller for menues and player2 clicks, make sure it doesn't deselect all options. 
		else if (Input.GetKeyDown(KeyCode.Mouse0) && MainController.Device != null)
		{
			SetSelected.Select?.Invoke();
		}
		if (c != null)
		{
			ReassignController(c, 0);
		}
	}

	public void ReassignController(ControllerActions c, int index)
	{
		PlayerController player = players.Keys.FirstOrDefault(p => players[p] == controllers[index]);
		controllers[index] = c;
		if (player != null)
		{
			players[player] = c;
		}
	}

	public void RemoveController(int index)
	{
		PlayerController player = players.Keys.FirstOrDefault(p => players[p] == controllers[index]);
		if (player != null)
		{
			players[player] = null;
		}
		controllers.RemoveAt(index);
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
