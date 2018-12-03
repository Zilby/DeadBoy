using UnityEditor;
using UnityEngine;

/// <summary>
/// Helps with the creation of prefabs in our scenes. 
/// </summary>
internal class PrefabCreator
{
	public static void CreatePrefabs(string path) {
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
