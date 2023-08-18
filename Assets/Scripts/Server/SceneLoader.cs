/*
 * Designed to transition between scenes such as main menu and simulation area through buttons and game events
 *  - is usually attached to buttons
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonPressed(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(sceneName);
    }
}
