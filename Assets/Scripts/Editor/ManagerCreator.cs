using UnityEditor;
using UnityEngine;

/// <summary>
/// Helps with the usage of singleton managers in our scenes. 
/// </summary>
internal class ManagerCreator
{
	[MenuItem("GameObject/Manager/Game", false, 1)]
	static void CreateGamePrefabs()
	{
		CreatePrefabs("Managers/Game");
	}

	[MenuItem("GameObject/Manager/General", false, 1)]
	static void CreateGeneralPrefabs()
	{
		CreatePrefabs("Managers/General");
	}

	static void CreatePrefabs(string path) {
		Object[] prefabs = Resources.LoadAll<Object>(path);
		foreach (Object o in prefabs)
		{
			if (GameObject.Find(o.name) == null)
			{
				Object created = PrefabUtility.InstantiatePrefab(o);
				Undo.RegisterCreatedObjectUndo(created, "Created " + created.name);
			}
		}
	}
}
