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

	/// <summary>
	/// Finds a child by name recursively. 
	/// </summary>
	public static T FindDeepChild<T>(Transform t, string name)
	{
		Transform tr = t.Find(name);
		T result = default(T);
		if (tr != null)
		{
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
	/// Fades out an audio source. 
	/// </summary>
	public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
	{
		float startVolume = audioSource.volume;

		while (audioSource.volume > 0)
		{
			audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

			yield return null;
		}

		audioSource.Stop();
		audioSource.volume = startVolume;
	}

	/// <summary>
	/// Fades in an audio source. 
	/// </summary>
	public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
	{
		float startVolume = 0.2f;
		float originalVolume = audioSource.volume;
		audioSource.volume = 0;
		audioSource.Play();

		while (audioSource.volume < originalVolume)
		{
			audioSource.volume += startVolume * Time.deltaTime / FadeTime;

			yield return null;
		}

		audioSource.volume = originalVolume;
	}

	/// <summary>
	/// Moves an object to the given location. 
	/// </summary>
	public static IEnumerator MoveToLocation(Transform t, Vector2 location, float speed = 0.01f)
	{
		while (Mathf.Abs(location.x) > 0 || Mathf.Abs(location.y) > 0)
		{
			float xspeed = location.x * speed;
			float yspeed = location.y * speed;
			t.position = new Vector3(t.position.x + xspeed, t.position.y + yspeed, t.position.z);
			location.x -= xspeed;
			location.y -= yspeed;
			yield return new WaitForFixedUpdate();
		}
	}

	/// <summary>
	/// Quits the game. 
	/// </summary>
	public static void Quit()
	{
		SaveManager.Save();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}

	#region Accessor Functions
	// For changing vectors and colors

	public static Vector2 X(this Vector2 v, float x)
	{
		v.x = x;
		return v;
	}

	public static Vector2 Y(this Vector2 v, float y)
	{
		v.y = y;
		return v;
	}

	public static Vector3 X(this Vector3 v, float x)
	{
		v.x = x;
		return v;
	}

	public static Vector3 Y(this Vector3 v, float y)
	{
		v.y = y;
		return v;
	}

	public static Vector3 Z(this Vector3 v, float z)
	{
		v.z = z;
		return v;
	}

	public static Color R(this Color c, float r)
	{
		c.r = r;
		return c;
	}

	public static Color G(this Color c, float g)
	{
		c.g = g;
		return c;
	}

	public static Color B(this Color c, float b)
	{
		c.b = b;
		return c;
	}

	public static Color A(this Color c, float a)
	{
		c.a = a;
		return c;
	}

	public static Vector2 XAdd(this Vector2 v, float x)
	{
		v.x += x;
		return v;
	}

	public static Vector2 YAdd(this Vector2 v, float y)
	{
		v.y += y;
		return v;
	}

	public static Vector3 XAdd(this Vector3 v, float x)
	{
		v.x += x;
		return v;
	}

	public static Vector3 YAdd(this Vector3 v, float y)
	{
		v.y += y;
		return v;
	}

	public static Vector3 ZAdd(this Vector3 v, float z)
	{
		v.z += z;
		return v;
	}

	public static Color RAdd(this Color c, float r)
	{
		c.r += r;
		return c;
	}

	public static Color GAdd(this Color c, float g)
	{
		c.g += g;
		return c;
	}

	public static Color BAdd(this Color c, float b)
	{
		c.b += b;
		return c;
	}

	public static Color AAdd(this Color c, float a)
	{
		c.a += a;
		return c;
	}
	#endregion
}
