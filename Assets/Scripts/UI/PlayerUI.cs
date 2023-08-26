using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    // Needed for TextMeshPro UI

/* 
 * Script in charge of managing player's UI such as health, temperature, energy, textboxes, and their target lock
 *  - Is child of SquadMemberUI- allows PlayerUI to update character portrait and have dialogue
 *  - Later make the text boxes resizable
 */

public class PlayerUI: SquadMemberUI
{

    public TextMeshProUGUI integrityText;
    private Vector3 integrityOriginalPos;
    private Vector3 integrityOriginalScale;

    public TextMeshProUGUI tempText;
    private Vector3 tempOriginalPos;
    private Vector3 tempOriginalScale;

    public TextMeshProUGUI energyText;
    private Vector3 energyOriginalPos;
    private Vector3 energyOriginalScale;

    // For managing current target's display 
    public GameObject targetLockImage;
    public TextMeshProUGUI targetLockText;
    public Camera targetLockCamera;
    private Entity enemy;

    // Start is called before the first frame update
    void Start()
    {
        // Storing original format of textboxes (need to create new Vector3 so original parameters can't change bc they are instances rather than references)
        var integrity = integrityText.GetComponent<RectTransform>();    
        integrityOriginalPos = new Vector3(integrity.localPosition.x, integrity.localPosition.y, integrity.localPosition.z);
        integrityOriginalScale = new Vector3(integrityText.transform.localScale.x, integrityText.transform.localScale.y, integrityText.transform.localScale.z);

        var temp = tempText.GetComponent<RectTransform>();
        tempOriginalPos = new Vector3(temp.localPosition.x, temp.localPosition.y, temp.localPosition.z);
        tempOriginalScale = new Vector3(temp.localScale.x, temp.localScale.y, temp.localScale.z);

        var energy = energyText.GetComponent<RectTransform>();
        energyOriginalPos = new Vector3(energy.localPosition.x, energy.localPosition.y, energy.localPosition.z);
        energyOriginalScale = new Vector3(energy.localScale.x, energy.localScale.y, energy.localScale.z);
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateColors();
        UpdateText();
        UpdatePortrait();
        if (snowman.isLockedOn && snowman.target != null)
        {
            enemy = snowman.target.gameObject.GetComponent<Entity>();
            targetLockImage.SetActive(true);
            UpdateTarget();
        }
        else
            targetLockImage.SetActive(false);
        if (snowman.animator.GetBool("isMelting") & !hasSpokenDeathVoiceLine)
        {
            hasSpokenDeathVoiceLine = true;
            OverrideSpeak(deathVoiceLines[(int)UnityEngine.Random.Range(0, deathVoiceLines.Count)]);
        }
    }

    private void UpdateText()
    {   
        TextShake();

        integrityText.SetText(Mathf.Round(snowman.systemIntegrity).ToString() + " / " + snowman.maxIntegrity);
        tempText.SetText(Mathf.Round(snowman.temperature).ToString());
        energyText.SetText(Mathf.Round(snowman.energy).ToString() + " / " + snowman.maxEnergy);
    }

    private void TextShake()
    {
        // Making integrity text shake if low
        if (snowman.systemIntegrity <= snowman.maxIntegrity / 3)
        {
            integrityText.rectTransform.localScale = integrityOriginalScale * 1.5f;
            integrityText.rectTransform.localPosition = integrityOriginalPos + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);
        }
        else
        {
            // Resetting text if integrity no longer low
            integrityText.rectTransform.localScale = integrityOriginalScale;
            integrityText.rectTransform.localPosition = integrityOriginalPos;
        }

        // Making temperature text shake if high
        if (snowman.temperature >= 40)
        {
            tempText.rectTransform.localScale = tempOriginalScale * 1.5f;
            tempText.rectTransform.localPosition = tempOriginalPos + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);
        }
        else
        {
            // Resetting text if temperature no longer high
            tempText.rectTransform.localScale = tempOriginalScale;
            tempText.rectTransform.localPosition = tempOriginalPos;
        }

        // Making energy text shake if low
        if (snowman.energy <= snowman.maxEnergy / 3)
        {
            energyText.rectTransform.localScale = energyOriginalScale * 1.5f;
            energyText.rectTransform.localPosition = energyOriginalPos + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);
        }
        else
        {
            energyText.rectTransform.localScale = energyOriginalScale;
            energyText.rectTransform.localPosition = energyOriginalPos;
        }
    }

    private void UpdateColors()
    {
        UpdateIntegrityColor();
        UpdateEnergyColor();
        UpdateTemperatureColor();
    }

    private void UpdateIntegrityColor()
    {
        // Setting text color based on how low health is
        if (snowman.systemIntegrity <= snowman.maxIntegrity / 3)
            integrityText.color = new Color32(236, 30, 17, 255); // Red; use Color32 instead of Color because Color32 has bytes as units while Color has floats
        else if (snowman.systemIntegrity <= snowman.maxIntegrity * 2 / 3)
            integrityText.color = new Color32(218, 224, 26, 255); // Yellow
        else
            integrityText.color = new Color32(32, 205, 44, 255); // Green
    }

    private void UpdateEnergyColor()
    {
        // Setting text color based on how low energy is
        if (snowman.energy <= snowman.maxEnergy / 3)
            energyText.color = new Color32(236, 30, 17, 255); // Red; use Color32 instead of Color because Color32 has bytes as units while Color has floats
        else if (snowman.energy <= snowman.maxEnergy * 2 / 3)
            energyText.color = new Color32(218, 224, 26, 255); // Yellow
        else
            energyText.color = new Color32(32, 205, 44, 255); // Green
    }

    private void UpdateTemperatureColor()
    {
        // Setting text color based on how high snowman.temperature is
        if (snowman.temperature >= 40)
            tempText.color = new Color32(236, 30, 17, 255); // Red
        else if (snowman.temperature >= 10)
            tempText.color = new Color32(218, 224, 26, 255); // Yellow
        else
            tempText.color = new Color32(32, 205, 44, 255); // Green
    }

    // Target Lock UI
    private void UpdateTarget()
    {
        // Displaying target's stats
        targetLockText.text = "HP: " + Mathf.Round(enemy.systemIntegrity) + " / " + enemy.maxIntegrity + "\nTemp: " + Mathf.Round(enemy.temperature)
                               + "\nNRG: " + Mathf.Round(enemy.energy) + " / " + enemy.maxEnergy;

        // Making secondary camera look at target
        targetLockCamera.transform.LookAt(enemy.transform);
        targetLockCamera.transform.position = enemy.transform.position + new Vector3(snowman.transform.position.x - enemy.transform.position.x, 0f, // Camera is positioned between enemy and player
                                              snowman.transform.position.z - enemy.transform.position.z).normalized * 4 * enemy.agent.radius;   // By adding vector to player to enemy's position (3 * radius so that camera is not inside target)
    }
}
