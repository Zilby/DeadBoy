using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	public static TutorialManager instance;

	public bool PlayOnStart = true;

	[StringInList(typeof(TutorialManager), "TutorialList")]
	[ConditionalHide("PlayOnStart", true)]
	public string tutorial;

	private Vector3 DEFAULT_OFFSET = new Vector3(0.0f, 2.7f, 0.0f);

	void Awake()
	{
		instance = this;
	}

	void Start()
	{	
		if (PlayOnStart) {
			RunTutorial();
		}
	}


	public static string[] TutorialList()
	{
		return new string[]{"BasicMovement", "DGSwimming", "Squish"};
	}

	public void RunTutorial()
	{
		RunTutorial(tutorial);
	}

	public void RunTutorial(string tut)
	{
		switch (tut) {
			case "BasicMovement":
				StartCoroutine(MovementTutorial(Character.Deadboy));
				return;
			case "DGSwimming":
				StartCoroutine(DGTutorial());
				return;
			case "Squish":
				StartCoroutine(SquishTutorial());
				return;
			default:
				return;
		}
	}

  /*
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
		List<PlayerController> swappedTo = new List<PlayerController>();
		Dictionary<PlayerController, int> tips = new Dictionary<PlayerController, int>();
		PlayerController lastControlled = MainPlayer;

		if (MainIsKeyboard)
		{
			foreach (PlayerController p in players.Keys)
			{
				if (p != MainPlayer)
				{
					tips[p] = ToolTips.instance.SetTooltipActive("Press " + p.CharIDInt + " to control " + p.Name, p.transform.position + tipOffset);
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
						tips[lastControlled] = ToolTips.instance.SetTooltipActive("Press " + lastControlled.CharIDInt + " to control " + lastControlled.Name, lastControlled.transform.position + tipOffset);
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
  */


	
	
	/// <summary>
	/// Show a prompt to press one of a list of buttons. Succeeds when any of the given keys is pressed. 
	/// </summary>
	public IEnumerator ShowInputTip(PlayerController pc, List<PlayerInput> keys, string str, float delay = 0.3f, Vector3? offset = null)
	{
		Vector3 tipOffset = offset ?? DEFAULT_OFFSET;

		yield return new WaitForSeconds(delay);

		int t = ToolTips.instance.SetTooltipActive(str, pc.transform.position + tipOffset);
		while (keys.TrueForAll(k => !DBInputManager.GetInput(pc, k, InputType.Held)))
		{
			ToolTips.instance.SetTooltipPosition(t, pc.transform.position + tipOffset);
			yield return null;
		}

		ToolTips.instance.SetTooltipInactive(t);	
	}
	// one button version
	public IEnumerator ShowInputTip(PlayerController pc, PlayerInput key, string str, float delay = 0.3f, Vector3? offset = null) {
		yield return ShowInputTip(pc, new List<PlayerInput>{key}, str, delay, offset); // StartCoroutine?
	}


	/// <summary>
	/// Show a prompt to swap to a character. Succeeds when swapped to. 
	/// </summary>
	public IEnumerator ShowSwapTip(PlayerController pc, string str, float delay = 0.3f, Vector3? offset = null)
	{
		Vector3 tipOffset = offset ?? DEFAULT_OFFSET;

		yield return new WaitForSeconds(delay);

		int t = ToolTips.instance.SetTooltipActive(str, pc.transform.position + tipOffset);
		while (DBInputManager.MainPlayer != pc)
		{
			ToolTips.instance.SetTooltipPosition(t, pc.transform.position + tipOffset);
			yield return null;
		}

		ToolTips.instance.SetTooltipInactive(t);	
	}


	/// <summary>
	/// Tutorial for how to move. 
	/// </summary>
	public IEnumerator MovementTutorial(Character c)
	{
		PlayerController pc = DBInputManager.instance.GetPlayerController(c);

		yield return StartCoroutine(ShowInputTip(pc, new List<PlayerInput>{PlayerInput.Left, PlayerInput.Right},
			"Press " + DBInputManager.GetInputName(pc, PlayerInput.Left) + " and " + DBInputManager.GetInputName(pc, PlayerInput.Right) + " to move"));


		yield return StartCoroutine(ShowInputTip(pc, PlayerInput.Jump,
			"Press " + DBInputManager.GetInputName(pc, PlayerInput.Jump) + " to jump"));

	}

	/// <summary>
	/// Tutorial for swapping between DB and DG and swimming. 
	/// </summary>
	public IEnumerator DGTutorial()
	{
		PlayerController DB = DBInputManager.instance.GetPlayerController(Character.Deadboy);
		PlayerController DG = DBInputManager.instance.GetPlayerController(Character.DrownedGirl);

		if (DBInputManager.MainIsKeyboard) {
			yield return StartCoroutine(ShowSwapTip(DG, "Press " + DG.CharIDInt + " to control " + DG.Name, delay: 0));
			yield return StartCoroutine(ShowSwapTip(DB,	"Press " + DB.CharIDInt + " to control " + DB.Name, offset: new Vector3(3.0f, 0.0f, 0.0f)));
		}

//Swap tip
		yield return StartCoroutine(ShowSwapTip(DG, "Press " + DBInputManager.GetInputName(DB, PlayerInput.Swap) + " to cycle characters"));

		yield return StartCoroutine(ShowInputTip(DG, new  List<PlayerInput>{PlayerInput.Up, PlayerInput.Down, PlayerInput.Right, PlayerInput.Left}, 
			"Use " + DBInputManager.GetInputName(DG, PlayerInput.Up) + ", " + DBInputManager.GetInputName(DG, PlayerInput.Down) + ", " + 
				DBInputManager.GetInputName(DG, PlayerInput.Left) + ", and " + DBInputManager.GetInputName(DG, PlayerInput.Right) + " to swim"));

	}

	/// <summary>
	/// Tutorial for swapping to DG and swimming. 
	/// </summary>
	public IEnumerator SquishTutorial()
	{
		yield return new WaitForSeconds(0.5f);

		PlayerController Sq = DBInputManager.instance.GetPlayerController(Character.Squish);

		if (DBInputManager.MainIsKeyboard) {
			yield return StartCoroutine(ShowSwapTip(Sq, "Press " + Sq.CharIDInt + " to control " + Sq.Name));
		} else {
			yield return StartCoroutine(ShowSwapTip(Sq, "Press " + DBInputManager.GetInputName(DBInputManager.MainPlayer, PlayerInput.Swap) + " to swap to " + Sq.Name));
		}


		yield return StartCoroutine(ShowInputTip(Sq, PlayerInput.Power, 
			"Press " + DBInputManager.GetInputName(Sq, PlayerInput.Power) + " as Squish to transform"));
		yield return StartCoroutine(ShowInputTip(Sq, PlayerInput.Power, 
			"Press " + DBInputManager.GetInputName(Sq, PlayerInput.Power) + " again to transform back"));
	}
}
