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
    Left, Right, Up, Down, Jump, Interact
}

public class PlayerControllerManager : MonoBehaviour
{

    #region Static

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
	public static void Unregister(PlayerController pc) {
        players.Remove(pc);
    }

    /// <summary>
    /// Returns whether an input for the given action was pressed this frame for a given character.
    /// </summary>
    public static bool GetInputStart(PlayerController pc, PlayerInput input) 
    {
        if (pc == MainPlayer)
        {
            switch(input)
            {
                case PlayerInput.Left:
                    return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
                case PlayerInput.Right:
                    return Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
                case PlayerInput.Up:
                    return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
                case PlayerInput.Down:
                    return Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
                case PlayerInput.Jump:
                    return Input.GetKeyDown(KeyCode.Space);
                case PlayerInput.Interact:
                    return Input.GetKeyDown(KeyCode.F);
                default:
                    return false;
            }
        }
        return false;
    }
    /// <summary>
    /// Returns whether an input for the given action was held this frame for a given character.
    /// </summary>
    public static bool GetInputHeld(PlayerController pc, PlayerInput input) 
    {
        if (pc == MainPlayer)
        {
            switch(input)
            {
                case PlayerInput.Left:
                    return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
                case PlayerInput.Right:
                    return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
                case PlayerInput.Up:
                    return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
                case PlayerInput.Down:
                    return Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
                case PlayerInput.Jump:
                    return Input.GetKey(KeyCode.Space);
                case PlayerInput.Interact:
                    return Input.GetKey(KeyCode.F);
                default:
                    return false;
            }
        }
        return false;
    }

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

        StartCoroutine(SwapTutorial());
    }

    protected IEnumerator SwapTutorial() 
    {
        yield return new WaitForSeconds(2.0f);

    	List<PlayerController> swappedTo = new List<PlayerController>();
        Dictionary<PlayerController, int> tips = new Dictionary<PlayerController, int>();
        PlayerController lastControlled = MainPlayer;
        Vector3 tipOffset = new Vector3(0.0f, 2.43f, 0.0f);

        foreach (PlayerController p in players)
        {
            if (p != MainPlayer) 
            {
                tips[p] = ToolTips.instance.SetTooltipActive("Press "+p.SORT_VALUE+ " to swap characters",p.transform.position + tipOffset);
            }
            else 
            {
                tips[p] = -1;
            }
        }


        while(swappedTo.Count < players.Count)
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
                    tips[lastControlled] = ToolTips.instance.SetTooltipActive("Press "+lastControlled.SORT_VALUE+ " to swap characters",lastControlled.transform.position + tipOffset);
                }
                
                if(swappedTo.IndexOf(MainPlayer) < 0) 
                {
                    swappedTo.Add(MainPlayer);
                }
                lastControlled = MainPlayer;
            }
        }

        int tabTip = ToolTips.instance.SetTooltipActive("Press E to cycle characters", MainPlayer.transform.position + tipOffset);
        while (MainPlayer == lastControlled) {
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
}
