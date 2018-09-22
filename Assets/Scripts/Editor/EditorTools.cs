using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor tools class
/// </summary>
public class EditorTools : EditorWindow
{
    
	public const string DOCS_LINK = "https://drive.google.com/drive/folders/1VqfBMsc2E3Q5fMgyBK8CIJ2HkOHcTpgA";
    public const string GITHUB_LINK = "https://github.com/Zilby/DeadBoy";

    public const string HAX = "allUnlocks";
    public const string SAVE_PREFERENCES = "saveChoices";

    private bool hax = false;
    /// <summary>
    /// Should these preferences hold when the tools window is opened again?
    /// </summary>
    private bool savePreferences = false;


    [MenuItem("DeadBoy/GoogleDocs")]
    static void DocLink()
    {
        Application.OpenURL(DOCS_LINK);
    }

    [MenuItem("DeadBoy/GitHub")]
    static void GitLink()
    {
        Application.OpenURL(GITHUB_LINK);
    }

    [MenuItem("DeadBoy/Tools")]
    static void ToolsInit()
    {
        EditorWindow window = GetWindow(typeof(EditorTools));
        window.Show();
    }

    void OnGUI()
    {
        savePreferences = EditorGUILayout.Toggle("Save Preferences", EditorPrefs.GetBool(SAVE_PREFERENCES));
        EditorPrefs.SetBool(SAVE_PREFERENCES, savePreferences);

        bool prefHax = savePreferences ? EditorPrefs.GetBool(HAX) : hax;

        hax = EditorGUILayout.Toggle("All Unlocks", prefHax);
        EditorPrefs.SetBool(HAX, hax);
    }
}