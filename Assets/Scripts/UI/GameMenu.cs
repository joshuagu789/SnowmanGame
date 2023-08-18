/*
 * Menu that pauses game when open and allows player to use options such as settings, help, and saving and returning to main menu
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    public GameObject menu;

    private bool isVisible = false;

    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(isVisible);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isVisible = !isVisible;
            menu.SetActive(isVisible);
            if (isVisible)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
        if (isVisible)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}
