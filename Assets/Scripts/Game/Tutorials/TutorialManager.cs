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
		return new string[]{"BasicMovement", "DGSwimming", "DGClimb", "Squish"};
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
			case "DGClimb":
				StartCoroutine(ClimbTutorial());
				return;
			case "Squish":
				StartCoroutine(SquishTutorial());
				return;
			default:
				return;
		}
	}

	///<summary>
	/// Should evaluate to true while the tip should still be shown
	///</summary>
	public delegate bool PendingFunc();

	public IEnumerator ShowTip(PendingFunc pending, PlayerController pc, string str, float delay = 0.3f, Vector3? offset = null) 
	{
		Vector3 tipOffset = offset ?? DEFAULT_OFFSET;

		yield return new WaitForSeconds(delay);

		int t = ToolTips.instance.SetTooltipActive(str, pc.transform.position + tipOffset);
		while (pending())
		{
			ToolTips.instance.SetTooltipPosition(t, pc.transform.position + tipOffset);
			yield return null;
		}

		ToolTips.instance.SetTooltipInactive(t);
	}
	
	/// <summary>
	/// Show a prompt to press one of a list of buttons. Succeeds when any of the given keys is pressed. 
	/// </summary>
	public IEnumerator ShowInputTip(PlayerController pc, List<PlayerInput> keys, string str, float delay = 0.3f, Vector3? offset = null)
	{
		yield return ShowTip(() => keys.TrueForAll(k => !DBInputManager.GetInput(pc, k, InputType.Held)), pc, str, delay, offset);
	}
	// one button version
	public IEnumerator ShowInputTip(PlayerController pc, PlayerInput key, string str, float delay = 0.3f, Vector3? offset = null) 
	{
		yield return ShowInputTip(pc, new List<PlayerInput>{key}, str, delay, offset);
	}


	/// <summary>
	/// Show a prompt to swap to a character. Succeeds when swapped to. 
	/// </summary>
	public IEnumerator ShowSwapTip(PlayerController pc, string str, float delay = 0.3f, Vector3? offset = null)
	{	
		yield return ShowTip(() => DBInputManager.MainPlayer != pc, pc, str, delay, offset);
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

		yield return StartCoroutine(ShowSwapTip(DG, "Press " + DBInputManager.GetInputName(DB, PlayerInput.Swap) + " to cycle characters"));

		yield return StartCoroutine(ShowInputTip(DG, new  List<PlayerInput>{PlayerInput.Up, PlayerInput.Down, PlayerInput.Right, PlayerInput.Left}, 
			"Use " + DBInputManager.GetInputName(DG, PlayerInput.Up) + ", " + DBInputManager.GetInputName(DG, PlayerInput.Down) + ", " + 
				DBInputManager.GetInputName(DG, PlayerInput.Left) + ", and " + DBInputManager.GetInputName(DG, PlayerInput.Right) + " to swim"));
	}

	public IEnumerator ClimbTutorial() 
	{
		// Hack to get actually the button prompt, This absolutely doesn't work for coop
		PlayerController DB = DBInputManager.instance.GetPlayerController(Character.Deadboy);
		PlayerController DG = DBInputManager.instance.GetPlayerController(Character.DrownedGirl);
		
		yield return StartCoroutine(ShowTip(() => !DG.climbing, DG, 
			"Hold " + DBInputManager.GetInputName(DB, PlayerInput.Jump) + " near a ledge to climb up"));

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
