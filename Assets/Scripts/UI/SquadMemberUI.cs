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

    // Start is called before the first frame update
    void Start()
    {
        dialogueBox.SetText("");
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePortrait();
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

    public IEnumerator Speak(string quote)
    {
        string output = "";
        foreach (char letter in quote)
        {
            output += letter;
            dialogueBox.SetText(output);
            if (!letter.Equals(" "))
                yield return new WaitForSeconds(0.05f);
        }
    }

}
