/*
 * Designed for player and squad members to chit-chat with each other when not in combat through voice lines organized into dictionaries
 *  
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundDialogue : MonoBehaviour
{
    /*
     * Each ally (including the player) has a personal dictionary storing all their chatter voice lines (background chit-chat in the game).
     * They interact with each other through questions and answers. An odd numbered key represents a question while an even numbered key represents an answer.
     * The dictionaries' keys represent the type of dialogue list it unlocks: 0 is in-combat voice lines, 1 is asking-about-well-being voice lines,
     * 2 is answering-about-well-being voice lines, 3 is asking-about-nearby-enemies voice lines, 4 is answering 3, 5 is self-remarks which is why 6 
     * is blank (so that answerers respond nothing)
     */

    public SquadMemberUI player_UI;
    private Dictionary<int, List<string>> playerVoiceLines = new Dictionary<int, List<string>>
    // Player's chatter voice lines
    { {0, new List<string>{"Focus, we enter combat!",}}, {1, new List<string>{"How are we on energy?","How is everyone on energy?","Report energy."}}, {2, new List<string>{"Lets try to conserve as much as possible.",}},
    {3, new List<string>{"Any enemies approaching us?","Report: any enemies watching us?"}}, {4, new List<string>{"I don't think so.","Maybe. Stay alert."}},
    {5, new List<string>{"Let's hurry, before dawn breaks.","Keep moving, and stay alert."}}, {6, new List<string>{""}}, };


    public SquadMemberUI modelB_UI;
    private Dictionary<int, List<string>> modelB_VoiceLines = new Dictionary<int, List<string>>
    // Model B's chatter voice lines
    { {0, new List<string>{"Enough chatter, I have eyes on target!",}}, {1, new List<string>{"How are we on energy?","How is our energy?"}}, {2, new List<string>{"I just hope it lasts.","We just need enough to last the night.","I will make do with what I have left."}},
    {3, new List<string>{"My side is clear. Any enemies on the others?",}}, {4, new List<string>{"I don't think so.","No.","None close enough to pose a threat."}},
    {5, new List<string>{"Stay calm, and we survive the night."}}, {6, new List<string>{""}}, };


    public SquadMemberUI modelC_UI;
    private Dictionary<int, List<string>> modelC_VoiceLines = new Dictionary<int, List<string>> 
    // Model C's chatter voice lines
    { {0, new List<string>{"Wait, we enter combat!",}}, { 1, new List<string> { "How is everyone on energy?", }}, { 2, new List<string> { "I'll try to ration it.", "Let's avoid combat to preserve it."}},
    { 3, new List<string> { "Any enemies approaching us?", }}, { 4, new List<string> { "Nothing on my side.", }},
    {5, new List<string>{"Let's keep moving.","Let's avoid combat whenever we can, alright?","Have faith, friends. We will make it through."}}, {6, new List<string>{""}}, };


    public SquadMemberUI modelD_UI;
    private Dictionary<int, List<string>> modelD_VoiceLines = new Dictionary<int, List<string>>
    // Model D's chatter voice lines
    { {0, new List<string>{"Not now, enemies incoming!",}}, { 1, new List<string> { "How are we on energy?", }}, { 2, new List<string> { "Lets try to conserve as much as possible.", }},
    { 3, new List<string> { "Any enemies approaching us?", }}, { 4, new List<string> { "I don't think so.", "If only my optics can see through this fog."}},
    {5, new List<string>{"Perhaps we should scavenge for supplies?"}}, {6, new List<string>{""}}, };


    private Dictionary<SquadMemberUI, Dictionary<int, List<string>>> chatterVoiceLines = new Dictionary<SquadMemberUI, Dictionary<int, List<string>>>();
    private List<SquadMemberUI> squadMemberUIs = new List<SquadMemberUI>();     // Contains 4 members of team and a bool for if they're able to speak
    private List<SquadMemberUI> speakers = new List<SquadMemberUI>();

    private int potentialSpeakers;
    private float timer = 0f;
    private bool ableToTalk = true;
    private float timerThreshold = 7f;
    private float maxTimeInterval = 25f;
    private float minTimeInterval = 10f;

    // Start is called before the first frame update
    void Start()
    {
        // This is a bit of a messy way of creating a dictionary but Unity can't display dictionaries in the inspector
        chatterVoiceLines.Add(player_UI, playerVoiceLines);
        chatterVoiceLines.Add(modelB_UI, modelB_VoiceLines);
        chatterVoiceLines.Add(modelC_UI, modelC_VoiceLines);
        chatterVoiceLines.Add(modelD_UI, modelD_VoiceLines);

        squadMemberUIs.Add(player_UI);
        squadMemberUIs.Add(modelB_UI);
        squadMemberUIs.Add(modelC_UI);
        squadMemberUIs.Add(modelD_UI);
    }

    // Update is called once per frame
    private void Update()
    {
        UpdatePotentialSpeakers();  // Updates the variable potentialSpeakers
        timer += Time.deltaTime;
        if (timer >= timerThreshold && ableToTalk && potentialSpeakers >= 2)    // Can only start interaction if there are at least 2 of party not fighting nor dead
        {
            timerThreshold = Random.Range(minTimeInterval, maxTimeInterval);
            ableToTalk = false;
            StartCoroutine(StartInteraction());
        }
    }

    private void UpdatePotentialSpeakers()
    {
        int counter = 0;
        foreach (SquadMemberUI speaker in squadMemberUIs)
        {
            var snowman = speaker.gameObject.GetComponent<Snowman>();
            if (!snowman.isLockedOn && !snowman.animator.GetBool("isMelting")) { counter++; }
        }
        potentialSpeakers = counter;
    }

    private IEnumerator StartInteraction()
    {
        speakers.Clear();
        SelectSpeakers();   // Modifying the list speakers which contains the members who will be talking

        bool canRespond = true;

        var dialogueCategory = (int)Random.Range(0, (int)chatterVoiceLines[speakers[0]].Keys.Count/2)*2 + 1;  // Selecting random odd number since question dialogues are in odd indexes
        var dialogueList = chatterVoiceLines[speakers[0]][dialogueCategory];    // Selecting list of dialogues that questioner can speak from
        if (!speakers[0].ableToSpeak)   // Indicates that the first speaker (questioner) cannot speak so the answerer(s) shouldn't be able to respond
            canRespond = false;
        speakers[0].Speak(dialogueList[(int)Random.Range(0, dialogueList.Count)]);

        yield return new WaitForSeconds(2.5f);
        dialogueCategory++;     // Changes dialogueCategory to even number- Even number after odd number always answers the dialogues posed in odd number

        for (int i = 1; i < speakers.Count; i++)    // Looping through the answerers
        {
            var snowman = speakers[i].gameObject.GetComponent<Snowman>();

            if (!snowman.animator.GetBool("isMelting") && canRespond && !snowman.isLockedOn)
            {
                dialogueList = chatterVoiceLines[speakers[i]][dialogueCategory];
                speakers[i].Speak(dialogueList[(int)Random.Range(0, dialogueList.Count)]);
            }
            else if (snowman.isLockedOn && !snowman.animator.GetBool("isMelting") && canRespond)    // Makes answerer respond with dialogueCategory of zero since they are in combat
            {
                dialogueList = chatterVoiceLines[speakers[i]][0];
                speakers[i].Speak(dialogueList[(int)Random.Range(0, dialogueList.Count)]);
            }
        }

        timer = 0;
        ableToTalk = true;
    }

    // Selecting and putting all the speakers for interaction into a list with questioner in index 0 and answerer(s) 
    private void SelectSpeakers()
    {
        while (speakers.Count < 2)
        {
            var speaker = squadMemberUIs[(int)Random.Range(0, squadMemberUIs.Count)];
            var snowman = speaker.gameObject.GetComponent<Snowman>();

            // Adding the randomly selected speaker to list speakers- the questioner can't be in combat but the answerer(s) can be
            if (speakers.Count == 0 && !speakers.Contains(speaker) && !snowman.isLockedOn && !snowman.animator.GetBool("isMelting"))
                speakers.Add(speaker);
            else if (speakers.Count != 0 && !speakers.Contains(speaker) && !snowman.animator.GetBool("isMelting"))
                speakers.Add(speaker);
        }
    }
}
