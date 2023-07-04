using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatus: Player
{
    public TextMeshProUGUI integrityText;
    public TextMeshProUGUI tempText;

    // For manipulating text
    public Transform integritySizing;
    public Transform tempSizing;

    public ThirdPersonMovement movementScript;

    // Start is called before the first frame update
    void Start()
    {
       // integrityText.SetText(systemIntegrity.ToString());
       // tempText.SetText(temperature.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStats();
        UpdateColors();
        UpdateText();
        //CheckMelt();
    }

    void UpdateStats()
    {
        float tempChange = tempGain - tempLoss;
        temperature += tempChange * Time.deltaTime;

        float integrityChange = integrityRegen - integrityLoss - temperature/10f;
        systemIntegrity += integrityChange * Time.deltaTime;

        // To keep player health within zero and max health and temperature above minimum
        systemIntegrity = Mathf.Clamp(systemIntegrity, 0, maxIntegrity);
        temperature = Mathf.Clamp(temperature, minTemperature, 1000000f);
    }

    void UpdateText()
    {
        TextShake();
        integrityText.SetText(Mathf.Round(systemIntegrity).ToString() + " / " + maxIntegrity);
        tempText.SetText(Mathf.Round(temperature).ToString());
    }

    void TextShake()
    {
        // Making integrity text shake if low
        if (systemIntegrity <= maxIntegrity / 3)
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

        // Making temperature text shake if high
        if (temperature >= 40)
        {
            tempSizing.localScale = new Vector3(1.5f, 1.5f, 1f);
            tempSizing.localPosition = new Vector3((Random.value * 10f + 185f), (Random.value * 10f - 40f), 0f);
        }
        else
        {
            // Resetting text if temperature no longer high
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
        if (systemIntegrity <= maxIntegrity / 3)
        {
            integrityText.color = new Color32(236, 30, 17, 255); // Red; use Color32 instead of Color because Color32 has bytes as units while Color has floats
        }
        else if (systemIntegrity <= maxIntegrity * 2 / 3)
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
        // Setting text color based on how high temperature is
        if (temperature >= 40)
        {
            tempText.color = new Color32(236, 30, 17, 255); // Red
        }
        else if (temperature >= 10)
        {
            tempText.color = new Color32(218, 224, 26, 255); // Yellow
        }
        else
        {
            tempText.color = new Color32(32, 205, 44, 255); // Green
        }
    }

    /*
    void CheckMelt() {
        // If player is going to melt
        if (systemIntegrity <= 0)
        {
            animator.SetBool("isMelting", true);
            speed = 1f;
        }
        else
        {
            animator.SetBool("isMelting", false);
            speed = 6f;
        }
    }
    */
    
}
