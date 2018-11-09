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
				new List<KeyCode> { KeyCode.F, KeyCode.J }
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
	public static bool GetInput(PlayerController pc, PlayerInput input, bool held, bool moveInput = true)
	{
		if (MainPlayer == pc && (!moveInput || pc.AcceptingMoveInput))
		{
			foreach (KeyCode k in KeyBindings[(int)input])
			{
				if (held ? Input.GetKey(k) : Input.GetKeyDown(k))
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
		Vector3 tipOffset = new Vector3(0.0f, 2.43f, 0.0f);

		int tip = ToolTips.instance.SetTooltipActive("WASD or arrow keys to move", MainPlayer.transform.position + tipOffset);
		float tipTime = 0.0f;
		while (tipTime < 3.0f) {
			yield return null;
			tipTime += Time.deltaTime;
			ToolTips.instance.SetTooltipPosition(tip, MainPlayer.transform.position + tipOffset);
		}
		ToolTips.instance.SetTooltipInactive(tip);
		yield return new WaitForSeconds(0.5f);

		tip = ToolTips.instance.SetTooltipActive("Press Space to jump", MainPlayer.transform.position + tipOffset);
		tipTime = 0.0f;
		while (tipTime < 2.0f) {
			yield return null;
			tipTime += Time.deltaTime;
			ToolTips.instance.SetTooltipPosition(tip, MainPlayer.transform.position + tipOffset);
		}
		ToolTips.instance.SetTooltipInactive(tip);

		StartCoroutine(SwapTutorial());
	}

	protected IEnumerator SwapTutorial()
	{
		yield return new WaitForSeconds(1.0f);

		List<PlayerController> swappedTo = new List<PlayerController>();
		Dictionary<PlayerController, int> tips = new Dictionary<PlayerController, int>();
		PlayerController lastControlled = MainPlayer;
		Vector3 tipOffset = new Vector3(0.0f, 2.43f, 0.0f);

		foreach (PlayerController p in players)
		{
			if (p != MainPlayer)
			{
				tips[p] = ToolTips.instance.SetTooltipActive("Press " + p.SORT_VALUE + " to swap characters", p.transform.position + tipOffset);
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
					tips[lastControlled] = ToolTips.instance.SetTooltipActive("Press " + lastControlled.SORT_VALUE + " to swap characters", lastControlled.transform.position + tipOffset);
				}

				if (swappedTo.IndexOf(MainPlayer) < 0)
				{
					swappedTo.Add(MainPlayer);
				}
				lastControlled = MainPlayer;
			}
		}

		int tabTip = ToolTips.instance.SetTooltipActive("Press E to cycle characters", MainPlayer.transform.position + tipOffset);
		while (MainPlayer == lastControlled)
		{
			ToolTips.instance.SetTooltipPosition(tabTip, MainPlayer.transform.position + tipOffset);
			yield return null;
		}
		ToolTips.instance.SetTooltipInactive(tabTip);
	}

	void Update()
	{
		// Go through each player with e;
		if (Input.GetKeyDown(KeyCode.E))
		{
			int curr = players.IndexOf(MainPlayer);
			curr += 1;
			if (curr >= players.Count)
			{
				curr = 0;
			}
			MainPlayer = players[curr];
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
	}

	#endregion
}
