/*
 * Designed for player and squad members to chit-chat with each other when not in combat through voice lines organized into dictionaries
 *  
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public SquadMemberUI player_UI;
    private Dictionary<int, List<string>> playerVoiceLines = new Dictionary<int, List<string>> { { 1, new List<string> { "a", "b" } } };

    public SquadMemberUI modelB_UI;
    private Dictionary<int, List<string>> modelB_VoiceLines = new Dictionary<int, List<string>> { };

    public SquadMemberUI modelC_UI;
    private Dictionary<int, List<string>> modelC_VoiceLines = new Dictionary<int, List<string>> { };

    public SquadMemberUI modelD_UI;
    private Dictionary<int, List<string>> modelD_VoiceLines = new Dictionary<int, List<string>> { };

    private SortedDictionary<SquadMemberUI, Dictionary<int, List<string>>> chatterVoiceLines = new SortedDictionary<SquadMemberUI, Dictionary<int, List<string>>>();

    // Start is called before the first frame update
    void Start()
    {
        chatterVoiceLines.Add(player_UI, playerVoiceLines);
        chatterVoiceLines.Add(modelB_UI, modelB_VoiceLines);
        chatterVoiceLines.Add(modelC_UI, modelC_VoiceLines);
        chatterVoiceLines.Add(modelD_UI, modelD_VoiceLines);
        SquadMemberUI a = null;
        if (3 > 1) { a = player_UI; }
        print(chatterVoiceLines[a][1]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
