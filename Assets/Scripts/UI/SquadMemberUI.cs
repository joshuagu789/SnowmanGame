/*
 * Script in charge of managing UI for player's team members (including the player itself which has a PlayerUI script inheriting from this)
 *  - Updates portrait of character based on their status
 *  - Allows for them to communicate in a dialogue box
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SquadMemberUI : MonoBehaviour
{
    public Entity snowman;     // Using Snowman (child of Entity) instead of Entity since only squad members player can have are snowmen

    // Managing character's portrait
    public Image imageField;
    public Sprite normal;
    public Sprite lockedOn;
    public Sprite death;
    public Sprite lockedOnDeath;

    public TextMeshProUGUI dialogueBox;

    // For dialogue that is often used repetitively
    public List<string> affirmativeVoiceLines = new List<string>();
    public List<string> readyVoiceLines = new List<string>();
    public List<string> deathVoiceLines = new List<string>();
    public List<string> alertVoiceLines = new List<string>();
    public List<string> alertResponseVoiceLines = new List<string>();

    [HideInInspector]
    public bool ableToSpeak = true;
    private bool showingStatus = false;
    private float displayTimer = 0;
    [HideInInspector]
    public bool hasSpokenDeathVoiceLine;

    // Start is called before the first frame update
    private void Start()
    {
        dialogueBox.SetText("");
    }

    // Update is called once per frame
    private void Update()
    {
        UpdatePortrait();
        if (showingStatus)
            DisplayStatus();
        else
            displayTimer = 0;
        if (snowman.animator.GetBool("isMelting") & !hasSpokenDeathVoiceLine)
        {
            hasSpokenDeathVoiceLine = true;
            OverrideSpeak(deathVoiceLines[(int)UnityEngine.Random.Range(0, deathVoiceLines.Count)]);
        }
    }

    public void UpdatePortrait()
    {
        if (snowman.animator.GetBool("isMelting") && snowman.isLockedOn && lockedOnDeath != null)
            imageField.sprite = lockedOnDeath;
        else if (snowman.animator.GetBool("isMelting") && death != null)
            imageField.sprite = death;
        else if (snowman.isLockedOn && lockedOn != null)
            imageField.sprite = lockedOn;
        else if(normal != null)
            imageField.sprite = normal;
    }

    // Outputs dialogue one character at a time for readability- can also be called on by outside scripts
    private IEnumerator Output(string quote)
    {
        if (ableToSpeak)
        {
            showingStatus = false;
            ableToSpeak = false;

            string output = "";
            foreach (char letter in quote)
            {
                output += letter;
                dialogueBox.SetText(output);
                if (letter.Equals(".") || letter.Equals(",") || letter.Equals("!") || letter.Equals("?") || letter.Equals(":"))
                    yield return new WaitForSeconds(1f);
                else if (!letter.Equals(" "))
                    yield return new WaitForSeconds(0.03f);
            }
            yield return new WaitForSeconds(6f);

            if (dialogueBox.text.Equals(output))    // If text after 6 seconds is still the same text
            {
                ableToSpeak = true;
                dialogueBox.SetText("");
            }
        }
    }

    // Makes entity output a random voice line from readyVoiceLines (also overrides)
    public void SpeakReady()
    {
        ableToSpeak = true;
        StartCoroutine(Output(readyVoiceLines[(int) UnityEngine.Random.Range(0, readyVoiceLines.Count)]));
    }

    // Makes entity output a random voice line from affirmativeVoiceLines (also overrides)
    public void SpeakAffirmative()
    {
        ableToSpeak = true;
        StartCoroutine(Output(affirmativeVoiceLines[(int) UnityEngine.Random.Range(0, affirmativeVoiceLines.Count)]));
    }

    // Displays health (systemIntegrity), temperature, energy, combat state
    public void DisplayStatus()
    {
        showingStatus = true;
        displayTimer += Time.deltaTime;
        dialogueBox.SetText("HP: " + Mathf.Round(snowman.systemIntegrity) + " / " + snowman.maxIntegrity + "\nTemp: " +
                                MathF.Round(snowman.temperature) + "\nNRG: " + MathF.Round(snowman.energy) + " / " + snowman.maxEnergy);
        if (displayTimer >= 6f) // Display lasts 6 seconds
        {
            showingStatus = false;
            dialogueBox.SetText("");
        }
    }

    // Forces character to speak even if it is not time yet
    public void OverrideSpeak(string quote)
    {
        ableToSpeak = true;
        StartCoroutine(Output(quote));
    }

    public void Speak(string quote)
    {
        StartCoroutine(Output(quote));
    }
}
