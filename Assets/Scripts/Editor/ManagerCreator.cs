using UnityEditor;
using UnityEngine;

/// <summary>
/// Helps with the usage of singleton managers in our scenes. 
/// </summary>
internal class ManagerCreator
{

	[MenuItem("GameObject/Manager/All", false, 1)]
	static void CreateAllPrefabs()
	{
		PrefabCreator.CreatePrefabs("Managers");
	}

	[MenuItem("GameObject/Manager/Game", false, 1)]
	static void CreateGamePrefabs()
	{
		PrefabCreator.CreatePrefabs("Managers/Game");
	}

	[MenuItem("GameObject/Manager/General", false, 1)]
	static void CreateGeneralPrefabs()
	{
		PrefabCreator.CreatePrefabs("Managers/General");
	}
}
