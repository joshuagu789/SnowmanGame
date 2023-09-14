/*
 * Script that uses UI to help Player communicate with allies through a list of commands presented on an Image
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Principal;
using System;

public class PlayerCommands : MonoBehaviour
{
    public Player player;
    [SerializeField]
    private new Transform camera;
    public GameObject commandBar;
    public TextMeshProUGUI outputTitle;
    public TextMeshProUGUI outputBox;

    private Dictionary<int, KeyCode> keycodes = new Dictionary<int, KeyCode> { {1, KeyCode.Alpha1 }, {2, KeyCode.Alpha2 }, {3, KeyCode.Alpha3 },
    {4, KeyCode.Alpha4 }, {5, KeyCode.Alpha5 }, {6, KeyCode.Alpha6 }, {7, KeyCode.Alpha7 }, {8, KeyCode.Alpha8 }, {9, KeyCode.Alpha9 }, {10, KeyCode.Alpha0 } };

    private List<Entity> targetAudience;    // Target to which the player commands
    private int counter;
    private bool toggleOn = false;
    private bool readyToSendOrder = false;

    private void Awake()
    {
        // Making command bar transparent
        commandBar.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.75f);
        commandBar.SetActive(false); 
        outputBox.color = new Color(1f, 1f, 1f, 0.85f);
    }

    // Update is called once per frame
    private void Update()
    {
        // Updating if the command bar should be displayed
        if (Input.GetKeyDown(KeyCode.Z) && !readyToSendOrder)
            toggleOn = !toggleOn;
        if (toggleOn)
            DisplayCommands();
        else
            commandBar.SetActive(false);
        CheckCommands();
    }

    // Making command bar visible
    private void DisplayCommands()
    {
        commandBar.SetActive(true);

        // Configuring outputBox to display options numbered 1 to x listing all of player's squad members
        if (!readyToSendOrder)
        {
            var output = "1. All \n";
            counter = 1;
            foreach (Entity ally in player.squadList)
            {
                if (ally.systemIntegrity > 0)
                {
                    counter++;
                    output += counter + ". " + ally.gameObject.name + "\n";
                }
            }
            outputTitle.text = "Select Target:";
            outputBox.text = output;
        }
        // Displaying all possible commands
        else if (readyToSendOrder)
        {
            outputTitle.text = "Give Command:";
            if (targetAudience.Count > 1)
                outputBox.text = "1. Focus fire \n2. Status Report \n3. Group up \n4. Spread out \n5. Tactical Ability \n6. Intel Ability \n7. Cancel";
            else
            {
                var tacticalAbility = targetAudience[0].GetComponent<SquadAbility>();
                var intelAbility = targetAudience[0].GetComponent<IntelAbility>();

                if(tacticalAbility != null && intelAbility != null)
                    outputBox.text = "1. Focus fire \n2. Status Report \n3. Group up \n4. Spread out \n5. " + tacticalAbility.GetAbilityType() + " \n6. " + intelAbility.GetAbilityType() + " \n7. Cancel";
                else
                    outputBox.text = "1. Focus fire \n2. Status Report \n3. Group up \n4. Spread out \n5. " + tacticalAbility.GetAbilityType() + " \n6. \" Intel Ability \n7. Cancel";
            }
            counter = 7;
        }
    }

    // Checking to see if an input corresponds with command option
    private void CheckCommands()
    {
        if (toggleOn && Input.anyKeyDown)
            foreach (int index in keycodes.Keys)
                if (index <= counter && Input.GetKeyDown(keycodes[index]))
                    StartCoroutine(ExecuteCommand(index));
    }

    // Executing command as indicated by what option corresponded with the number displayed in command bar
    private IEnumerator ExecuteCommand(int optionNumber)
    {
        // Alerting allies to await command
        if (toggleOn && !readyToSendOrder && optionNumber <= player.squadList.Capacity + 1) 
        {
            targetAudience = new List<Entity>();

            // Option 1 selects all members of squad
            if (optionNumber == 1)
            {
                foreach (Entity ally in player.squadList)
                    if (ally.systemIntegrity > 0)
                        targetAudience.Add(ally);
            }                  
            // Selecting only the Entity corresponding with option
            else if (optionNumber != 0 && player.squadList[optionNumber - 2].systemIntegrity > 0)
                targetAudience.Add(player.squadList[optionNumber - 2]);   

            foreach (Entity ally in targetAudience)
            {
                ally.gameObject.GetComponentInChildren<SquadMemberUI>().SpeakReady();    // Giving ally a random ready response
            }
            readyToSendOrder = true;
            toggleOn = false;
            yield return new WaitForSeconds(0.55f);
            toggleOn = true;
        }

        // Giving command to allies depending on option selected
        else if (toggleOn && readyToSendOrder)
        {
            if (optionNumber == 1)
            {
                foreach (Entity ally in targetAudience)
                {
                    ally.FocusFire(player.target);  // All allies get player's target
                    ally.gameObject.GetComponentInChildren<SquadMemberUI>().SpeakAffirmative();    // Giving ally a random ready response
                }
            }
            // Making ally report their systemIntegrity (health), temperature, energy
            else if (optionNumber == 2)
            {
                foreach (Entity ally in targetAudience)
                {
                    ally.gameObject.GetComponentInChildren<SquadMemberUI>().DisplayStatus();
                }
            }
            // Increasing/Decreasing ally Entity's leash range
            else if (optionNumber == 3 || optionNumber == 4)
            {
                int increment = 0;
                if (optionNumber == 3) { increment = -2; }
                else if (optionNumber == 4) { increment = 2; }

                foreach (Entity ally in targetAudience)
                {
                    ally.IncrementLeashRange(increment);  // All allies change leash range by increment * default leash range
                    ally.gameObject.GetComponentInChildren<SquadMemberUI>().SpeakAffirmative();
                }
            }
            // Making ally/allies use squad abilities if they are ready
            else if (optionNumber == 5)
            {
                foreach (Entity ally in targetAudience)
                {
                    var ability = ally.gameObject.GetComponent<SquadAbility>();
                    var allyUI = ally.gameObject.GetComponentInChildren<SquadMemberUI>();

                    if (ability.CanUseAbility() && ability != null)
                    {
                        ability.UseAbility(new Vector3(camera.transform.forward.x, 0f, camera.transform.forward.z));
                        allyUI.SpeakAffirmative();
                    }
                    else if (!ability.HasEnoughEnergy() && ability != null)
                        allyUI.SpeakNoEnergy();
                    else if (!ability.IsOffCooldown() || ally.isDisabled)
                        allyUI.SpeakOccupied();
                }
            }
            // Making ally/allies use intel abilities if they are ready
            else if (optionNumber == 6)
            {
                foreach (Entity ally in targetAudience)
                {
                    var ability = ally.gameObject.GetComponent<IntelAbility>();
                    var allyUI = ally.gameObject.GetComponentInChildren<SquadMemberUI>();

                    if (ability.CanUseAbility() && ability != null)
                    {
                        Dictionary<GameObject, float> locationsList = new Dictionary<GameObject, float>();
                        var output = "";
                        locationsList = ability.GetLocations(camera.transform);

                        if (locationsList.Count > 0)    // Making ally say where locations are
                        {
                            foreach (GameObject thing in locationsList.Keys)
                                output += "[" + locationsList[thing] + "] ";
                            allyUI.OverrideSpeak(output);
                        }
                        else    // If ally has nothing to say (no locations nearby)
                            allyUI.OverrideSpeak("Nothing to report.");
                    }
                    else if (!ability.HasEnoughEnergy() && ability != null)
                        allyUI.SpeakNoEnergy();
                    else if (!ability.IsOffCooldown() || ally.isDisabled)
                        allyUI.SpeakOccupied();
                }
            }
            readyToSendOrder = false;
            toggleOn = false;
        }
    }
}
