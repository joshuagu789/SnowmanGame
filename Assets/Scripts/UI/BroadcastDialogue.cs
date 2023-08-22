using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BroadcastDialogue : MonoBehaviour
{
    public Player player;
    public List<SquadMemberUI> partyList;   // Index 0 is player, 1 is model b, 2 is model c, 3 is model d
    public GameObject broadcaster;
    public TextMeshProUGUI broadcastText;
    public Image image;
    public GameObject imageBackground;

    // Start is called before the first frame update
    void Start()
    {
        broadcaster.SetActive(false);
        broadcastText.SetText("");
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Making broadcast box visible then outputting all squad member's dialogues onto it one after another
    //  - Each row in the lists represents a string piece of dialogue that the corresponding squad member will say
    //  - Coroutine is wordy with multiple sections but I'm afraid that things will go wrong if I separate them out into multiple coroutines
    //  - Assumes both lists are of the same length 
    private IEnumerator Broadcast(List<SquadMemberUI> allies, List<string> dialogueList)
    {
        imageBackground.SetActive(false);

        // Creates special effect where the broadcast box turns on and off multiple times ending with broadcaster on
        for (int i = 0; i < 3; i++)
        {
            broadcaster.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            broadcaster.SetActive(true);
            yield return new WaitForSeconds(0.05f);
        }
        imageBackground.SetActive(true);

        // Looping through each participant in dialogue and making them say their corresponding line
        for(int i = 0; i < allies.Count; i++)
        {
            image.sprite = allies[i].imageField.sprite; // Matching up speaker's portrait with the portrait inside the broadcaster
            string output = "";
            foreach (char letter in dialogueList[i])
            {
                // Outputting text
                output += letter;
                broadcastText.SetText(output);
                if (letter.Equals(".") || letter.Equals(",") || letter.Equals("!") || letter.Equals("?") || letter.Equals(":")) // Pausing at these symbols
                    yield return new WaitForSeconds(1f);
                else if (!letter.Equals(" "))
                    yield return new WaitForSeconds(0.03f);
            }
            yield return new WaitForSeconds(3f);
        }

        // Creates special effect where the broadcast box turns on and off multiple times ending with broadcaster off
        for (int i = 0; i < 3; i++)
        {
            broadcaster.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            broadcaster.SetActive(false);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void StartWaveDialogue()
    {
        // Preparing the list of all candidates to be part of dialogue
        List<SquadMemberUI> possibleSpeakersList = new List<SquadMemberUI> { player.gameObject.GetComponent<SquadMemberUI>() };
        foreach (Entity ally in player.squadList)
        {
            possibleSpeakersList.Add(ally.gameObject.GetComponent<SquadMemberUI>());
        }

        // Preparing the two lists to send to Broadcast(List, List) as long as they are filled up enough
        List<SquadMemberUI> speakersList = new List<SquadMemberUI>();
        List<string> dialogueList = new List<string>();

        // Randomly adding allies to speakersList and their corresponding voiceline to dialogueList
        while (possibleSpeakersList.Count > 1 && speakersList.Count < 2)
        {
            var possibleSpeaker = possibleSpeakersList[(int)Random.Range(0, possibleSpeakersList.Count)];   // Selecting random ally (which includes player)
            if (!possibleSpeaker.gameObject.GetComponent<Snowman>().animator.GetBool("isMelting"))
            {
                speakersList.Add(possibleSpeaker);

                // The first speaker will use an alert voice line while the rest use alert response voice lines
                if (speakersList.Count == 1)
                    dialogueList.Add(possibleSpeaker.alertVoiceLines[(int)Random.Range(0, possibleSpeaker.alertVoiceLines.Count)]);
                else
                    dialogueList.Add(possibleSpeaker.alertResponseVoiceLines[(int)Random.Range(0, possibleSpeaker.alertResponseVoiceLines.Count)]);
            }
            possibleSpeakersList.Remove(possibleSpeaker);
        }

        if (speakersList.Count >= 2)
            StartCoroutine(Broadcast(speakersList, dialogueList));
    }
}
