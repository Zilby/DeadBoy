using UnityEditor;
using UnityEngine;

/// <summary>
/// Helps with the usage of characters in our scenes. 
/// </summary>
internal class CharacterCreator
{

	[MenuItem("GameObject/Character/All", false, 1)]
	static void CreateAllPrefabs()
	{
		PrefabCreator.CreatePrefabs("Characters");
	}

	[MenuItem("GameObject/Character/Deadboy", false, 1)]
	static void CreateDeadboy()
	{
		PrefabCreator.CreatePrefabs("Characters/Deadboy");
	}

	[MenuItem("GameObject/Character/DrownedGirl", false, 1)]
	static void CreateDrownedGirl()
	{
		PrefabCreator.CreatePrefabs("Characters/DrownedGirl");
	}

	[MenuItem("GameObject/Character/Squish", false, 1)]
	static void CreateSquish()
	{
		PrefabCreator.CreatePrefabs("Characters/Squish");
	}

	[MenuItem("GameObject/Character/ElectricBaby", false, 1)]
	static void CreateElectricBaby()
	{
		PrefabCreator.CreatePrefabs("Characters/ElectricBaby");
	}

	[MenuItem("GameObject/Character/FireKid", false, 1)]
	static void CreateFireKid()
	{
		PrefabCreator.CreatePrefabs("Characters/FireKid");
	}

}
