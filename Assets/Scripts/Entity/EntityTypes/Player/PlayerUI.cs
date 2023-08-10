using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    // Needed for TextMeshPro UI

/* 
 * Script in charge of managing player's UI such as health, temperature, energy, textboxes
 *  - Later make the text boxes resizable
 */

public class PlayerUI: MonoBehaviour
{

    public TextMeshProUGUI integrityText;
    public TextMeshProUGUI tempText;

    // For manipulating text
    public Transform integritySizing;
    public Transform tempSizing;

    public Player player;
    public PlayerMovement movementScript;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateColors();
        UpdateText();
    }

    void UpdateText()
    {
        Debug.Log("Width " + Screen.width + " Height " + Screen.height);
        TextShake();

        /*
        // To keep player health within zero and max health and player.temperature above minimum
        player.systemIntegrity = Mathf.Clamp(player.systemIntegrity, 0, player.maxIntegrity);
        player.temperature = Mathf.Clamp(player.temperature, player.minTemperature, 1000000f);
        */

        integrityText.SetText(Mathf.Round(player.systemIntegrity).ToString() + " / " + player.maxIntegrity);
        tempText.SetText(Mathf.Round(player.temperature).ToString());
    }

    void TextShake()
    {
        // Making integrity text shake if low
        if (player.systemIntegrity <= player.maxIntegrity / 3)
        {
            integritySizing.localScale = new Vector3(1.5f, 1.5f, 1f);
            integritySizing.localPosition = new Vector3((Random.value * 10f + 180f), (Random.value * 10f - 10f), 0f);
        }
        else
        {
            // Resetting text if integrity no longer low
            integritySizing.localScale = new Vector3(1f, 1f, 1f);
            integritySizing.localPosition = new Vector3(140f, 0f, 0f);
        }

        // Making player.temperature text shake if high
        if (player.temperature >= 40)
        {
            tempSizing.localScale = new Vector3(1.5f, 1.5f, 1f);
            tempSizing.localPosition = new Vector3((Random.value * 10f + 185f), (Random.value * 10f - 40f), 0f);
        }
        else
        {
            // Resetting text if player.temperature no longer high
            tempSizing.localScale = new Vector3(1f, 1f, 1f);
            tempSizing.localPosition = new Vector3(150f, -27f, 0f);
        }
    }

    void UpdateColors()
    {
        UpdateIntegrityColor();
        UpdateTemperatureColor();
    }

    void UpdateIntegrityColor()
    {
        // Setting text color based on how low health is
        if (player.systemIntegrity <= player.maxIntegrity / 3)
        {
            integrityText.color = new Color32(236, 30, 17, 255); // Red; use Color32 instead of Color because Color32 has bytes as units while Color has floats
        }
        else if (player.systemIntegrity <= player.maxIntegrity * 2 / 3)
        {
            integrityText.color = new Color32(218, 224, 26, 255); // Yellow
        }
        else
        {
            integrityText.color = new Color32(32, 205, 44, 255); // Green
        }
    }

    void UpdateTemperatureColor()
    {
        // Setting text color based on how high player.temperature is
        if (player.temperature >= 40)
        {
            tempText.color = new Color32(236, 30, 17, 255); // Red
        }
        else if (player.temperature >= 10)
        {
            tempText.color = new Color32(218, 224, 26, 255); // Yellow
        }
        else
        {
            tempText.color = new Color32(32, 205, 44, 255); // Green
        }
    }
}
