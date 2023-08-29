/*
 * Script similar to PlayerCommands but for interacting with a build menu
 */
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBuilder : MonoBehaviour
{
    [SerializeField]
    private Player player;
    [SerializeField]
    private List<GameObject> buildings = new List<GameObject>();
    [SerializeField]
    private GameObject buildMenu;
    [SerializeField]
    private TextMeshProUGUI outputTitle;
    [SerializeField]
    private TextMeshProUGUI outputBox;
    [SerializeField]
    private Spawner spawner;

    private Dictionary<int, KeyCode> keycodes = new Dictionary<int, KeyCode> { {1, KeyCode.Alpha1 }, {2, KeyCode.Alpha2 }, {3, KeyCode.Alpha3 },
    {4, KeyCode.Alpha4 }, {5, KeyCode.Alpha5 }, {6, KeyCode.Alpha6 }, {7, KeyCode.Alpha7 }, {8, KeyCode.Alpha8 }, {9, KeyCode.Alpha9 }, {10, KeyCode.Alpha0 } };

    private bool toggleOn;
    private bool readyToSendOrder = false;
    private int optionsCount;

    private void Awake()
    {
        buildMenu.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        // Updating if the command bar should be displayed
        if (Input.GetKeyDown(KeyCode.B))
            toggleOn = !toggleOn;
        if (toggleOn)
            DisplayBuildMenu();
        else
            buildMenu.SetActive(false);
        if(readyToSendOrder)
            CheckCommands();
    }

    // Making build menu visible with all options
    private void DisplayBuildMenu()
    {
        if (!readyToSendOrder)
        {
            buildMenu.SetActive(true);
            optionsCount = 0;
            var output = "";
            foreach (GameObject building in buildings)
            {
                optionsCount++;
                output += optionsCount + ". " + building.name + "\n";
            }
            optionsCount++;
            output += optionsCount + ". " + "Cancel\n";
            outputTitle.text = "Select Building:";
            outputBox.text = output;
            readyToSendOrder = true;
        }
    }

    // Checking to see if an input corresponds with command option
    private void CheckCommands()
    {
        if (toggleOn && Input.anyKeyDown)
            foreach (int index in keycodes.Keys)
                if (index <= optionsCount && Input.GetKeyDown(keycodes[index]))
                    AttemptBuild(index);
    }

    private void AttemptBuild(int optionNumber)
    {
        if (optionNumber == optionsCount)   // The "Cancel" button
        {
            toggleOn = false;
            readyToSendOrder = false;
        }
        else
        {
            var requiredMaterials = buildings[optionNumber - 1].GetComponent<Building>().buildCost;  // List of types of materials needed and their amounts

            if (HasMaterials(requiredMaterials))
            {
                // Deleting itemObjects and their amounts from entity and its allies to spawn building
                SpendMaterials(requiredMaterials);
                spawner.SpawnSpecific(buildings[optionNumber - 1], 1, player.transform.position + player.transform.forward * 3, 0, 0);
                toggleOn = false;
                readyToSendOrder = false;
            }
            else
            {
                toggleOn = false;   // Have dialogue for not enough materials?
                readyToSendOrder = false;
                player.gameObject.GetComponent<PlayerUI>().OverrideSpeak("Not enough materials.");
            }
        }
    }

    // Checking to see if player and their team's total storage contains required components to build structure
    private bool HasMaterials(List<Item> requiredMaterials)
    {
        bool hasMaterials = true;

        foreach (Item item in requiredMaterials)
        {
            int amountStillNeeded = item.GetAmount(); 
            List<Entity> allies = new List<Entity> { player };
            allies.AddRange(allies[0].squadList);

            foreach (Entity ally in allies)
            {
                var allyStorage = ally.gameObject.GetComponent<StorageUnit>();

                if (allyStorage != null && allyStorage.GetStorage().ContainsKey(item.GetItem()))
                {
                    if (amountStillNeeded <= allyStorage.GetStorage()[item.GetItem()])    // If ally's storage has enough to meet amount of the item needed
                    {
                        amountStillNeeded -= allyStorage.GetStorage()[item.GetItem()];
                        break;
                    }
                    else
                    {
                        amountStillNeeded -= allyStorage.GetStorage()[item.GetItem()];
                    }
                }
            }
            if (amountStillNeeded > 0)
            {
                hasMaterials = false;
                break;
            }
        }

        return hasMaterials;
    }

    private void SpendMaterials(List<Item> requiredMaterials)
    {
        foreach (Item item in requiredMaterials)
        {
            int amountStillNeeded = item.GetAmount();
            List<Entity> allies = new List<Entity> { player };
            allies.AddRange(allies[0].squadList);

            foreach (Entity ally in allies)
            {
                var allyStorage = ally.gameObject.GetComponent<StorageUnit>();

                if (allyStorage != null && allyStorage.GetStorage().ContainsKey(item.GetItem()))
                {
                    if (amountStillNeeded <= allyStorage.GetStorage()[item.GetItem()])    // If ally's storage has enough to meet amount of the item needed
                    {
                        int amount = amountStillNeeded;
                        amountStillNeeded -= allyStorage.GetStorage()[item.GetItem()];
                        ally.GetComponent<StorageUnit>().DeleteItem(item.GetItem(), amount);
                        break;
                    }
                    else    // If ally doesn't have enough of the specific item
                    {
                        int amount = amountStillNeeded;
                        amountStillNeeded -= allyStorage.GetStorage()[item.GetItem()];
                        ally.GetComponent<StorageUnit>().DeleteItem(item.GetItem(), amount);
                    }
                }
            }
        }
    }
}
