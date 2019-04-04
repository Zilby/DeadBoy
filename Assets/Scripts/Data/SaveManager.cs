using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine;

/// <summary>
/// Keeps track of and loads all permanent data.
/// </summary>
public class SaveManager : MonoBehaviour
{
	public static SaveManager instance;

	private static string FILE_NAME = Path.DirectorySeparatorChar + "dbFileData";
	private const string EDITOR_NAME = "Editor";

	private static string SaveFile
	{
		get
		{
			string fileName = FILE_NAME;
#if UNITY_EDITOR
			fileName += EDITOR_NAME;
#endif
			return Application.persistentDataPath + fileName + ".zlb";
		}
	}

	/// <summary>
	/// For saving/loading data. 
	/// </summary>
	[Serializable]
	public class SaveData
	{
		public FileData file;
		public SessionData session;
		public OptionsData options;
		public InputData input;

		public SaveData(FileData f = null, SessionData s = null, OptionsData o = null, InputData i = null)
		{
			file = f == null ? new FileData() : f;
			session = s == null ? new SessionData() : s;
			options = o == null ? new OptionsData() : o;
			input = i == null ? new InputData() : i;
		}
	}

	public static SaveData saveData;

	/// <summary>
	/// Whether or not data has been loaded.
	/// </summary>
	public static bool loaded;

	/// <summary>
	/// Unlocks all levels (for testing purposes).
	/// </summary>
	public static void LevelHax()
	{
		saveData.file.highestLevel = 9999;
	}

	/// <summary>
	/// Resets all game data.
	/// </summary>
	public static void Reset()
	{
		saveData = new SaveData();
	}

	/// <summary>
	/// Saves the game.
	/// </summary>
	public static void Save()
	{
		BinaryFormatter bf = new BinaryFormatter();

		FileStream file = File.Create(SaveFile);
		bf.Serialize(file, saveData);
		file.Close();
	}


	/// <summary>
	/// Loads the game.
	/// </summary>
	public static void Load()
	{
		saveData = new SaveData();
		bool fileFound = false;
		if (File.Exists(SaveFile))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(SaveFile, FileMode.Open);
			try
			{
				saveData = (SaveData)bf.Deserialize(file);
				file.Close();
				fileFound = true;
				Debug.Log("Loaded save file from "+SaveFile);
			}
			catch (Exception e)
			{
				Debug.LogWarning(e.Message + " Failed to deserialize save file.");
			}
		}
		if (!fileFound)
		{
			Debug.LogWarning("No file found at: " + SaveFile);
		}
		saveData.options.SetOptions();
		loaded = true;
	}

	protected virtual void Awake()
	{
		if (instance == null)
		{
			Load();
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}