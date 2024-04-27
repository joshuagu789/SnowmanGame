using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script given to every canvas/grouping of UI. CanvasUI manages the buttons on the canvas, duration of the canvas, and when the canvas is displayed.
/// Script should be attached to the Canvas GameObject.
/// </summary>
public class CanvasUI : MonoBehaviour
{
    [SerializeField]
    private UI_Manager.UI_Type UI_Type;

    private void Awake()
    {
        UI_Manager.TryAddCanvas(UI_Type, GetComponent<Canvas>());   // throw error if try add fails?
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
