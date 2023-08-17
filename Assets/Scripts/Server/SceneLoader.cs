/*
 * Designed to transition between scenes such as main menu and simulation area through buttons and game events
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;
    public TextMeshProUGUI simulationButton; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonPressed()
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
