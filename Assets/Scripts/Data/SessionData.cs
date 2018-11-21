﻿using System;

/// <summary>
/// All file specific data. 
/// </summary>
[Serializable]
public class SessionData
{
	/// <summary>
	/// The player's current level. 
	/// </summary>
	public LevelManager.Levels level;

	/// <summary>
	/// The player's current checkpoint within a level. 
	/// </summary>
	public int checkpoint;
}