using System.Collections;
using System.Collections.Generic;
using Anima2D;
using UnityEngine;
using UnityEngine.Sprites;

/// <summary>
/// The types of inputs player Controllers will check for.
/// </summary>
public enum PlayerInput
{
	Left = 0,
	Right = 1,
	Up = 2,
	Down = 3,
	Jump = 4,
	Interact = 5,
	Swap = 6,
	Pause = 7,
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

public class InputManager : MonoBehaviour
{

	#region Static

	#region Fields

	public static List<List<KeyCode>> KeyBindings
	{
		get
		{
			return new List<List<KeyCode>>
			{
				new List<KeyCode> { KeyCode.A, KeyCode.LeftArrow },
				new List<KeyCode> { KeyCode.D, KeyCode.RightArrow },
				new List<KeyCode> { KeyCode.W, KeyCode.UpArrow },
				new List<KeyCode> { KeyCode.S, KeyCode.DownArrow },
				new List<KeyCode> { KeyCode.Space },
				new List<KeyCode> { KeyCode.F, KeyCode.J },
				new List<KeyCode> { KeyCode.E, KeyCode.Tab },
				new List<KeyCode> { KeyCode.Escape },
			};
		}
	}

	/// <summary>
	/// The main player (ie: controlled player).
	/// </summary>
	public static PlayerController MainPlayer
	{
		get { return mainPlayer; }
		// Set mainplayer in front
		set
		{
			if (mainPlayer != null)
			{
				foreach (SpriteMeshInstance s in mainPlayer.Sprites)
				{
					s.sortingOrder -= 1000;
				}
			}
			mainPlayer = value;
			foreach (SpriteMeshInstance s in mainPlayer.Sprites)
			{
				s.sortingOrder += 1000;
			}
			CameraController.movingToNewPosition = true;
		}
	}
	/// <summary>
	/// The main player (ie: controlled player).
	/// </summary>
	protected static PlayerController mainPlayer;
	/// <summary>
	/// All of the available players.
	/// </summary>
	public static List<PlayerController> players = new List<PlayerController>();

	#endregion

	#region Functions

	/// <summary>
	/// Adds a character to the list of playable characters.
	/// Optionally sets the character as the controlled one.
	/// Should be called in awake
	/// </summary>
	public static void Register(PlayerController pc, bool main)
	{
		players.Add(pc);
		if (main)
		{
			MainPlayer = pc;
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
	/// <param name="held">Whether or not the input was held or just pressed.</param>
	/// <param name="moveInput">Whether or not the player must be accepting move input.</param>
	public static bool GetInput(PlayerController pc, PlayerInput input, InputType type, bool moveInput = true)
	{
		if (MainPlayer == pc && (!moveInput || pc.AcceptingMoveInput))
		{
			foreach (KeyCode k in KeyBindings[(int)input])
			{
				if ((type == InputType.Held && Input.GetKey(k)) ||
					(type == InputType.Pressed && Input.GetKeyDown(k)) ||
					(type == InputType.Released && Input.GetKeyUp(k)))
				{
					return true;
				}
			}
		}
		return false;
	}

	#endregion

	#endregion

	#region Functions

	void Start()
	{
		players.Sort(delegate (PlayerController p1, PlayerController p2)
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
		int t = ToolTips.instance.SetTooltipActive("Press " +  KeyBindings[(int)PlayerInput.Left][0] + " and " +
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


	void Update()
	{
		foreach (KeyCode k in KeyBindings[(int)PlayerInput.Swap])
		{
			// Go through each player with e;
			if (Input.GetKeyDown(k))
			{
				int curr = players.IndexOf(MainPlayer);
				curr += 1;
				if (curr >= players.Count)
				{
					curr = 0;
				}
				MainPlayer = players[curr];
			}
		}
		// Set individual player based on hitting their sort value number key. 
		for (int i = 0; i < Utils.keyCodes.Length; i++)
		{
			if (Input.GetKeyDown(Utils.keyCodes[i]))
			{
				foreach (PlayerController p in players)
				{
					if (p.SORT_VALUE == i && p != MainPlayer)
					{
						MainPlayer = p;
						break;
					}
				}
			}
		}

		CheckPause();
	}

	void CheckPause() {
		if (GetInput(MainPlayer, PlayerInput.Pause, InputType.Pressed, false)) {
			PauseMenu.instance.Pause();
		}
	}

	#endregion
}
