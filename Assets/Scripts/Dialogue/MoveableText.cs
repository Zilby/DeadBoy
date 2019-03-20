using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

/// <summary>
/// Moves text across screen. 
/// </summary>
public class MoveableText : MonoBehaviour
{
	private TextMeshProUGUI text;

	/// <summary>
	/// Whether or not to skip the delay for text being written.
	/// </summary>
	private bool skip;

	/// <summary>
	/// The name of the sound played when playing the text. 
	/// </summary>
	private string sound;

	/// <summary>
	/// The amount of delay between letters being typed. 
	/// </summary>
	private float letterDelay = 0.005f;

	/// <summary>
	/// The name of the sound played when playing the text. 
	/// </summary>
	public string Sound
	{
		set { sound = value; }
	}

	/// <summary>
	/// Gets or sets the letter delay.
	/// </summary>
	/// <value>The letter delay.</value>
	public float LetterDelay
	{
		get { return letterDelay; }
		set { letterDelay = value; }
	}

	/// <summary>
	/// Gets the tmpro text. 
	/// </summary>
	public TextMeshProUGUI Text
	{
		get { return text; }
	}

	// Use this for initialization
	void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
	}


	/// <summary>
	/// Clears the text. 
	/// </summary>
	public void ClearText()
	{
		text.text = "";
	}


	/// <summary>
	/// Skips text on spacebar.
	/// </summary>
	private void Update()
	{
		if (DBInputManager.GetInput(DBInputManager.controllers[0], PlayerInput.Submit, InputType.Pressed))
		{
			skip = true;
		}
	}


	/// <summary>
	/// Types text across the screen. 
	/// </summary>
	/// <param name="message">The message to be displayed</param>
	public IEnumerator TypeText(string message)
	{
		skip = false;
		ClearText();
		string current = "";
		char[] m = message.ToCharArray();
		//SoundManager.TextEvent();
		for (int i = 0; i < message.Length; i++)
		{
			if (skip)
			{
				text.text = message;
				skip = false;
				break;
			}
			current += m[i];
			if (Regex.IsMatch("" + m[i], "[A-Z]|[a-z]") && sound != "")
			{
				SFXManager.instance.PlayClip(sound);
			}
			text.text = current;
			text.text += "<color=#00000000>";
			for (int j = i + 1; j < message.Length; j++)
			{
				text.text += m[j];
			}
			text.text += "</color>";
			yield return new WaitForSecondsRealtime(letterDelay);
		}
		//SoundManager.StopTextEvent();
	}
}
