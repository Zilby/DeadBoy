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
				foreach (PlayerController p in players.Keys)
				{
					if (players[p] == controllers[0])
					{
						return p;
					}
				}
			}
			return null;
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
	/// Sets up a new controller with the given characteristics. 
	/// </summary>
	public static ControllerActions SetUpController(bool keyboard, bool usesEventSystem)
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
			InControlInputModule ICIM = EventSystem.current.GetComponent<InControlInputModule>();
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
		foreach (PlayerController p in players.Keys)
		{
			if (GetInput(p, input, type, moveInput))
			{
				return p;
			}
		}
		return null;
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

	/*

	void Start()
	{
		StartCoroutine(GeneralTutorial());
	}

	protected IEnumerator GeneralTutorial()
	{
		yield return new WaitForSeconds(1.0f);
		yield return MovementTutorial();
		yield return new WaitForSeconds(0.3f);
		yield return SwapTutorial();
	}

	protected IEnumerator MovementTutorial()
	{
		Vector3 tipOffset = new Vector3(0.0f, 2.7f, 0.0f);
		int t = ToolTips.instance.SetTooltipActive("Press " + KeyBindings[(int)PlayerInput.Left][0] + " and " +
										   KeyBindings[(int)PlayerInput.Right][0] + " to move", MainPlayer.transform.position + tipOffset);
		while (!GetInput(MainPlayer, PlayerInput.Left, InputType.Held) && !GetInput(MainPlayer, PlayerInput.Right, InputType.Held))
		{
			yield return null;
		}

		ToolTips.instance.SetTooltipInactive(t);

		yield return new WaitForSeconds(0.3f);

		t = ToolTips.instance.SetTooltipActive("Press " + KeyBindings[(int)PlayerInput.Jump][0] + " to jump", MainPlayer.transform.position + tipOffset);
		while (!GetInput(MainPlayer, PlayerInput.Jump, InputType.Held))
		{
			ToolTips.instance.SetTooltipPosition(t, MainPlayer.transform.position + tipOffset);
			yield return null;
		}

		ToolTips.instance.SetTooltipInactive(t);
	}


	protected IEnumerator SwapTutorial()
	{
		Vector3 tipOffset = new Vector3(0.0f, 2.7f, 0.0f);
		List<PlayerController> swappedTo = new List<PlayerController>();
		Dictionary<PlayerController, int> tips = new Dictionary<PlayerController, int>();
		PlayerController lastControlled = MainPlayer;

		foreach (PlayerController p in players)
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

		int tabTip = ToolTips.instance.SetTooltipActive("Press " + KeyBindings[(int)PlayerInput.Swap][0] + " to cycle characters", MainPlayer.transform.position + tipOffset);
		while (MainPlayer == lastControlled)
		{
			ToolTips.instance.SetTooltipPosition(tabTip, MainPlayer.transform.position + tipOffset);
			yield return null;
		}
		ToolTips.instance.SetTooltipInactive(tabTip);
	}
	*/


	void Update()
	{
		// Cycle Players on input
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
			int curr = sortedPlayers.IndexOf(player) + 1;
			if (curr >= sortedPlayers.Count)
			{
				curr = 0;
			}
			while (sortedPlayers[curr] != player)
			{
				if (players[sortedPlayers[curr]] == null)
				{
					UserSwappedPlayers(sortedPlayers[curr], player);
					break;
				}
				curr += 1;
				if (curr >= sortedPlayers.Count)
				{
						curr = 0;
				}
			}
		}
		// Set individual player based on hitting their sort value number key. 
		for (int i = 0; i < Utils.keyCodes.Length; i++)
		{
			if (Input.GetKeyDown(Utils.keyCodes[i]))
			{
				foreach (PlayerController p in players.Keys)
				{
					if (players[p] != null && players[p].Device == null)
					{
						foreach (PlayerController p2 in players.Keys)
						{
							if (p2.SORT_VALUE == i && p != p2)
							{
								UserSwappedPlayers(p2, p);
								break;
							}
						}
					}
				}
			}
		}

		CheckPause();
	}

	#endregion
}
