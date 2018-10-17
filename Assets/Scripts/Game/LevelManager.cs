using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// Event that restarts the current level.
    /// </summary>
    public static Action RestartLevel; 


    // Start is called before the first frame update
    void Start()
    {
        RestartLevel += Restart;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Restart()
    {
        StartCoroutine(LevelRestartCoroutine());
    } 

    IEnumerator LevelRestartCoroutine()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.33f); //scaled by above
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  
    }

}
