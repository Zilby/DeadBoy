using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Creates a cutscene using the given list of params. 
/// </summary>
public class Cutscene : MonoBehaviour
{
	public enum SceneType
	{
		Dialogue = 0,
		Image = 1,
		Animation = 2,
		Action = 3,
	}

	[System.Serializable]
	public struct Scene
	{
		public SceneType sceneType;

		[StringInList(typeof(DialogueManager), "GetDialogueList")]
		public string name;

		public List<Sprite> sprites;

		[ConditionalHide("IsImage", true)]
		public bool fadeIn;

		[ConditionalHide("IsImage", true)]
		public bool fadeOut;

		[ConditionalHide("IsAnimation", true)]
		public float delay;

		[ConditionalHide("IsAction", true)]
		public InteractAction action;

		public bool IsImage
		{
			get { return sceneType == SceneType.Image || sceneType == SceneType.Animation; }
		}

		public bool IsAnimation
		{
			get { return sceneType == SceneType.Animation; }
		}

		public bool IsAction
		{
			get { return sceneType == SceneType.Action; }
		}
	}

	/// <summary>
	/// All of the individual scenes in this cutscene. 
	/// </summary>
	public List<Scene> scenes;

	public bool playOnLoad = false;
	
	[ConditionalHide("runTutorialOnFinish", true, true)]
	public bool loadSceneOnFinish = false;
	[StringInList(typeof(LevelManager), "GetLoadedLevels")]
	public string loadedScene;

	[ConditionalHide("loadSceneOnFinish", true, true)]
	public bool runTutorialOnFinish = false;
	[StringInList(typeof(TutorialManager), "TutorialList")]
	public string tutorial;

	private FadeableUI fadeable;
	private Image image;

	private void Awake()
	{
		fadeable = GetComponentInChildren<FadeableUI>();
		image = GetComponentInChildren<Image>();
	}

	public void Start()
	{
		if (playOnLoad) {
			StartCoroutine(ExecuteCutscene());
		}
	}

	public IEnumerator ExecuteCutscene()
	{
		foreach (Scene s in scenes)
		{
			switch (s.sceneType)
			{
				case SceneType.Dialogue:
					yield return StartCoroutine(DialogueManager.instance.BeginDialogue(s.name));
					break;
				case SceneType.Image:
					yield return DisplayImage(s);
					break;
				case SceneType.Animation:
					yield return DisplayAnimation(s);
					break;
				case SceneType.Action:
					yield return StartCoroutine(s.action.Act(DBInputManager.MainPlayer));
					break;
			}
		}
		yield return fadeable.FadeOut();
		if (loadSceneOnFinish)
		{
			Fader.SceneEvent(loadedScene);
		}
		if (runTutorialOnFinish)
		{
			TutorialManager.instance.RunTutorial(tutorial);
		}
	}

	public IEnumerator DisplayImage(Scene s)
	{
		DBInputManager.instance.restrictInput = true;
		image.sprite = s.sprites[0];
		if (s.fadeIn)
		{
			yield return fadeable.FadeIn();
		}
		yield return new WaitForSeconds(0.5f);
		yield return DBInputManager.WaitForKeypress(PlayerInput.Submit, PlayerInput.Jump);
		if (s.fadeOut)
		{
			yield return fadeable.FadeOut();
		}
		DBInputManager.instance.restrictInput = false;
	}

	public IEnumerator DisplayAnimation(Scene s)
	{
		DBInputManager.instance.restrictInput = true;
		Coroutine swapping = StartCoroutine(SwapImages(s));
		if (s.fadeIn)
		{
			yield return fadeable.FadeIn();
		}
		yield return new WaitForSeconds(0.5f);
		yield return DBInputManager.WaitForKeypress(PlayerInput.Submit, PlayerInput.Jump);
		StopCoroutine(swapping);
		if (s.fadeOut)
		{
			yield return fadeable.FadeOut();
		}
		DBInputManager.instance.restrictInput = false;
	}

	public IEnumerator SwapImages(Scene s)
	{
		for (int i = 0; ; ++i)
		{
			if (i >= s.sprites.Count)
			{
				i = 0;
			}
			image.sprite = s.sprites[i];
			yield return new WaitForSeconds(s.delay);
		}
	}
}
