﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

		public bool IsImage
		{
			get { return sceneType == SceneType.Image || sceneType == SceneType.Animation; }
		}

		public bool IsAnimation
		{
			get { return sceneType == SceneType.Animation; }
		}
	}

	/// <summary>
	/// All of the individual scenes in this cutscene. 
	/// </summary>
	public List<Scene> scenes;

	private FadeableUI fadeable;
	private Image image;

	private void Awake()
	{
		fadeable = GetComponentInChildren<FadeableUI>();
		image = GetComponentInChildren<Image>();
	}

	public void Start()
	{
		StartCoroutine(ExecuteCutscene());
	}

	public IEnumerator ExecuteCutscene()
	{
		foreach (Scene s in scenes)
		{
			switch (s.sceneType)
			{
				case SceneType.Dialogue:
					yield return DialogueManager.instance.BeginDialogue(s.name);
					break;
				case SceneType.Image:
					yield return DisplayImage(s);
					break;
				case SceneType.Animation:
					yield return DisplayAnimation(s);
					break;
			}
		}
		yield return fadeable.FadeOut();
	}

	public IEnumerator DisplayImage(Scene s)
	{
		image.sprite = s.sprites[0];
		if (s.fadeIn)
		{
			yield return fadeable.FadeIn();
		}
		yield return new WaitForSeconds(0.5f);
		yield return DBInputManager.WaitForKeypress(PlayerInput.Submit);
		if (s.fadeOut)
		{
			yield return fadeable.FadeOut();
		}
	}

	public IEnumerator DisplayAnimation(Scene s)
	{
		Coroutine swapping = StartCoroutine(SwapImages(s));
		if (s.fadeIn)
		{
			yield return fadeable.FadeIn();
		}
		yield return new WaitForSeconds(0.5f);
		yield return DBInputManager.WaitForKeypress(PlayerInput.Submit);
		StopCoroutine(swapping);
		if (s.fadeOut)
		{
			yield return fadeable.FadeOut();
		}
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
