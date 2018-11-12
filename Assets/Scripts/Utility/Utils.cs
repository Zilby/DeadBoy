using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General utility class
/// </summary>
public static class Utils
{
	public static KeyCode[] keyCodes = {
		 KeyCode.Alpha0,
		 KeyCode.Alpha1,
		 KeyCode.Alpha2,
		 KeyCode.Alpha3,
		 KeyCode.Alpha4,
		 KeyCode.Alpha5,
		 KeyCode.Alpha6,
		 KeyCode.Alpha7,
		 KeyCode.Alpha8,
		 KeyCode.Alpha9,
	};

	public static T FindDeepChild<T>(Transform t, string name)
	{
		Transform tr = t.Find(name);
		T result = default(T);
		if (tr != null) {
			result = tr.GetComponent<T>();
		}
		if (result == null)
		{
			foreach (Transform child in t)
			{
				result = FindDeepChild<T>(child, name);
				if (result != null)
					break;
			}
		}
		return result;
	}

	/// <summary>
	/// Quits the game. 
	/// </summary>
	public static void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}
}
